using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftSoftware.ShiftGrid.Core
{
    public class AnonymousColumnHidingException : Exception
    {
        public AnonymousColumnHidingException()
        {
            
        }

        public AnonymousColumnHidingException(string message) : base(message) { }
    }
}
