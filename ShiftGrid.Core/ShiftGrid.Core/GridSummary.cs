namespace ShiftGrid.Core
{
    public interface GridSummary
    {
        int DataCount { get; set; }
    }

    public class GenericGridSummary : GridSummary
    {
        public int DataCount { get; set; }
    }
}
