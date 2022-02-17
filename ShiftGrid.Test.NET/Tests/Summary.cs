using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using ShiftGrid.Test.NET.Models;
using ShiftSoftware.ShiftGrid.Core;
using System.Collections.Generic;

namespace ShiftGrid.Test.NET.Tests
{
    [TestClass]
    public class Summary
    {
        [TestMethod]
        public async Task Basic()
        {
            await Utils.DataInserter(100);

            var db = Utils.GetDBContext();

            var logs = new List<string>();

            var shiftGrid = db.TestItems
            .SelectSummary(x => new
            {
                Count = x.Count(),
                TotalPrice = x.Sum(y => y.Price)
            })
            .ToShiftGrid();

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                int.Parse(shiftGrid.Summary["Count"].ToString()) == 100 &&
                decimal.Parse(shiftGrid.Summary["TotalPrice"].ToString()) == 50500m
            );
        }

        [TestMethod]
        public async Task CalculatedFields()
        {
            await Utils.DataInserter(100);

            var db = Utils.GetDBContext();

            var logs = new List<string>();

            Controllers.DataController.SetupLogger(db, logs);

            var shiftGrid = db.TestItems
                .Select(x => new
                {
                    NewPrice = 10m * x.Price
                })
                .SelectSummary(x => new
                {
                    Count = x.Count(),
                    TotalPrice = x.Sum(y => y.NewPrice)
                })
                .ToShiftGrid();

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                int.Parse(shiftGrid.Summary["Count"].ToString()) == 100 &&
                decimal.Parse(shiftGrid.Summary["TotalPrice"].ToString()) == 505000m &&
                logs.Count == 2
            );
        }

        [TestMethod]
        public async Task CalculatedFields_FilteredWithResults()
        {
            await Utils.DataInserter(100);

            var db = Utils.GetDBContext();

            var logs = new List<string>();
            Controllers.DataController.SetupLogger(db, logs);

            var shiftGrid = db.TestItems
                .Select(x => new
                {
                    NewPrice = 10m * x.Price
                })
                .SelectSummary(x => new
                {
                    Count = x.Count(),
                    TotalPrice = x.Sum(y => y.NewPrice)
                })
                .ToShiftGrid(new GridConfig
                {
                    Filters = new List<GridFilter> {
                        new GridFilter {
                            Field = "NewPrice",
                            Operator = GridFilterOperator.GreaterThan,
                            Value = 8000m
                        },
                        new GridFilter {
                            Field = "NewPrice",
                            Operator = GridFilterOperator.LessThan,
                            Value = 9000m
                        }
                    }
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                int.Parse(shiftGrid.Summary["Count"].ToString()) == 9 &&
                decimal.Parse(shiftGrid.Summary["TotalPrice"].ToString()) == 76500m &&
                logs.Count == 2
            );
        }

        [TestMethod]
        public async Task CalculatedFields_FilteredWithNoResults_Nullable()
        {
            await Utils.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems
                .Select(x => new
                {
                    NewPrice = 10m * x.Price
                })
                .SelectSummary(x => new
                {
                    Count = x.Count(),
                    TotalPrice = x.Sum(y => y.NewPrice)
                })
                .ToShiftGrid(new GridConfig
                {
                    Filters = new List<GridFilter> {
                        new GridFilter {
                            Field = "NewPrice",
                            Operator = GridFilterOperator.GreaterThan,
                            Value = 80m
                        },
                        new GridFilter {
                            Field = "NewPrice",
                            Operator = GridFilterOperator.LessThan,
                            Value = 90m
                        }
                    }
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                int.Parse(shiftGrid.Summary["Count"].ToString()) == 0 &&
                shiftGrid.Summary["TotalPrice"] == null
            );
        }

        [TestMethod]
        public async Task CalculatedFields_FilteredWithNoResults_NotNullable()
        {
            await Utils.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems
                .Select(x => new
                {
                    NewPrice = 10m * x.Price
                })
                .SelectSummary(x => new
                {
                    Count = x.Count(),
                    TotalPrice = x.Sum(y => y.NewPrice.Value)
                })
                .ToShiftGrid(new GridConfig
                {
                    Filters = new List<GridFilter> {
                        new GridFilter {
                            Field = "NewPrice",
                            Operator = GridFilterOperator.GreaterThan,
                            Value = 80m
                        },
                        new GridFilter {
                            Field = "NewPrice",
                            Operator = GridFilterOperator.LessThan,
                            Value = 90m
                        }
                    }
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                int.Parse(shiftGrid.Summary["Count"].ToString()) == 0 &&
                decimal.Parse(shiftGrid.Summary["TotalPrice"].ToString()) == 0m
            );
        }

        [TestMethod]
        public async Task CalculatedFields_FilteredWithResults_WithoutCount()
        {
            await Utils.DataInserter(100);

            var db = Utils.GetDBContext();

            var logs = new List<string>();
            Controllers.DataController.SetupLogger(db, logs);

            var shiftGrid = db.TestItems
                .Select(x => new
                {
                    NewPrice = 10m * x.Price
                })
                .SelectSummary(x => new
                {
                    TotalPrice = x.Sum(y => y.NewPrice)
                })
                .ToShiftGrid(new GridConfig
                {
                    Filters = new List<GridFilter> {
                        new GridFilter {
                            Field = "NewPrice",
                            Operator = GridFilterOperator.GreaterThan,
                            Value = 8000m
                        },
                        new GridFilter {
                            Field = "NewPrice",
                            Operator = GridFilterOperator.LessThan,
                            Value = 9000m
                        }
                    }
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                int.Parse(shiftGrid.Summary["Count"].ToString()) == 9 &&
                decimal.Parse(shiftGrid.Summary["TotalPrice"].ToString()) == 76500m &&
                logs.Count == 3
            );
        }
    }
}