﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETFramework.Tests.Postgres
{
    [TestClass]
    public class Summary : ShiftGrid.Test.Shared.Tests.Summary
    {
        public Summary() : base(typeof(EF.PostgresDB), new Utils(typeof(EF.PostgresDB)))
        {

        }
    }
}