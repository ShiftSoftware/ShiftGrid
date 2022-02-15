using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ShiftGrid.Core;

namespace ShiftGrid.Test.NET.Models
{
    public class TestItem
    {
        [GridColumn(Field = "ID", HeaderText = "Identification", Order = 1)]
        public long ID { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Price { get; set; }
        public long? TypeId { get; set; }
        public long? ParentTestItemId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("ParentTestItemId")]
        public virtual TestItem ParentTestItem { get; set; }

        [ForeignKey("TypeId")]
        public virtual Type Type { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [InverseProperty("ParentTestItem")]
        public virtual ICollection<TestItem> ChildTestItems { get; set; }
    }
}