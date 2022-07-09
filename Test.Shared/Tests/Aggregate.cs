using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using ShiftGrid.Test.Shared.Models;
using ShiftSoftware.ShiftGrid.Core;
using System.Collections.Generic;

namespace ShiftGrid.Test.Shared.Tests
{
    public class Aggregate
    {
        public System.Type DBType { get; set; }
        public Shared.Utils Utils { get; set; }
        public Aggregate(System.Type type, Utils utils)
        {
            this.DBType = type;
            this.Utils = utils;
        }

        [TestMethod]
        public async Task Basic()
        {
            await this.Utils.PopulateTestData(100);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
            .SelectAggregate(x => new
            {
                Count = x.Count(),
                TotalPrice = x.Sum(y => y.Price)
            })
            .ToShiftGrid(nameof(TestItem.ID));

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Aggregate.Count == 100 &&
                shiftGrid.Aggregate.TotalPrice == 50500m
            );
        }

        [TestMethod]
        public async Task CalculatedFields()
        {
            await this.Utils.PopulateTestData(100);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new
                {
                    NewPrice = 10m * x.Price
                })
                .SelectAggregate(x => new
                {
                    Count = x.Count(),
                    TotalPrice = x.Sum(y => y.NewPrice)
                })
                .ToShiftGrid("NewPrice");

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Aggregate.Count == 100 &&
                shiftGrid.Aggregate.TotalPrice == 505000m &&
                this.Utils.Logs.Count == 2
            );
        }

        [TestMethod]
        public async Task CalculatedFields_FilteredWithResults()
        {
            await this.Utils.PopulateTestData(100);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new
                {
                    NewPrice = 10m * x.Price
                })
                .SelectAggregate(x => new
                {
                    Count = x.Count(),
                    TotalPrice = x.Sum(y => y.NewPrice)
                })
                .ToShiftGrid("NewPrice", SortDirection.Ascending, 
                new GridConfig
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
                shiftGrid.Aggregate.Count == 9 &&
                shiftGrid.Aggregate.TotalPrice == 76500m &&
                this.Utils.Logs.Count == 2
            );
        }

        [TestMethod]
        public async Task CalculatedFields_FilteredWithNoResults_Nullable()
        {
            await this.Utils.PopulateTestData(100);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new
                {
                    NewPrice = 10m * x.Price
                })
                .SelectAggregate(x => new
                {
                    Count = x.Count(),
                    TotalPrice = x.Sum(y => y.NewPrice)
                })
                .ToShiftGrid("NewPrice", SortDirection.Ascending,
                new GridConfig
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
                shiftGrid.Aggregate?.Count == null &&
                shiftGrid.Aggregate?.TotalPrice == null
            );
        }

        [TestMethod]
        public async Task CalculatedFields_FilteredWithNoResults_NotNullable()
        {
            await this.Utils.PopulateTestData(100);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new
                {
                    NewPrice = 10m * x.Price
                })
                .SelectAggregate(x => new
                {
                    Count = x.Count(),
                    TotalPrice = x.Sum(y => y.NewPrice.Value)
                })
                .ToShiftGrid("NewPrice", SortDirection.Ascending, 
                new GridConfig
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
                shiftGrid.Aggregate?.Count == null &&
                shiftGrid.Aggregate?.TotalPrice == null
            );
        }

        [TestMethod]
        public async Task CalculatedFields_FilteredWithResults_WithoutCount()
        {
            await this.Utils.PopulateTestData(100);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new
                {
                    NewPrice = 10m * x.Price
                })
                .SelectAggregate(x => new
                {
                    TotalPrice = x.Sum(y => y.NewPrice)
                })
                .ToShiftGrid("NewPrice", SortDirection.Ascending, 
                new GridConfig
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
                //shiftGrid.Aggregate.Count == 9 &&
                shiftGrid.Aggregate.TotalPrice == 76500m &&
                this.Utils.Logs.Count == 3
            );
        }
    }
}