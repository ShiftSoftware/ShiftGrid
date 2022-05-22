using System;
using System.Collections.Generic;

namespace ShiftGrid.Test.Shared.Models
{
    public class TestItem
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Price { get; set; }
        public long? TypeId { get; set; }
        public long? ParentTestItemId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual TestItem ParentTestItem { get; set; }
        public virtual Type Type { get; set; }
        public virtual ICollection<TestItem> ChildTestItems { get; set; }
    }
}