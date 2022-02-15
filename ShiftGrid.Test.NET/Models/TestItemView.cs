using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShiftGrid.Core;

namespace ShiftGrid.Test.NET.Models
{
    public class TestItemView
    {
        [GridColumn(Field = "ID", HeaderText = "My Id")]
        public long ID { get; set; }
        public string Title { get; set; }
        public long? TypeId { get; set; }
        public string Type { get; set; }
    }
}