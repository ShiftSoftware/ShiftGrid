using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ShiftSoftware.ShiftGrid.Core
{
    public static class Extensions
    {
        public static Grid<T, object> ToShiftGrid<T>(this IQueryable<T> select, string stableSortField, SortDirection stableSortDirection = SortDirection.Ascending, GridConfig gridConfig = null)
        {
            return new Grid<T, object>
            {
                StableSort = new GridSort { Field = stableSortField, SortDirection = stableSortDirection },
                Select = select
            }.Init(gridConfig);
        }

        public static async Task<Grid<T, object>> ToShiftGridAsync<T>(this IQueryable<T> select, string stableSortField, SortDirection stableSortDirection = SortDirection.Ascending, GridConfig gridConfig = null)
        {
            return await new Grid<T, object>
            {
                StableSort = new GridSort { Field = stableSortField, SortDirection = stableSortDirection },
                Select = select
            }.InitAsync(gridConfig);
        }

        public static Grid<T, T2> ToShiftGrid<T, T2>(this SelectAndAggregate<T, T2> selectAndAggregate, string stableSortField, SortDirection stableSortDirection = SortDirection.Ascending, GridConfig gridConfig = null)
        {
            return new Grid<T, T2>()
            {
                StableSort = new GridSort { Field = stableSortField, SortDirection = stableSortDirection },
                Select = selectAndAggregate.Select,
                AggregateSelect = selectAndAggregate.AggregateSelect,
            }.Init(gridConfig);
        }
        public static async Task<Grid<T, T2>> ToShiftGridAsync<T, T2>(this SelectAndAggregate<T, T2> selectAndAggregate, string stableSortField, SortDirection stableSortDirection = SortDirection.Ascending, GridConfig gridConfig = null)
        {
            return await new Grid<T, T2>()
            {
                StableSort = new GridSort { Field = stableSortField, SortDirection = stableSortDirection },
                Select = selectAndAggregate.Select,
                AggregateSelect = selectAndAggregate.AggregateSelect,
            }.InitAsync(gridConfig);
        }

        public class SelectAndAggregate<T, T2>
        {
            public IQueryable<T> Select { get; set; }
            public Expression<Func<IGrouping<int, T>, T2>> AggregateSelect { get; set; }
        }

        public class SelectAndAggregateSelectCombo<T, T2>
        {
            public IQueryable<T> Select { get; set; }
            public Expression<Func<IGrouping<int, T>, T2>> AggregateSelect { get; set; }
        }

        public static SelectAndAggregate<T, T2> SelectAggregate<T, T2>(this IQueryable<T> select, Expression<Func<IGrouping<int, T>, T2>> aggregateSelect)
        {
            return new SelectAndAggregate<T, T2>
            {
                Select = select,
                AggregateSelect = aggregateSelect
            };
        }
    }
}
