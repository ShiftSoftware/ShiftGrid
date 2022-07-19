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
    public class Grid<T, T2>
    {
        #region Public Props

        public int DataPageIndex { get; set; }
        public int DataPageSize { get; set; }
        public int DataCount { get; set; } = -1;
        public List<T> Data { get; set; }
        public T2 Aggregate { get; set; }
        public List<GridSort> Sort { get; set; }
        public GridSort StableSort { get; set; }
        public List<GridFilter> Filters { get; set; }
        public List<GridColumn> Columns { get; set; }
        public GridPagination Pagination { get; set; }
        public DateTime BeforeLoadingData { get; set; }
        public DateTime AfterLoadingData { get; set; }

        #endregion

        #region Public Methods
        public Grid()
        {
            this.Filters = new List<GridFilter> { };
            this.Sort = new List<GridSort> { };
            this.Columns = new List<GridColumn> { };
            this.DataPageSize = 20;
            this.Data = new List<T>();
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

        #region Private & Internal Props

        private ExportConfig ExportConfig { get; set; }
        internal IQueryable<T> Select { get; set; }
        internal Expression<Func<IGrouping<int, T>, T2>> AggregateSelect { get; set; }
        private IQueryable<T> ProccessedSelect { get; set; }
        private bool ExportMode { get; set; }

        #endregion

        #region Private & Internal Methods
        internal Grid<T, T2> Init(GridConfig payload = null)
        {
            this.GridConfig = payload;

            this.Prepare(payload);
            this.GenerateQuery();
            this.LoadAggregate();
            this.ProcessPagination();

            this.BeforeLoadingData = DateTime.UtcNow;
            var data = this.GetPaginatedQuery().ToDynamicList<T>();
            this.AfterLoadingData = DateTime.UtcNow;
            this.Data.AddRange(data);
            this.GenerateColumns();
            return this;
        }
        internal async Task<Grid<T, T2>> InitAsync(GridConfig payload = null)
        {
            this.GridConfig = payload;

            this.Prepare(payload);
            this.GenerateQuery();
            await this.LoadAggregateAsync();
            this.ProcessPagination();

            this.BeforeLoadingData = DateTime.UtcNow;
            var data = await this.GetPaginatedQuery().ToDynamicListAsync<T>();
            this.AfterLoadingData = DateTime.UtcNow;
            this.Data.AddRange(data);
            this.GenerateColumns();
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

                if (payload.Columns != null)
                    this.Columns = payload.Columns;

                if (payload.ExportConfig != null && payload.ExportConfig.Export)
                {
                    this.DataPageIndex = 0;
                    this.DataPageSize = -1;
                    this.ExportMode = true;
                    this.ExportConfig = payload.ExportConfig;
                }
            }
        }
        private void GenerateQuery()
        {
            var hiddenColumns = this.Columns?
                    .Where(x => !x.Visible)?
                    .Select(x => x.Field)?
                    .ToList();

            var tType = typeof(T).ToString();

            //This was a dirty fix to prevent people from trying to Hide Columns
            //When a Custom Model is not provided.
            //This worked (Somewhat fine). But they can use db.TestItems.ToShiftGrid()
            //In this case the exception is not thrown
            //if (hiddenColumns != null && hiddenColumns.Count > 0 && tType.Contains("<>") && tType.Contains("AnonymousType"))
            //{
            //    throw new AnonymousColumnHidingException("Hiding Columns on Anonymous Objects is not allowed on this version. It might become available in future versions.");
            //}

            if (hiddenColumns != null && hiddenColumns.Count > 0)
            {
                var selectType = this.Select.GetType().ToString();

                if (selectType.StartsWith("System.Data.Entity.DbSet") || selectType.StartsWith("Microsoft.EntityFrameworkCore.Internal.InternalDbSet"))
                {
                    throw new ColumnHidingException("Hiding Columns on DBSet is not allowed on this version. It might become available in future versions.");
                }
            }

            if (hiddenColumns != null && hiddenColumns.Count > 0)
            {
                if (tType.Contains("<>") && tType.Contains("AnonymousType"))
                    this.Select = new AnonymousColumnRemover<T>(this.Select).RemoveColumns(hiddenColumns);
                else
                    this.Select = new ColumnRemover<T>(this.Select).RemoveColumns(hiddenColumns);

                if (this.AggregateSelect != null)    
                    this.AggregateSelect = new AggregateColumnRemover<T, T2>(this.AggregateSelect).RemoveColumns(hiddenColumns);
            }

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

                    
                    if (theFilter.Value?.GetType() == typeof(Newtonsoft.Json.Linq.JArray) || theFilter.Value?.GetType() == typeof(System.Text.Json.JsonElement))
                    {
                        Type elementType = select.ElementType;
                        var dataType = elementType.GetProperties().First(x => x.Name == theFilter.Field).PropertyType;
                        Type listType = typeof(List<>).MakeGenericType(dataType);
                        var valueList = (IList)Activator.CreateInstance(listType);

                        if (theFilter.Value.GetType() == typeof(Newtonsoft.Json.Linq.JArray))
                        {
                            foreach (var item in (theFilter.Value as Newtonsoft.Json.Linq.JArray).Select(x => x.ToObject(dataType)))
                                valueList.Add(item);

                            theFilter.Value = valueList;
                        }
                        else if (theFilter.Value.GetType() == typeof(System.Text.Json.JsonElement))
                        {
                            var jsonElement = (System.Text.Json.JsonElement)theFilter.Value;

                            if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                            {
                                foreach (var item in jsonElement.EnumerateArray())
                                {
                                    valueList.Add(parseJsonElement(item, dataType));
                                }

                                theFilter.Value = valueList;
                            }
                            else
                            {
                                theFilter.Value = parseJsonElement(jsonElement, dataType);
                            }
                        }
                    }
                    

                    values.Add(theFilter.Value);

                    if (theFilter.Operator == GridFilterOperator.In || theFilter.Operator == GridFilterOperator.NotIn)
                    {
                        value = $"{theFilter.Field}";
                        field = $"@{index}";
                    }

                    var theOperator = Mappings.OperatorMapping[theFilter.Operator];

                    var theExpression = $"{field}{theOperator}{valuePrefix}{value}{valuePostfix}";

                    var expressionPrefix = Mappings.ExpressionPrefix.Keys.Contains(theFilter.Operator) ? Mappings.ExpressionPrefix[theFilter.Operator] : "";
                    var expressionPostfix = Mappings.ExpressionPostfix.Keys.Contains(theFilter.Operator) ? Mappings.ExpressionPostfix[theFilter.Operator] : "";

                    theExpression = $"{expressionPrefix}{theExpression}{expressionPostfix}";

                    expressions.Add(theExpression);

                    index++;
                }

                var expression = string.Join(" || ", expressions);

                select = select.Where(expression, values.ToArray());
            }

            //IQueryable newSelect = new ColumnRemover(this.Select)
            //    .RemoveColumns(
            //        this.Columns
            //        .Where(x => !x.Visible)
            //        .Select(x => x.Field).ToList()
            //    );

            //if (this.Columns != null && this.Columns.Count > 0)
            //{
            //    //var names = new AndAlsoModifier().GetSelectedProperties(select.Expression);

            //    var propertiesToSelect  = select.ElementType.GetProperties().Select(x => x.Name);

            //    //var propertiesToSelect = new AndAlsoModifier().GetSelectedProperties(select.Expression);

            //    System.Diagnostics.Debug.WriteLine("Cols Are: " + string.Join(", ", propertiesToSelect));

            //    if (this.Columns != null)
            //        propertiesToSelect = propertiesToSelect.Where(x => !this.Columns.Any(y => y.Field.ToUpper() == x.ToUpper() && !y.Visible)).ToList();

            //    var fields = string.Join(", ", propertiesToSelect);

            //    //var fields = string.Join(", ", names);
            //    //var fields = string.Join(", ", this.TypeColumns.Select(x => x.Field));
            //    newSelect = select.Select($"new ({fields})");
            //}
            //else
            //newSelect = select;

            this.ProccessedSelect = select;
        }

        private object parseJsonElement(System.Text.Json.JsonElement item, Type dataType)
        {
            object parsedValue = null;

            try
            {
                if (dataType == typeof(DateTime) || dataType == typeof(DateTime?))
                    parsedValue = item.GetDateTime();
                else if (dataType == typeof(bool) || dataType == typeof(bool?))
                    parsedValue = item.GetBoolean();
                else if (dataType == typeof(decimal) || dataType == typeof(decimal?))
                    parsedValue = item.GetDecimal();
                else if (dataType == typeof(DateTimeOffset) || dataType == typeof(DateTimeOffset?))
                    parsedValue = item.GetDateTimeOffset();
                else if (dataType == typeof(double) || dataType == typeof(double?))
                    parsedValue = item.GetDouble();
                else if (dataType == typeof(Guid) || dataType == typeof(Guid?))
                    parsedValue = item.GetGuid();
                else if (dataType == typeof(short) || dataType == typeof(short?))
                    parsedValue = item.GetInt16();
                else if (dataType == typeof(int) || dataType == typeof(int?))
                    parsedValue = item.GetInt32();
                else if (dataType == typeof(long) || dataType == typeof(long?))
                    parsedValue = item.GetInt64();
                else if (dataType == typeof(string))
                    parsedValue = item.GetString();
            }
            catch
            {

            }

            return parsedValue;
        }

        private IQueryable GetPaginatedQuery()
        {
            var sorts = new List<GridSort>();

            sorts.AddRange(this.Sort);

            if (sorts.Count == 0)
                sorts.Add(this.StableSort);

            IQueryable sort = this.ProccessedSelect
                .OrderBy(string.Join(", ", sorts.Select(x => $"{x.Field} {(x.SortDirection == SortDirection.Descending ? "desc" : "")}")))
                .ThenBy($"{this.StableSort.Field} {(this.StableSort.SortDirection == SortDirection.Descending ? "desc" : "")}");

            IQueryable dataToIterate;

            if (this.DataPageSize != -1 && this.DataCount > DataPageSize)
                dataToIterate = sort.Skip((DataPageIndex * DataPageSize)).Take(DataPageSize);
            else
                dataToIterate = sort;

            return dataToIterate;
        }
        private void GenerateColumns()
        {
            var dataTypeColumns = new List<GridColumn>();

            var props = typeof(T).GetProperties().Where(x => x.MemberType == System.Reflection.MemberTypes.Property).ToList();

            //throw new Exception(props.Count.ToString());

            //if (props.Count == 0 && this.Data.Count > 0)
            //    props = this.Data.FirstOrDefault().GetType().GetProperties().Where(x => x.MemberType == System.Reflection.MemberTypes.Property).ToList();

            foreach (var col in props)
            {
                var customAttribute = (GridColumnAttribute)col.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(GridColumnAttribute));

                var gridColumn = new GridColumn
                {
                    Field = col.Name,
                    Order = customAttribute == null || customAttribute.Order == int.MinValue ? new Nullable<int>() : customAttribute.Order,
                    HeaderText = customAttribute == null ? col.Name : customAttribute.HeaderText,
                    Visible = true,
                };

                while (gridColumn.Order != null && this.Columns.Any(x => x.Order == gridColumn.Order))
                    gridColumn.Order++;

                dataTypeColumns.Add(gridColumn);
            }

            dataTypeColumns.ForEach((dataTypeColumn) =>
            {
                var payloadColumn = this.Columns.FirstOrDefault(y => y.Field == dataTypeColumn.Field);

                if (payloadColumn != null)
                {
                    //This will overwrite the Attribute Order if confilicts
                    if (payloadColumn.Order != null)
                        dataTypeColumn.Order = payloadColumn.Order;

                    dataTypeColumn.Visible = payloadColumn.Visible;
                }
            });

            var autoOrder = 0;

            dataTypeColumns.ForEach((dataTypeColumn) =>
            {
                if (dataTypeColumn.Order == null)
                {
                    while (dataTypeColumns.Any(x => x.Order == autoOrder))
                        autoOrder++;

                    dataTypeColumn.Order = autoOrder;

                    autoOrder++;
                }
            });

            this.Columns = dataTypeColumns.OrderBy(x => x.Order).ToList();
        }
        private void LoadAggregate()
        {
            if (this.ExportMode)
                return;

            if (this.AggregateSelect != null)
            {
                var aggregate = GetGroupedAggregate().ToDynamicList().Cast<T2>().FirstOrDefault();

                this.Aggregate = aggregate;
            }
        }
        private async Task LoadAggregateAsync()
        {
            if (this.ExportMode)
                return;

            if (this.AggregateSelect != null)
            {
                var aggregate = (await GetGroupedAggregate().ToDynamicListAsync()).Cast<T2>().FirstOrDefault();

                this.Aggregate = aggregate;
            }
        }
       
        private IQueryable<T2> GetGroupedAggregate()
        {
            return this.ProccessedSelect.GroupBy(x => Math.Abs(1)).Select(this.AggregateSelect);
        }

        private void ProcessPagination()
        {
            if (this.ExportMode)
                return;

            if (!this.ExportMode)
            {
                if (this.Aggregate == null || !this.Aggregate.GetType().GetProperties().Any(x => x.Name == "Count"))
                    this.DataCount = this.ProccessedSelect.Count();
            }

            if (this.DataCount < 0)
                this.DataCount = (int)this.Aggregate.GetType().GetProperties().FirstOrDefault(x => x.Name == "Count")?.GetValue(this.Aggregate);

            //Show All
            if (this.DataPageSize == -1)
            {
                this.Pagination.Count = 1;
                this.DataPageIndex = 0;
                this.Pagination.PageSize = 1;
            }
            else
            {
                this.Pagination.Count = DataCount / this.DataPageSize + (DataCount % this.DataPageSize > 0 ? 1 : 0);
            }

            this.Pagination.LastPageIndex = this.Pagination.Count - 1;

            if (this.DataPageIndex > this.Pagination.LastPageIndex)
                this.DataPageIndex = this.Pagination.LastPageIndex;
            
            this.Pagination.PageIndex = this.DataPageIndex % this.Pagination.PageSize;

            this.Pagination.PageStart = (this.DataPageIndex / this.Pagination.PageSize) * this.Pagination.PageSize;
            this.Pagination.PageEnd = this.Pagination.PageStart + (this.Pagination.PageSize - 1);

            if (this.Pagination.Count == 0)
                this.Pagination.LastPageIndex = 0;

            if (this.Pagination.PageEnd > this.Pagination.LastPageIndex)
                this.Pagination.PageEnd = this.Pagination.LastPageIndex;

            this.Pagination.HasPreviousPage = this.Pagination.PageSize > 1 && this.Pagination.PageStart > 0;
            this.Pagination.HasNextPage = this.Pagination.LastPageIndex > this.Pagination.PageEnd;

            this.Pagination.DataStart = (this.DataPageIndex * this.DataPageSize) + 1;

            if (DataCount == 0)
                this.Pagination.DataStart = 0;

            this.Pagination.DataEnd = this.Pagination.DataStart + this.DataPageSize - 1;

            if (this.Pagination.DataEnd > DataCount)
                this.Pagination.DataEnd = DataCount;

            if (this.DataPageSize == -1)
                this.Pagination.DataEnd = DataCount;
        }
        private FileHelpers.FileHelperEngine GetCSVEngine()
        {
            if (!this.ExportMode)
                throw new Exception("Export Mode is not Active. ExportConfig.Export must be marked as True.");

            var engine = new FileHelpers.DelimitedFileEngine(typeof(T), System.Text.Encoding.UTF8);

            var excludedFields = this.Columns.Where(y => !y.Visible);

            if (this.ExportConfig.Delimiter != null)
                engine.Options.Delimiter = this.ExportConfig.Delimiter;

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
            return this.Data.Cast<object>();
        }
        private GridConfig GridConfig { get; set; }

        #endregion
    }
}
