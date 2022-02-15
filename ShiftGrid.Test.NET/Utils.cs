using ShiftGrid.Test.NET.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShiftGrid.Test.NET
{
    public class Utils
    {
        public static DB GetDBContext()
        {
            return new DB();
        }
    }
}