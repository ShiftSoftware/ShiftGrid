using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShiftGrid.Test.NET
{
    public class InsertPayload
    {
        public int DataCount { get; set; }
        public long? ParentTestItemId { get; set; }
        public DataTemplate DataTemplate { get; set; }
        public Increments Increments { get; set; }
    }

    public class DataTemplate
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public decimal? Price { get; set; }
        public long? ParentTestItemId { get; set; }
    }

    public class Increments
    {
        public int Day { get; set; }
        public int Price { get; set; }
    }
}