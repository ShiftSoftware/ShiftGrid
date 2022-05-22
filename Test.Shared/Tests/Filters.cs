using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using ShiftGrid.Test.Shared.Models;
using ShiftSoftware.ShiftGrid.Core;
using System.Collections.Generic;

namespace ShiftGrid.Test.Shared.Tests
{
    public class Filters
    {
        public System.Type DBType { get; set; }
        public Shared.Utils Utils { get; set; }
        public Filters(System.Type type, Utils utils)
        {
            this.DBType = type;
            this.Utils = utils;
        }

        [TestMethod]
        public async Task Filters_Equals()
        {
            await this.Utils.PopulateTestData(100);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
            new GridConfig
            {
                Filters = new List<GridFilter> {
                   new GridFilter
                   {
                       Field = nameof(TestItem.ID),
                       Operator = GridFilterOperator.Equals,
                       Value = 1
                   }
               }
            });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Data.Count() == 1 &&
                shiftGrid.Data.ElementAt(0).Title == "Title - 1"
            );
        }

        [TestMethod]
        public async Task Filters_In()
        {
            await this.Utils.PopulateTestData(100);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
            new GridConfig
            {
                Filters = new List<GridFilter> {
                   new GridFilter
                   {
                       Field = nameof(TestItem.ID),
                       Operator = GridFilterOperator.In,
                       Value = new List<long> { 1, 4, 10 }
                   }
               }
            });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Data.Count == 3 &&
                shiftGrid.Data.ElementAt(0).Title == "Title - 1" &&
                shiftGrid.Data.ElementAt(1).Title == "Title - 4" &&
                shiftGrid.Data.ElementAt(2).Title == "Title - 10"
            );
        }

        [TestMethod]
        public async Task Filters_NotIn()
        {
            await this.Utils.PopulateTestData(100);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
            new GridConfig
            {
                Filters = new List<GridFilter> {
                   new GridFilter
                   {
                       Field = nameof(TestItem.ID),
                       Operator = GridFilterOperator.NotIn,
                       Value = new List<long> { 1, 2, 3 }
                   }
               }
            });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.DataCount == 97 &&
                shiftGrid.Data.ElementAt(0).Title == "Title - 4" &&
                shiftGrid.Data.ElementAt(1).Title == "Title - 5" &&
                shiftGrid.Data.ElementAt(2).Title == "Title - 6"
            );
        }

        [TestMethod]
        public async Task Filters_In_NoModel()
        {
            await this.Utils.PopulateTestData(100);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems.ToShiftGrid(nameof(TestItem.ID), SortDirection.Ascending,
            new GridConfig
            {
                Filters = new List<GridFilter> {
                   new GridFilter
                   {
                       Field = nameof(TestItem.ID),
                       Operator = GridFilterOperator.In,
                       Value = new List<long> { 15, 25, 35 }
                   }
               }
            });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Data.Count == 3 &&
                shiftGrid.Data.ElementAt(0).Title == "Title - 15" &&
                shiftGrid.Data.ElementAt(1).Title == "Title - 25" &&
                shiftGrid.Data.ElementAt(2).Title == "Title - 35"
            );
        }

        [TestMethod]
        public async Task Filters_StartsWith()
        {
            await this.Utils.PopulateTestData(100);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
            new GridConfig
            {
                Filters = new List<GridFilter> {
                   new GridFilter
                   {
                       Field = nameof(TestItem.Title),
                       Operator = GridFilterOperator.StartsWith,
                       Value = "Title - 1"
                   }
               }
            });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Data.Count() == 12 &&
                shiftGrid.Data.ElementAt(0).Title == "Title - 1" &&
                shiftGrid.Data.ElementAt(1).Title == "Title - 10" &&
                shiftGrid.Data.ElementAt(2).Title == "Title - 11" &&
                shiftGrid.Data.ElementAt(3).Title == "Title - 12" &&
                shiftGrid.Data.ElementAt(4).Title == "Title - 13" &&
                shiftGrid.Data.ElementAt(5).Title == "Title - 14" &&
                shiftGrid.Data.ElementAt(6).Title == "Title - 15" &&
                shiftGrid.Data.ElementAt(7).Title == "Title - 16" &&
                shiftGrid.Data.ElementAt(8).Title == "Title - 17" &&
                shiftGrid.Data.ElementAt(9).Title == "Title - 18" &&
                shiftGrid.Data.ElementAt(10).Title == "Title - 19" &&
                shiftGrid.Data.ElementAt(11).Title == "Title - 100"
            );
        }

        [TestMethod]
        public async Task Filters_EndsWith()
        {
            await this.Utils.PopulateTestData(100);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
            new GridConfig
            {
                Filters = new List<GridFilter> {
                   new GridFilter
                   {
                       Field = nameof(TestItem.Title),
                       Operator = GridFilterOperator.EndsWith,
                       Value = "1"
                   }
               }
            });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Data.Count() == 10 &&
                shiftGrid.Data.ElementAt(0).Title == "Title - 1" &&
                shiftGrid.Data.ElementAt(1).Title == "Title - 11" &&
                shiftGrid.Data.ElementAt(2).Title == "Title - 21" &&
                shiftGrid.Data.ElementAt(3).Title == "Title - 31" &&
                shiftGrid.Data.ElementAt(4).Title == "Title - 41" &&
                shiftGrid.Data.ElementAt(5).Title == "Title - 51" &&
                shiftGrid.Data.ElementAt(6).Title == "Title - 61" &&
                shiftGrid.Data.ElementAt(7).Title == "Title - 71" &&
                shiftGrid.Data.ElementAt(8).Title == "Title - 81" &&
                shiftGrid.Data.ElementAt(9).Title == "Title - 91"
            );
        }

        [TestMethod]
        public async Task Filters_Or_Equals()
        {
            await this.Utils.PopulateTestData(100);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
            new GridConfig
            {
                Filters = new List<GridFilter> {
                   new GridFilter
                   {
                       Field = nameof(TestItem.Title),
                       Operator = GridFilterOperator.Equals,
                       Value = "Title - 1",
                       OR = new List<GridFilter> {
                           new GridFilter
                           {
                               Field = nameof(TestItem.Title),
                               Operator = GridFilterOperator.Equals,
                               Value = "Title - 21"
                           },
                           new GridFilter
                           {
                               Field = nameof(TestItem.Title),
                               Operator = GridFilterOperator.Equals,
                               Value = "Title - 31"
                           }
                       }
                   }
               }
            });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Data.Count() == 3 &&
                shiftGrid.Data.ElementAt(0).Title == "Title - 1" &&
                shiftGrid.Data.ElementAt(1).Title == "Title - 21" &&
                shiftGrid.Data.ElementAt(2).Title == "Title - 31"
            );
        }

        [TestMethod]
        public async Task Filters_Or_InAndEquals()
        {
            await this.Utils.PopulateTestData(100);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
            new GridConfig
            {
                Filters = new List<GridFilter>{
                   new GridFilter
                   {
                       Field = nameof(TestItem.ID),
                       Operator = GridFilterOperator.In,
                       Value = new List<long> { 33, 44, 55, 66 },
                       OR = new List<GridFilter> {
                           new GridFilter
                           {
                               Field = nameof(TestItem.Title),
                               Operator = GridFilterOperator.Equals,
                               Value = "Title - 14"
                           },
                           new GridFilter
                           {
                               Field = nameof(TestItem.Title),
                               Operator = GridFilterOperator.Equals,
                               Value = "Title - 19"
                           }
                       }
                   }
               }
            });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Data.Count() == 6 &&
                shiftGrid.Data.ElementAt(0).Title == "Title - 14" &&
                shiftGrid.Data.ElementAt(1).Title == "Title - 19" &&
                shiftGrid.Data.ElementAt(2).Title == "Title - 33" &&
                shiftGrid.Data.ElementAt(3).Title == "Title - 44" &&
                shiftGrid.Data.ElementAt(4).Title == "Title - 55" &&
                shiftGrid.Data.ElementAt(5).Title == "Title - 66"
            );
        }

        [TestMethod]
        public async Task Filters_SubItem()
        {
            await this.Utils.PopulateTestData(100, 10);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title,
                Items = x.ChildTestItems.Select(y => new SubTestItemView
                {
                    Title = y.Title,
                })
            })
            .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
            new GridConfig
            {
                Filters = new List<GridFilter> {
                   new GridFilter
                   {
                       Field = "Items.FirstOrDefault().Title",
                       Operator = GridFilterOperator.Equals,
                       Value = "Sub Title - 1"
                   }
               }
            });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Data.Count() == 1 &&
                shiftGrid.Data.ElementAt(0).Title == "Title - 1"
            );
        }

        [TestMethod]
        public async Task Filters_OrAndSubItem()
        {
            await this.Utils.PopulateTestData(100, 10);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title,
                Items = x.ChildTestItems.Select(y => new SubTestItemView
                {
                    Title = y.Title,
                })
            })
            .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
            new GridConfig
            {
                Filters = new List<GridFilter> {
                   new GridFilter
                   {
                       Field = "Items.FirstOrDefault().Title",
                       Operator = GridFilterOperator.Equals,
                       Value = "Sub Title - 1",
                       OR = new List<GridFilter> {
                           new GridFilter {
                               Field = nameof(TestItem.ID),
                               Operator = GridFilterOperator.Equals,
                               Value = "5"
                           }
                       }
                   }
               }
            });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Data.Count() == 2 &&
                shiftGrid.Data.ElementAt(0).Title == "Title - 1" &&
                shiftGrid.Data.ElementAt(1).Title == "Title - 5"
            );
        }

        [TestMethod]
        public async Task Filters_NavigatedItems_SubItemAggregates()
        {
            await this.Utils.PopulateTestData(100, 5);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems.Select(x => new
            {
                ID = x.ID,
                Title = x.Title,
                Type = x.Type.Name,
                Items = x.ChildTestItems.Select(y => new
                {
                    IncreasedPrice = y.Price * 100,
                })
            })
            .ToShiftGrid("ID", SortDirection.Ascending,
            new GridConfig
            {
                Filters = new List<GridFilter> {
                   new GridFilter
                   {
                       //Field = "Items.FirstOrDefault().IncreasedPrice", //This statement fails on MySQL. Though It works if we use IncreasedPrice = y.Price. Without the multiplication
                       Field = "Items.Min(IncreasedPrice)",
                       Operator = GridFilterOperator.Equals,
                       Value = 1000m
                   },
                   new GridFilter
                   {
                       Field = "Type",
                       Operator = GridFilterOperator.Equals,
                       Value = "Type - 1"
                   },
                   new GridFilter
                   {
                       Field = "Items.Sum(y=> y.IncreasedPrice)",
                       Operator = GridFilterOperator.Equals,
                       Value = 15000m
                   }
               },
            });


            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Data.Count() == 1 &&
                this.Utils.Logs.Count == 2
                //&& data.ElementAt(0).Title == "Title - 1"
            );
        }

        [TestMethod]
        public async Task Filters_NavigatedItems_SubItemAggregates_NoModel()
        {
            await this.Utils.PopulateTestData(100, 5);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
            .ToShiftGrid(nameof(TestItem.ID), SortDirection.Ascending,
            new GridConfig
            {
                Filters = new List<GridFilter> {
                   new GridFilter
                   {
                       Field = "ChildTestItems.FirstOrDefault().Price",
                       Operator = GridFilterOperator.Equals,
                       Value = 10m
                   },
                   new GridFilter
                   {
                       Field = "Type.Name",
                       Operator = GridFilterOperator.Equals,
                       Value = "Type - 1"
                   },
                   new GridFilter
                   {
                       Field = "ChildTestItems.Sum(y=> y.Price)",
                       Operator = GridFilterOperator.Equals,
                       Value = 150m
                   }
               },
            });


            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Data.Count() == 1
            //&& data.ElementAt(0).Title == "Title - 1"
            );
        }
    }
}