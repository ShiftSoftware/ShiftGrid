namespace ShiftGrid.Core
{
    public class GridPagination
    {
        public int Count { get; set; }
        public int PageSize { get; set; }
        public int PageStart { get; set; }
        public int PageEnd { get; set; }
        public int PageIndex { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public int LastPageIndex { get; set; }
        public int DataStart { get; set; }
        public int DataEnd { get; set; }
    }
}
