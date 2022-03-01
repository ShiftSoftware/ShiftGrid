using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ShiftSoftware.ShiftGrid.Core
{
    public class Grid<T>
    {
        #region Public Props

        public int DataPageIndex { get; set; }
        public int DataPageSize { get; set; }
        public List<object> Data { get; set; }
        public Dictionary<string, object> Summary { get; set; }
        public List<GridSort> Sort { get; set; }
        public List<GridFilter> Filters { get; set; }
        public List<GridColumn> Columns { get; set; }
        public GridPagination Pagination { get; set; }

        #endregion

        #region Public Methods
        public Grid()
        {
            this.Filters = new List<GridFilter> { };
            this.Sort = new List<GridSort> { };
            this.Columns = new List<GridColumn> { };
            this.DataPageSize = 20;
            this.Data = new List<object>();
            this.Pagination = new GridPagination() { PageSize = 10 };
        }

        public string ToCSVString()
        {
            var engine = this.GetCSVEngine();

            return engine.WriteString(GetExportableData());
        }

        public MemoryStream ToCSVStream()
        {
            var engine = this.GetCSVEngine();

            var stream = new MemoryStream();
            var gisstreamWriter = new StreamWriter(stream, System.Text.Encoding.UTF8);

            engine.WriteStream(gisstreamWriter, GetExportableData());

            gisstreamWriter.Flush();

            return stream;
        }

        public void SaveCSV(string path)
        {
            var engine = this.GetCSVEngine();

            engine.WriteFile(path, GetExportableData());
        }

        #endregion

        private FileHelpers.FileHelperEngine GetCSVEngine()
        {
            if (!this.ExportMode)
                throw new Exception("Export Mode is not Active. ExportConfig.Export must be marked as True.");

            var engine = new FileHelpers.FileHelperEngine(typeof(T), System.Text.Encoding.UTF8);

            var excludedFields = this.TypeColumns.Where(x => !this.Columns.Any(y => y.Field == x.Field));

            foreach (var excluded in excludedFields)
            {
                if (engine.Options.FieldsNames.Any(x => x == excluded.Field))
                    engine.Options.RemoveField(excluded.Field);
            }

            engine.HeaderText = engine.GetFileHeader();

            return engine;
        }

        private IEnumerable<object> GetExportableData()
        {
            if (this.Data.Count > 0 && this.Data.FirstOrDefault().GetType() == typeof(T))
                return this.Data;

            return this.Data.Select(x => ConvertSelectedItem(x));
        }
        private object ConvertSelectedItem(object dataItem)
        {
            var t = (T)Activator.CreateInstance(typeof(T));

            var dataOjectProps = dataItem.GetType().GetProperties().Where(y => y.MemberType == System.Reflection.MemberTypes.Property).ToList();

            var modelProps = typeof(T).GetProperties().Where(y => y.MemberType == System.Reflection.MemberTypes.Property).ToList();

            foreach (var dataField in dataOjectProps)
            {
                var targetField = modelProps.Where(y => y.Name == dataField.Name).FirstOrDefault();

                if (targetField == null)
                    continue;

                var value = dataField.GetValue(dataItem);

                targetField.SetValue(t, value);

            }

            return (object)t;
        }

        #region Private & Internal Props

        internal IQueryable<T> Select { get; set; }
        internal Expression<Func<IGrouping<int, T>, object>> SummarySelect { get; set; }
        private IQueryable ProccessedSelect { get; set; }
        private IQueryable<T> SummaryProcessedSelect { get; set; }
        private List<GridColumn> TypeColumns { get; set; }
        public bool ExportMode { get; set; }

        #endregion

        #region Private & Internal Methods
        internal Grid<T> Init(GridConfig payload = null)
        {
            this.GridConfig = payload;

            this.Prepare(payload);
            this.GenerateQuery();
            var data = this.GetPaginatedQuery().ToDynamicList();
            this.Data.AddRange(data);
            this.GenerateColumns();
            this.LoadSummary();
            this.EnsureSummary();
            this.ProcessPagination();

            return this;
        }
        internal async Task<Grid<T>> InitAsync(GridConfig payload = null)
        {
            this.GridConfig = payload;

            this.Prepare(payload);
            this.GenerateQuery();
            var data = await this.GetPaginatedQuery().ToDynamicListAsync();
            this.Data.AddRange(data);
            this.GenerateColumns();
            await this.LoadSummaryAsync();
            this.EnsureSummary();
            this.ProcessPagination();

            return this;
        }

        private void Prepare(GridConfig payload = null)
        {
            if (payload != null)
            {
                if (payload.Sort != null)
                    this.Sort = payload.Sort;

                if (payload.Filters != null)
                    this.Filters = payload.Filters;

                if (payload.Pagination != null)
                    this.Pagination.PageSize = payload.Pagination.PageSize;

                this.DataPageIndex = payload.DataPageIndex;

                if (payload.DataPageSize != 0)
                    this.DataPageSize = payload.DataPageSize;

                this.Columns = payload.Columns;

                if (payload.ExportConfig != null && payload.ExportConfig.Export)
                {
                    this.DataPageIndex = 0;
                    this.DataPageSize = -1;
                    this.ExportMode = true;
                }
            }

            if (this.Sort.Count == 0)
            {
                this.Sort.Add(new GridSort
                {
                    Field = typeof(T).GetProperties().Where(x => x.MemberType == System.Reflection.MemberTypes.Property).First().Name,
                    SortDirection = SortDirection.Ascending
                });
            }
        }
        private void GenerateQuery()
        {
            var select = this.Select.Where("1=1");

            foreach (var filter in this.Filters)
            {
                var expressions = new List<string>();
                var values = new List<object>();

                var filterGroups = new List<GridFilter>();

                filterGroups.Add(filter);

                if (filter.OR != null)
                    filterGroups.AddRange(filter.OR);

                var index = 0;
                foreach (var theFilter in filterGroups)
                {
                    var valuePrefix = Mappings.OperatorValuePrefix.Keys.Contains(theFilter.Operator) ? Mappings.OperatorValuePrefix[theFilter.Operator] : "";
                    var valuePostfix = Mappings.OperatorValuePostfix.Keys.Contains(theFilter.Operator) ? Mappings.OperatorValuePostfix[theFilter.Operator] : "";

                    var value = $"@{index}";
                    var field = theFilter.Field;

                    if (theFilter.Operator == GridFilterOperator.In)
                    {
                        value = $"{theFilter.Field}";
                        field = $"@{index}";

                        Type elementType = select.ElementType;

                        var dataType = elementType.GetProperties().First(x => x.Name == theFilter.Field).PropertyType;
                        Type listType = typeof(List<>).MakeGenericType(dataType);

                        //Provided by Client from a Jobject
                        if (theFilter.Value.GetType() == typeof(JArray))
                        {
                            var valueList = (IList)Activator.CreateInstance(listType);

                            foreach (var item in (theFilter.Value as JArray).Select(x => x.ToObject(dataType)))
                                valueList.Add(item);

                            theFilter.Value = valueList;
                        }
                        else
                        {
                            if (listType != theFilter.Value.GetType())
                                throw new Exception($"The provided collection for filtering '{theFilter.Field}' does not match the data type of the Field. The Field Type is ({dataType.ToString()}). And the Collection Type is ({theFilter.Value.GetType().ToString()})");
                        }
                    }

                    if (theFilter.Value == null)
                        value = "null";
                    else
                        values.Add(theFilter.Value);

                    var theOperator = Mappings.OperatorMapping[theFilter.Operator];

                    var theExpression = $"{field}{theOperator}{valuePrefix}{value}{valuePostfix}";

                    expressions.Add(theExpression);

                    index++;
                }

                var expression = string.Join(" || ", expressions);

                select = select.Where(expression, values.ToArray());
            }

            this.SummaryProcessedSelect = select;

            IQueryable newSelect;

            if (this.Columns != null && this.Columns.Count > 0)
            {
                var fields = string.Join(", ", this.Columns.Select(x => x.Field));
                newSelect = select.Select($"new ({fields})");
            }
            else
                newSelect = select;

            this.ProccessedSelect = newSelect;
        }
        private IQueryable GetPaginatedQuery()
        {
            IQueryable sort = this.ProccessedSelect.OrderBy(string.Join(", ", this.Sort.Select(x => $"{x.Field} {(x.SortDirection == SortDirection.Descending ? "desc" : "")}")));

            this.Data = new List<object>();

            IQueryable dataToIterate;

            if (this.DataPageSize != -1)
                dataToIterate = sort.Skip((DataPageIndex * DataPageSize)).Take(DataPageSize);
            else
                dataToIterate = sort;

            return dataToIterate;
        }
        private void GenerateColumns()
        {
            var columns = new List<GridColumn>();

            //var jsonResolver = new PropertyRenameAndIgnoreSerializerContractResolver();

            var order = 0;

            var props = typeof(T).GetProperties().Where(x => x.MemberType == System.Reflection.MemberTypes.Property).ToList();

            if (props.Count == 0)
                props = this.Data.FirstOrDefault().GetType().GetProperties().Where(x => x.MemberType == System.Reflection.MemberTypes.Property).ToList();

            foreach (var col in props)
            {
                var customAttribute = (GridColumn)col.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(GridColumn));

                columns.Add(new GridColumn
                {
                    Field = col.Name,
                    Order = customAttribute == null ? order : customAttribute.Order,
                    HeaderText = customAttribute == null ? col.Name : customAttribute.HeaderText
                });

                order++;
            }

            this.TypeColumns = columns;

            if (this.Columns == null)
            {
                this.Columns = columns;
            }

            this.Columns.ForEach((x) =>
            {
                x.HeaderText = columns.FirstOrDefault(y => y.Field == x.Field)?.HeaderText;
            });

            //foreach (var ignoredCol in this.TypeColumns.Where(x => !this.Columns.Any(y => y.Field == x.Field)))
            //    jsonResolver.IgnoreProperty(typeof(T), ignoredCol.Field);

            //var serializerSettings = new JsonSerializerSettings();
            //serializerSettings.ContractResolver = jsonResolver;

            //this.JsonSerializerSettings = serializerSettings;
        }
        private void LoadSummary()
        {
            if (this.ExportMode)
                return;

            if (this.SummarySelect != null)
            {
                var summary = this.GetGroupedSummary().ToDynamicList().FirstOrDefault();
                this.ProcessSummary(ref summary);
            }
        }
        private async Task LoadSummaryAsync()
        {
            if (this.ExportMode)
                return;

            if (this.SummarySelect != null)
            {
                var summary = (await this.GetGroupedSummary().ToDynamicListAsync()).FirstOrDefault();
                this.ProcessSummary(ref summary);
            }
        }
        private void ProcessSummary(ref object summary)
        {
            this.Summary = new Dictionary<string, object> { };

            foreach (var property in SummarySelect.Body.Type.GetProperties().Where(x => x.MemberType == System.Reflection.MemberTypes.Property).ToList())
            {
                var summaryProperty = summary == null ? null : (summary as object).GetType().GetProperties().Where(x => x.MemberType == System.Reflection.MemberTypes.Property && x.Name == property.Name).FirstOrDefault();

                this.Summary[property.Name] = summaryProperty == null ? Activator.CreateInstance(property.PropertyType) : summaryProperty.GetValue(summary);
            }
        }
        private IQueryable<object> GetGroupedSummary()
        {
            return this.SummaryProcessedSelect.GroupBy(x => Math.Abs(1)).Select(this.SummarySelect);
        }
        private void EnsureSummary()
        {
            if (this.Summary == null)
                this.Summary = new Dictionary<string, object> { };

            if (!this.Summary.Keys.Contains("Count") && !this.ExportMode)
                this.Summary["Count"] = this.ProccessedSelect.Count();
        }
        private void ProcessPagination()
        {
            if (this.ExportMode)
                return;

            var dataCount = (int) this.Summary["Count"];

            //Show All
            if (this.DataPageSize == -1)
            {
                this.Pagination.Count = 1;
                this.DataPageIndex = 0;
                this.Pagination.PageSize = 1;
            }
            else
            {
                this.Pagination.Count = dataCount / this.DataPageSize + (dataCount % this.DataPageSize > 0 ? 1 : 0);
            }

            this.Pagination.PageIndex = this.DataPageIndex % this.Pagination.PageSize;

            this.Pagination.PageStart = (this.DataPageIndex / this.Pagination.PageSize) * this.Pagination.PageSize;
            this.Pagination.PageEnd = this.Pagination.PageStart + (this.Pagination.PageSize - 1);

            this.Pagination.LastPageIndex = this.Pagination.Count - 1;

            if (this.Pagination.Count == 0)
                this.Pagination.LastPageIndex = 0;

            if (this.Pagination.PageEnd > this.Pagination.LastPageIndex)
                this.Pagination.PageEnd = this.Pagination.LastPageIndex;

            this.Pagination.HasPreviousPage = this.Pagination.PageSize > 1 && this.Pagination.PageStart > 0;
            this.Pagination.HasNextPage = this.Pagination.LastPageIndex > this.Pagination.PageEnd;

            this.Pagination.DataStart = (this.DataPageIndex * this.DataPageSize) + 1;

            if (dataCount == 0)
                this.Pagination.DataStart = 0;

            this.Pagination.DataEnd = this.Pagination.DataStart + this.DataPageSize - 1;

            if (this.Pagination.DataEnd > dataCount)
                this.Pagination.DataEnd = dataCount;

            if (this.DataPageSize == -1)
                this.Pagination.DataEnd = dataCount;
        }
        private GridConfig GridConfig { get; set; }

        #endregion
    }
}
