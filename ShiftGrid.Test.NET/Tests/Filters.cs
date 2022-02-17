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
    public class Filters
    {
        [TestMethod]
        public async Task Filters_Equals()
        {
            await Utils.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(new GridConfig
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

            var data = shiftGrid.Data.Select(x => (TestItemView)x);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                data.Count() == 1 &&
                data.ElementAt(0).Title == "Title - 1"
            );
        }

        [TestMethod]
        public async Task Filters_In()
        {
            await Utils.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(new GridConfig
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

            var data = shiftGrid.Data.Select(x => (TestItemView)x);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Data.Count == 3 &&
                data.ElementAt(0).Title == "Title - 1" &&
                data.ElementAt(1).Title == "Title - 4" &&
                data.ElementAt(2).Title == "Title - 10"
            );
        }

        [TestMethod]
        public async Task Filters_In_NoModel()
        {
            await Utils.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.ToShiftGrid(new GridConfig
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

            var objectProps = shiftGrid.Data.First().GetType().GetProperties().Where(x => x.MemberType == System.Reflection.MemberTypes.Property).ToList();

            var titleProp = objectProps.FirstOrDefault(x => x.Name == "Title");

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Data.Count == 3 &&
                titleProp.GetValue(shiftGrid.Data.ElementAt(0)).ToString() == "Title - 15" &&
                titleProp.GetValue(shiftGrid.Data.ElementAt(1)).ToString() == "Title - 25" &&
                titleProp.GetValue(shiftGrid.Data.ElementAt(2)).ToString() == "Title - 35"
            );
        }

        [TestMethod]
        public async Task Filters_StartsWith()
        {
            await Utils.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(new GridConfig
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

            var data = shiftGrid.Data.Select(x => (TestItemView)x);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                data.Count() == 12 &&
                data.ElementAt(0).Title == "Title - 1" &&
                data.ElementAt(1).Title == "Title - 10" &&
                data.ElementAt(2).Title == "Title - 11" &&
                data.ElementAt(3).Title == "Title - 12" &&
                data.ElementAt(4).Title == "Title - 13" &&
                data.ElementAt(5).Title == "Title - 14" &&
                data.ElementAt(6).Title == "Title - 15" &&
                data.ElementAt(7).Title == "Title - 16" &&
                data.ElementAt(8).Title == "Title - 17" &&
                data.ElementAt(9).Title == "Title - 18" &&
                data.ElementAt(10).Title == "Title - 19" &&
                data.ElementAt(11).Title == "Title - 100"
            );
        }

        [TestMethod]
        public async Task Filters_EndsWith()
        {
            await Utils.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(new GridConfig
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

            var data = shiftGrid.Data.Select(x => (TestItemView)x);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                data.Count() == 10 &&
                data.ElementAt(0).Title == "Title - 1" &&
                data.ElementAt(1).Title == "Title - 11" &&
                data.ElementAt(2).Title == "Title - 21" &&
                data.ElementAt(3).Title == "Title - 31" &&
                data.ElementAt(4).Title == "Title - 41" &&
                data.ElementAt(5).Title == "Title - 51" &&
                data.ElementAt(6).Title == "Title - 61" &&
                data.ElementAt(7).Title == "Title - 71" &&
                data.ElementAt(8).Title == "Title - 81" &&
                data.ElementAt(9).Title == "Title - 91"
            );
        }

        [TestMethod]
        public async Task Filters_Or_Equals()
        {
            await Utils.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(new GridConfig
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

            var data = shiftGrid.Data.Select(x => (TestItemView)x);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                data.Count() == 3 &&
                data.ElementAt(0).Title == "Title - 1" &&
                data.ElementAt(1).Title == "Title - 21" &&
                data.ElementAt(2).Title == "Title - 31"
            );
        }

        [TestMethod]
        public async Task Filters_Or_InAndEquals()
        {
            await Utils.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(new GridConfig
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

            var data = shiftGrid.Data.Select(x => (TestItemView)x);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                data.Count() == 6 &&
                data.ElementAt(0).Title == "Title - 14" &&
                data.ElementAt(1).Title == "Title - 19" &&
                data.ElementAt(2).Title == "Title - 33" &&
                data.ElementAt(3).Title == "Title - 44" &&
                data.ElementAt(4).Title == "Title - 55" &&
                data.ElementAt(5).Title == "Title - 66"
            );
        }

        [TestMethod]
        public async Task Filters_SubItem()
        {
            await Utils.DataInserter(100, 10);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title,
                Items = x.ChildTestItems.Select(y => new SubTestItemView
                {
                    Title = y.Title,
                })
            })
            .ToShiftGrid(new GridConfig
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

            var data = shiftGrid.Data.Select(x => (TestItemView)x);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                data.Count() == 1 &&
                data.ElementAt(0).Title == "Title - 1"
            );
        }

        [TestMethod]
        public async Task Filters_OrAndSubItem()
        {
            await Utils.DataInserter(100, 10);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title,
                Items = x.ChildTestItems.Select(y => new SubTestItemView
                {
                    Title = y.Title,
                })
            })
            .ToShiftGrid(new GridConfig
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

            var data = shiftGrid.Data.Select(x => (TestItemView)x);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                data.Count() == 2 &&
                data.ElementAt(0).Title == "Title - 1" &&
                data.ElementAt(1).Title == "Title - 5"
            );
        }

        [TestMethod]
        public async Task Filters_NavigatedItems_SubItemAggregates()
        {
            await Utils.DataInserter(100, 5);

            var db = Utils.GetDBContext();

            var logs = new List<string>();
            Controllers.DataController.SetupLogger(db, logs);

            var shiftGrid = db.TestItems.Select(x => new
            {
                ID = x.ID,
                Title = x.Title,
                Type = x.Type.Name,
                Items = x.ChildTestItems.Select(y => new
                {
                    IncreasedPrice = y.Price * 100,
                })
            })
            .ToShiftGrid(new GridConfig
            {
                Filters = new List<GridFilter> {
                   new GridFilter
                   {
                       Field = "Items.FirstOrDefault().IncreasedPrice",
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
                logs.Count == 2
                //&& data.ElementAt(0).Title == "Title - 1"
            );
        }

        [TestMethod]
        public async Task Filters_NavigatedItems_SubItemAggregates_NoModel()
        {
            await Utils.DataInserter(100, 5);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems
            .ToShiftGrid(new GridConfig
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