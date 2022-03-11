using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftSoftware.ShiftGrid.Core
{
    public class ExportConfig
    {
        public bool Export { get; set; }
        public string Delimiter { get; set; }
        public List<string> HiddenFields { get; set; }
    }
}
