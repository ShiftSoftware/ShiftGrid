using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ShiftGrid.Test.NET.EF
{
    public class DB : DBBase
    {
        public DB() : base("name=ShiftGrid_SQLServer")
        {

        }
    }
}