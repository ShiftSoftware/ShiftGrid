using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShiftSoftware.ShiftGrid.Core;

namespace ShiftGrid.Test.NET.Models
{
    [FileHelpers.DelimitedRecord(",")]
    public class TestItemView
    {
        [GridColumn(HeaderText = "My ID")]
        public long ID { get; set; }
        public decimal? CalculatedPrice { get; set; }
        public string Title { get; set; }
        public long? TypeId { get; set; }
        public string Type { get; set; }

        [FileHelpers.FieldHidden]
        public IEnumerable<SubTestItemView> Items { get; set; }
    }
}