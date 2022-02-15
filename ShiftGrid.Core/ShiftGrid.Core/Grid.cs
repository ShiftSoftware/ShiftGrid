using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ShiftGrid.Core
{
    public class Grid<T>
    {
        Dictionary<string, string> OperatorMapping = new Dictionary<string, string>
        {
            { GridFilterOperator.Equals, "=" },
            { GridFilterOperator.NotEquals, "!=" },
            { GridFilterOperator.GreaterThan, ">" },
            { GridFilterOperator.GreaterThanOrEquals, ">=" },
            { GridFilterOperator.LessThan, "<" },
            { GridFilterOperator.LessThanOrEquals, "<=" },
            { GridFilterOperator.Contains, ".Contains" },
            { GridFilterOperator.In, ".Contains" },
            { GridFilterOperator.StartsWith, ".StartsWith" },
            { GridFilterOperator.EndsWith, ".EndsWith" },
        };

        Dictionary<string, string> OperatorValuePrefix = new Dictionary<string, string>
        {
            { GridFilterOperator.Contains, "(" },
            { GridFilterOperator.In, "(" },
            { GridFilterOperator.StartsWith, "(" },
            { GridFilterOperator.EndsWith, "(" },
        };

        Dictionary<string, string> OperatorValuePostfix = new Dictionary<string, string>
        {
            { GridFilterOperator.Contains, ")" },
            { GridFilterOperator.In, ")" },
            { GridFilterOperator.StartsWith, ")" },
            { GridFilterOperator.EndsWith, ")" },
        };

        [JsonIgnore]
        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        public GridPagination Pagination { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        internal IQueryable<T> ShiftQL { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        internal bool ShiftQLInitialized { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        private System.Linq.Expressions.Expression<Func<IGrouping<int, T>, GridSummary>> SummarySelect { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        private IQueryable ProccessedSelect { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        private IQueryable<T> SummaryProcessedSelect { get; set; }

        public GridSummary Summary { get; set; }
        public int DataPageIndex { get; set; }
        public int DataPageSize { get; set; }

        private bool FromPayload { get; set; }

        public List<T> Data { get; set; }

        public ObservableCollection<GridFilter> Filters { get; set; }

        public ObservableCollection<GridSort> Sort { get; set; }

        public List<GridColumn> Columns { get; set; }

        private List<GridColumn> TypeColumns { get; set; }

        public Grid()
        {
            this.Filters = new ObservableCollection<GridFilter>();
            this.Sort = new ObservableCollection<GridSort>();

            this.Filters.CollectionChanged += Filters_CollectionChanged;
            this.Sort.CollectionChanged += Sort_CollectionChanged;
        }

        private void Sort_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ObservablesChanged("Sorting");
        }

        private void Filters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ObservablesChanged("Filters");
        }

        private void ObservablesChanged(string Type)
        {
            if (this.FromPayload)
                throw new Exception($"{Type} Can Not be Modified from Code. Because they're provided in the request payload.");
            else
            {
                if (this.ShiftQLInitialized)
                    throw new Exception($"{Type} can not be Modified after {nameof(Extensions.ToShiftGrid)} is invoked.");
            }
        }

        private void GenerateQuery()
        {
            var select = this.ShiftQL.Where("1=1");

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
                    var valuePrefix = OperatorValuePrefix.Keys.Contains(theFilter.Operator) ? OperatorValuePrefix[theFilter.Operator] : "";
                    var valuePostfix = OperatorValuePostfix.Keys.Contains(theFilter.Operator) ? OperatorValuePostfix[theFilter.Operator] : "";

                    var value = $"@{index}";
                    var field = theFilter.Field;

                    if (theFilter.Operator == GridFilterOperator.In)
                    {
                        value = $"{theFilter.Field}";
                        field = $"@{index}";

                        var dataType = (typeof(T)).GetProperties().First(x => x.Name == theFilter.Field).PropertyType;
                        Type listType = typeof(List<>).MakeGenericType(dataType);

                        //Provided by Client from a Jobject
                        if (theFilter.Value.GetType() == typeof(Newtonsoft.Json.Linq.JArray))
                        {
                            var valueList = (IList)Activator.CreateInstance(listType);

                            foreach (var item in (theFilter.Value as Newtonsoft.Json.Linq.JArray).Select(x => x.ToObject(dataType)))
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

                    var theOperator = OperatorMapping[theFilter.Operator];

                    var theExpression = $"{field}{theOperator}{valuePrefix}{value}{valuePostfix}";

                    expressions.Add(theExpression);

                    index++;
                }

                var expression = string.Join(" || ", expressions);

                //throw new Exception(expression);

                select = select.Where(expression, values.ToArray());
            }

            this.SummaryProcessedSelect = select;

            IQueryable newSelect;

            if (this.TypeColumns.Count != this.Columns.Count)
            {
                var fields = string.Join(", ", this.Columns.Select(x => x.Field));
                newSelect = select.Select($"new ({fields})");
            }
            else
                newSelect = select;

            this.ProccessedSelect = newSelect;
        }

        private void LoadSummary()
        {
            if (this.SummarySelect != null)
            {
                var summarySelect = this.SummaryProcessedSelect.GroupBy(x => 0).Select(this.SummarySelect);
                this.Summary = summarySelect.FirstOrDefault();
            }

            if (this.Summary == null)
                this.Summary = new GenericGridSummary { };

            this.Summary.DataCount = this.ProccessedSelect.Count();
        }

        private void GenerateColumns()
        {
            var columns = new List<GridColumn>();

            var props = typeof(T).GetProperties();

            var jsonResolver = new PropertyRenameAndIgnoreSerializerContractResolver();

            var order = 0;

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

            foreach (var ignoredCol in this.TypeColumns.Where(x => !this.Columns.Any(y => y.Field == x.Field)))
                jsonResolver.IgnoreProperty(typeof(T), ignoredCol.Field);

            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = jsonResolver;

            this.JsonSerializerSettings = serializerSettings;
        }

        private void LoadData()
        {
            IQueryable sort = this.ProccessedSelect.OrderBy(string.Join(", ", this.Sort.Select(x => $"{x.Field} {(x.SortDirection == SortDirection.Descending ? "desc" : "")}")));

            this.Data = new List<T>();

            IQueryable dataToIterate;

            if (this.DataPageSize != -1)
                dataToIterate = sort.Skip((DataPageIndex * DataPageSize)).Take(DataPageSize);
            else
                dataToIterate = sort;

            foreach (var item in dataToIterate)
                this.Data.Add(item.ToType<T>());
            //this.Data.Add((T) item);
        }

        private void ProcessPagination()
        {
            //if (this.Summary == null)
            //    this.TotalDataCount = this.ProccessedSelect.Count();
            //else

            //Summary is Assigned on LoadSummar(). It will only be null if a summary SQL is provided and no data is returned
            //if (this.Summary == null)
            //    this.TotalDataCount = 0;
            //else
            
            if (this.Pagination == null)
            {
                this.Pagination = new GridPagination();
                this.Pagination.PageSize = 10;
            }

            //Show All
            if (this.DataPageSize == -1)
            {
                this.Pagination.Count = 1;
                this.DataPageIndex = 0;
                this.Pagination.PageSize = 1;
            }
            else
            {
                this.Pagination.Count = this.Summary.DataCount / this.DataPageSize + (this.Summary.DataCount % this.DataPageSize > 0 ? 1 : 0);
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

            if (this.Summary.DataCount == 0)
                this.Pagination.DataStart = 0;

            this.Pagination.DataEnd = this.Pagination.DataStart + this.DataPageSize - 1;

            if (this.Pagination.DataEnd > this.Summary.DataCount)
                this.Pagination.DataEnd = this.Summary.DataCount;

            if (this.DataPageSize == -1)
                this.Pagination.DataEnd = this.Summary.DataCount;
        }

        public void Init()
        {
            if (!this.ShiftQLInitialized)
                throw new Exception($"{nameof(Init)} should be called after {nameof(Extensions.ToShiftGrid)} is invoked.");

            if (this.Sort == null || this.Sort.Count == 0)
                throw new Exception("Sorting is not specified. At least one item is required in DataGrid.Sort");

            this.GenerateColumns();
            this.GenerateQuery();
            this.LoadData();
            this.LoadSummary();
            this.ProcessPagination();
        }

        public Grid<T> SetSummary(System.Linq.Expressions.Expression<Func<IGrouping<int, T>, GridSummary>> summarySelect)
        {
            this.SummarySelect = summarySelect;

            return this;
        }

        public Grid<T> AddFilter(GridFilter filter)
        {
            this.Filters.Add(filter);
            return this;
        }

        public static bool InitFromPayload(JToken payload, ref Grid<T> grid)
        {
            if (payload != null)
            {
                if (payload.ToObject<object>() != null && payload["Summary"] != null)
                    payload["Summary"] = null;

                grid = payload.ToObject<Grid<T>>();

                if (grid == null)
                    return false;

                grid.FromPayload = true;

                return true;
            }

            return false;
        }

        public Grid<T> SortBy(GridSort sort)
        {
            PreventInconsistency();
            this.Sort.Add(sort);
            return this;
        }

        public Grid<T> FilterBy(GridFilter filter)
        {
            PreventInconsistency();
            this.Filters.Add(filter);
            return this;
        }

        private void PreventInconsistency()
        {
            if (this.FromPayload)
            {

            }
        }

        public string ToCSV()
        {
            var stream = new MemoryStream();

            var engine = new FileHelpers.FileHelperEngine(typeof(T));

            var excludedFields = this.TypeColumns.Where(x => !this.Columns.Any(y => y.Field == x.Field));

            foreach (var excluded in excludedFields)
                engine.Options.RemoveField(excluded.Field);

            engine.HeaderText = engine.GetFileHeader();

            return engine.WriteString(this.Data.Cast<object>());
        }
    }
}
