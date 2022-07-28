using ShiftSoftware.ShiftGrid.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShiftGrid.Test.Shared.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.Shared.Tests
{
    public class ColumnOrder
    {
        public System.Type DBType { get; set; }
        public Shared.Utils Utils { get; set; }

        public ColumnOrder(System.Type type, Utils utils)
        {
            this.DBType = type;
            this.Utils = utils;
        }

        [TestMethod]
        public async Task OrderByAttribute()
        {
            await this.Utils.PopulateTestData(100, 6);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new TestItemView
                {
                    ID = x.ID,
                    Title = x.Title,
                    CalculatedPrice = x.Price,
                    TypeId = x.TypeId
                })
                .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
                new GridConfig
                {
                    Columns = new List<GridColumn>
                    {

                    }
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid.Columns, Newtonsoft.Json.Formatting.Indented));

            var cols = shiftGrid.Columns.ToDictionary(x => x.Field, x => x);

            Assert.IsTrue(
                cols["ID"].Order == 2 &&
                cols["CalculatedPrice"].Order == 3 &&
                cols["Title"].Order == 0 &&
                cols["TypeId"].Order == 4 &&
                cols["Type"].Order == 1 &&
                cols["Items"].Order == 5
            );
        }

        [TestMethod]
        public async Task OverwriteAttributeByConfig()
        {
            await this.Utils.PopulateTestData(100, 6);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new TestItemView
                {
                    ID = x.ID,
                    Title = x.Title,
                    CalculatedPrice = x.Price,
                    TypeId = x.TypeId
                })
                .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
                new GridConfig
                {
                    Columns = new List<GridColumn>
                    {
                        new GridColumn { 
                            Field = "Title",
                            Order = 3
                        }
                    }
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid.Columns, Newtonsoft.Json.Formatting.Indented));

            var cols = shiftGrid.Columns.ToDictionary(x => x.Field, x => x);

            Assert.IsTrue(
                cols["Type"].Order == 0 &&
                cols["ID"].Order == 1 &&
                cols["CalculatedPrice"].Order == 2 &&
                cols["Title"].Order == 3 &&
                cols["TypeId"].Order == 4 &&
                cols["Items"].Order == 5
            );
        }

        [TestMethod]
        public async Task OverwriteMultipleAttributeByConfig()
        {
            await this.Utils.PopulateTestData(100, 6);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new TestItemView
                {
                    ID = x.ID,
                    Title = x.Title,
                    CalculatedPrice = x.Price,
                    TypeId = x.TypeId
                })
                .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
                new GridConfig
                {
                    Columns = new List<GridColumn>
                    {
                        new GridColumn {
                            Field = "Title",
                            Order = 1
                        },
                        new GridColumn {
                            Field = "Type",
                            Order = 2
                        },
                    }
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid.Columns, Newtonsoft.Json.Formatting.Indented));

            var cols = shiftGrid.Columns.ToDictionary(x => x.Field, x => x);

            Assert.IsTrue(
                cols["ID"].Order == 0 &&
                cols["Title"].Order == 1 &&
                cols["Type"].Order == 2 &&
                cols["CalculatedPrice"].Order == 3 &&
                cols["TypeId"].Order == 4 &&
                cols["Items"].Order == 5
            );
        }

        [TestMethod]
        public async Task LargeOrderByConfig()
        {
            await this.Utils.PopulateTestData(100, 6);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new TestItemView
                {
                    ID = x.ID,
                    Title = x.Title,
                    CalculatedPrice = x.Price,
                    TypeId = x.TypeId
                })
                .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
                new GridConfig
                {
                    Columns = new List<GridColumn>
                    {
                        new GridColumn {
                            Field = "Title",
                            Order = 100
                        },
                    }
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid.Columns, Newtonsoft.Json.Formatting.Indented));

            var cols = shiftGrid.Columns.ToDictionary(x => x.Field, x => x);

            Assert.IsTrue(
                cols["Type"].Order == 0 &&
                cols["ID"].Order == 1 &&
                cols["CalculatedPrice"].Order == 2 &&
                cols["TypeId"].Order == 3 &&
                cols["Items"].Order == 4 &&
                cols["Title"].Order == 5
            );
        }

        [TestMethod]
        public async Task SmallNumber()
        {
            await this.Utils.PopulateTestData(100, 6);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new TestItemView
                {
                    ID = x.ID,
                    Title = x.Title,
                    CalculatedPrice = x.Price,
                    TypeId = x.TypeId
                })
                .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
                new GridConfig
                {
                    Columns = new List<GridColumn>
                    {
                        new GridColumn {
                            Field = "Title",
                            Order = -1
                        },
                        new GridColumn {
                            Field = "Type",
                            Order = -2
                        },
                    }
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid.Columns, Newtonsoft.Json.Formatting.Indented));

            var cols = shiftGrid.Columns.ToDictionary(x => x.Field, x => x);

            Assert.IsTrue(
                cols["Title"].Order == 0 &&
                cols["Type"].Order == 1 &&
                cols["ID"].Order == 2 &&
                cols["CalculatedPrice"].Order == 3 &&
                cols["TypeId"].Order == 4 &&
                cols["Items"].Order == 5
            );
        }

        [TestMethod]
        public async Task OverwriteAttributeWithByConfig_UsingAndExistingAttributeOrder()
        {
            await this.Utils.PopulateTestData(100, 6);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new TestItemView
                {
                    ID = x.ID,
                    Title = x.Title,
                    CalculatedPrice = x.Price,
                    TypeId = x.TypeId
                })
                .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
                new GridConfig
                {
                    Columns = new List<GridColumn>
                    {
                        new GridColumn {
                            Field = "Title",
                            Order = 1
                        }
                    }
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid.Columns, Newtonsoft.Json.Formatting.Indented));

            var cols = shiftGrid.Columns.ToDictionary(x => x.Field, x => x);

            Assert.IsTrue(
                cols["Type"].Order == 0 &&
                cols["Title"].Order == 1 &&
                cols["ID"].Order == 2 &&
                cols["CalculatedPrice"].Order == 3 &&
                cols["TypeId"].Order == 4 &&
                cols["Items"].Order == 5
            );
        }

        class OrderModel2
        {
            public long ID { get; set; }
            [GridColumnAttribute(Order = 4)]
            public string Title { get; set; }
            public decimal? Price { get; set; }
            public decimal? RetailPrice { get; set; }
            public long? TypeId { get; set; }
            public virtual ICollection<TestItemView> Items { get; set; }
        }

        class OrderModel3
        {
            public long ID { get; set; }
            public string Title { get; set; }
            public decimal? Price { get; set; }
            public decimal? RetailPrice { get; set; }
            [GridColumnAttribute(Order = 2)]
            public long? TypeId { get; set; }
            public virtual ICollection<TestItemView> Items { get; set; }
        }

        class OrderModel4
        {
            public long ID { get; set; }
            public string Title { get; set; }
            [GridColumnAttribute(Order = 3)]
            public decimal? Price { get; set; }
            public decimal? RetailPrice { get; set; }
            [GridColumnAttribute(Order = 2)]
            public long? TypeId { get; set; }
            public virtual ICollection<TestItemView> Items { get; set; }
        }

        [TestMethod]
        public async Task OverwriteAttributeWithExistingOrder_2()
        {
            await this.Utils.PopulateTestData(100, 6);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new OrderModel2
                {
                    ID = x.ID,
                    Title = x.Title,
                    Price = x.Price,
                    RetailPrice = x.Price * 1.5m,
                    TypeId = x.TypeId,
                    Items = x.ChildTestItems.Select(y=> new TestItemView { 
                        ID = y.ID,
                    }).ToList()
                })
                .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
                new GridConfig
                {
                    Columns = new List<GridColumn>
                    {
                        new GridColumn {
                            Field = "ID",
                            Order = 0
                        }
                    }
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid.Columns, Newtonsoft.Json.Formatting.Indented));

            var cols = shiftGrid.Columns.ToDictionary(x => x.Field, x => x);

            Assert.IsTrue(
                cols["ID"].Order == 0 &&
                cols["Price"].Order == 1 &&
                cols["RetailPrice"].Order == 2 &&
                cols["TypeId"].Order == 3 &&
                cols["Title"].Order == 4 &&
                cols["Items"].Order == 5
            );
        }

        [TestMethod]
        public async Task OverwriteAttributeWithExistingOrder_3()
        {
            await this.Utils.PopulateTestData(100, 6);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new OrderModel3
                {
                    ID = x.ID,
                    Title = x.Title,
                    Price = x.Price,
                    RetailPrice = x.Price * 1.5m,
                    TypeId = x.TypeId,
                    Items = x.ChildTestItems.Select(y => new TestItemView
                    {
                        ID = y.ID,
                    }).ToList()
                })
                .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
                new GridConfig
                {
                    //Columns = new List<GridColumn>
                    //{
                    //    new GridColumn {
                    //        Field = "ID",
                    //        Order = 0 //1 is already assigned to Type by another attribute121
                    //    }
                    //}
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid.Columns, Newtonsoft.Json.Formatting.Indented));

            var cols = shiftGrid.Columns.ToDictionary(x => x.Field, x => x);

            Assert.IsTrue(
                cols["ID"].Order == 0 &&
                cols["Title"].Order == 1 &&
                cols["TypeId"].Order == 2 &&
                cols["Price"].Order == 3 &&
                cols["RetailPrice"].Order == 4 &&
                cols["Items"].Order == 5
            );
        }

        [TestMethod]
        public async Task OverwriteAttributeWithExistingOrder_4()
        {
            await this.Utils.PopulateTestData(100, 6);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new OrderModel4
                {
                    ID = x.ID,
                    Title = x.Title,
                    Price = x.Price,
                    RetailPrice = x.Price * 1.5m,
                    TypeId = x.TypeId,
                    Items = x.ChildTestItems.Select(y => new TestItemView
                    {
                        ID = y.ID,
                    }).ToList()
                })
                .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
                new GridConfig
                {
                    //Columns = new List<GridColumn>
                    //{
                    //    new GridColumn {
                    //        Field = "ID",
                    //        Order = 0 //1 is already assigned to Type by another attribute121
                    //    }
                    //}
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid.Columns, Newtonsoft.Json.Formatting.Indented));

            var cols = shiftGrid.Columns.ToDictionary(x => x.Field, x => x);

            Assert.IsTrue(
                cols["ID"].Order == 0 &&
                cols["Title"].Order == 1 &&
                cols["TypeId"].Order == 2 &&
                cols["Price"].Order == 3 &&
                cols["RetailPrice"].Order == 4 &&
                cols["Items"].Order == 5
            );
        }

        [TestMethod]
        public async Task OverwriteAttributeWithExistingOrder_Swap()
        {
            await this.Utils.PopulateTestData(100, 6);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new TestItemView
                {
                    ID = x.ID,
                    Title = x.Title,
                    CalculatedPrice = x.Price,
                    TypeId = x.TypeId
                })
                .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
                new GridConfig
                {
                    Columns = new List<GridColumn>
                    {
                        new GridColumn {
                            Field = "Title",
                            Order = 1 //1 is already assigned to Type by another attribute121
                        },
                        new GridColumn {
                            Field = "ID",
                            Order = 2 //[2] would be assigned to Type, since Title is taking over [1]
                                      //But now [2] is reserved for ID and Type should take [3]
                        },
                    },
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid.Columns, Newtonsoft.Json.Formatting.Indented));

            var cols = shiftGrid.Columns.ToDictionary(x => x.Field, x => x);

            Assert.IsTrue(
                cols["Type"].Order == 0 &&
                cols["Title"].Order == 1 &&
                cols["ID"].Order == 2 &&
                cols["CalculatedPrice"].Order == 3 &&
                cols["TypeId"].Order == 4 &&
                cols["Items"].Order == 5
            );
        }
    }
}