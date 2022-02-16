using System.Collections.Generic;

namespace ShiftSoftware.ShiftGrid.Core
{
    public class GridConfig
    {
        public PaginationConfig Pagination { get; set; }
        public int DataPageIndex { get; set; }
        public int DataPageSize { get; set; }
        public List<GridFilter> Filters { get; set; }
        public List<GridSort> Sort { get; set; }
        public List<GridColumn> Columns { get; set; }
    }
}
