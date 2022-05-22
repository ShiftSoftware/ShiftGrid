using System.Collections.Generic;
using ShiftSoftware.ShiftGrid.Core;

namespace ShiftGrid.Test.Shared.Models
{
    [FileHelpers.DelimitedRecord(",")]
    public class TestItemView
    {
        [GridColumn(HeaderText = "My ID")]
        public long ID { get; set; }
        [FileHelpers.FieldCaption("Calculated Price")]
        public decimal? CalculatedPrice { get; set; }
        [GridColumnAttribute(Order = 0)]
        public string Title { get; set; }

        [FileHelpers.FieldHidden]
        public long? TypeId { get; set; }
        [GridColumnAttribute(Order = 1)]
        public string Type { get; set; }

        [FileHelpers.FieldHidden]
        public IEnumerable<SubTestItemView> Items { get; set; }
    }
}