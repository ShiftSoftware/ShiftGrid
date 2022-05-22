using System;

namespace ShiftGrid.Test.Shared.Insert
{
    public class DataTemplate
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public decimal? Price { get; set; }
        public long? ParentTestItemId { get; set; }
    }
}
