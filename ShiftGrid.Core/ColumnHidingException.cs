using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftSoftware.ShiftGrid.Core
{
    public class ColumnHidingException : Exception
    {
        public ColumnHidingException()
        {
            
        }

        public ColumnHidingException(string message) : base(message) { }
    }
}
