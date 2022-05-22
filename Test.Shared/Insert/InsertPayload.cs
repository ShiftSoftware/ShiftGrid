namespace ShiftGrid.Test.Shared.Insert
{
    public class InsertPayload
    {
        public int DataCount { get; set; }
        public long? ParentTestItemId { get; set; }
        public DataTemplate DataTemplate { get; set; }
        public Increments Increments { get; set; }
    }
}
