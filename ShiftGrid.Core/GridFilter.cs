using System.Collections.Generic;

namespace ShiftSoftware.ShiftGrid.Core
{
    public class GridFilter
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
        public List<GridFilter> OR { get; set; }
    }
}
