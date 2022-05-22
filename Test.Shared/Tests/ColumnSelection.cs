using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShiftSoftware.ShiftGrid.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using ShiftGrid.Test.Shared.Models;
using System.Threading.Tasks;


namespace ShiftGrid.Test.Shared.Tests
{
    public class ColumnSelection
    {
        public System.Type DBType { get; set; }
        public Shared.Utils Utils { get; set; }

        public ColumnSelection(System.Type type, Utils utils)
        {
            this.DBType = type;
            this.Utils = utils;
        }

        [TestMethod]
        [ExpectedException(typeof(ColumnHidingException))]
        public async Task ExcludeFieldFromDBSet()
        {
            await this.Utils.PopulateTestData(10);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .ToShiftGrid(new GridSort
                {
                    Field = "ID",
                    SortDirection = SortDirection.Ascending
                },
                new GridConfig
                {
                    Columns = new List<GridColumn>
                    {
                        new GridColumn {
                            Field = "TypeId",
                            Visible = false
                        }
                    }
                });
        }

        [TestMethod]
        public async Task ExcludeField()
        {
            await this.Utils.PopulateTestData(10);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new TestItemView
                {
                    ID = x.ID,
                    Title = x.Title,
                    CalculatedPrice = x.Price,
                    TypeId = x.TypeId
                })
                .ToShiftGrid(new GridSort
                {
                    Field = "ID",
                    SortDirection = SortDirection.Ascending
                },
                new GridConfig
                {
                    Columns = new List<GridColumn>
                    {
                        new GridColumn {
                            Field = "TypeId",
                            Visible = false
                        },
                        new GridColumn {
                            Field = "CalculatedPrice",
                            Visible = false
                        }
                    }
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Console.WriteLine(string.Join(Environment.NewLine + Environment.NewLine + Environment.NewLine, this.Utils.Logs));

            var cols = shiftGrid.Columns.ToDictionary(x => x.Field, x => x);

            Assert.IsTrue(
                shiftGrid.DataCount == 10 &&

                shiftGrid.Data.FirstOrDefault().Title == "Title - 1" &&
                shiftGrid.Data.FirstOrDefault().ID == 1 &&
                shiftGrid.Data.FirstOrDefault().TypeId == null &&
                shiftGrid.Data.FirstOrDefault().CalculatedPrice == null &&

                cols["ID"].Visible &&
                !cols["CalculatedPrice"].Visible &&
                cols["Title"].Visible &&
                !cols["TypeId"].Visible &&
                cols["Type"].Visible &&
                cols["Items"].Visible
            );
        }

        [TestMethod]
        public async Task ExcludeCollection()
        {
            await this.Utils.PopulateTestData(10);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new TestItemView
                {
                    ID = x.ID,
                    Title = x.Title,
                    CalculatedPrice = x.Price,
                    TypeId = x.TypeId,
                    Items = x.ChildTestItems.Select(y => new SubTestItemView {
                        Title = y.Title,
                    })
                })
                .ToShiftGrid(new GridSort
                {
                    Field = "ID",
                    SortDirection = SortDirection.Ascending
                },
                new GridConfig
                {
                    Columns = new List<GridColumn>
                    {
                        new GridColumn {
                            Field = "Items",
                            Visible = false
                        }
                    }
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Console.WriteLine(string.Join(Environment.NewLine + Environment.NewLine + Environment.NewLine, this.Utils.Logs));

            var cols = shiftGrid.Columns.ToDictionary(x => x.Field, x => x);

            Assert.IsTrue(
                shiftGrid.DataCount == 10 &&
                shiftGrid.Data.FirstOrDefault().Title == "Title - 1" &&
                shiftGrid.Data.FirstOrDefault().ID == 1 &&
                shiftGrid.Data.FirstOrDefault().Items == null &&

                cols["ID"].Visible &&
                cols["CalculatedPrice"].Visible &&
                cols["Title"].Visible &&
                cols["TypeId"].Visible &&
                cols["Type"].Visible &&
                !cols["Items"].Visible
            );
        }

        [TestMethod]
        public async Task IdenticalField_ExcludeFieldButKeepInCollection()
        {
            await this.Utils.PopulateTestData(10, 10);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new TestItemView
                {
                    ID = x.ID,
                    Title = x.Title,
                    CalculatedPrice = x.Price,
                    TypeId = x.TypeId,
                    Items = x.ChildTestItems.Select(y => new SubTestItemView
                    {
                        Title = y.Title,
                    })
                })
                .ToShiftGrid(new GridSort
                {
                    Field = "ID",
                    SortDirection = SortDirection.Ascending
                },
                new GridConfig
                {
                    Columns = new List<GridColumn>
                    {
                        new GridColumn {
                            Field = "Title",
                            Visible = false
                        }
                    }
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Console.WriteLine(string.Join(Environment.NewLine + Environment.NewLine + Environment.NewLine, this.Utils.Logs));

            var cols = shiftGrid.Columns.ToDictionary(x => x.Field, x => x);

            Assert.IsTrue(
                shiftGrid.Data.FirstOrDefault().Title == null &&
                shiftGrid.Data.FirstOrDefault().Items.First().Title.StartsWith("Sub Title") &&

                cols["ID"].Visible &&
                cols["CalculatedPrice"].Visible &&
                !cols["Title"].Visible &&
                cols["TypeId"].Visible &&
                cols["Type"].Visible &&
                cols["Items"].Visible
            );
        }

        [TestMethod]
        public async Task ExcludeField_AnnynomousObject()
        {
            await this.Utils.PopulateTestData(10, 0);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new
                {
                    ID = x.ID,
                    Title = x.Title,
                    Aliace = x.TypeId * x.Price,
                    x.TypeId,
                    x.Price
                })
                .ToShiftGrid(new GridSort
                {
                    Field = "ID",
                    SortDirection = SortDirection.Ascending
                },
                new GridConfig
                {
                    Columns = new List<GridColumn>
                    {
                        new GridColumn {
                            Field = "Title",
                            Visible = false
                        },
                        new GridColumn {
                            Field = "Aliace",
                            Visible = false
                        },
                    }
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            //Console.WriteLine(string.Join(Environment.NewLine + Environment.NewLine + Environment.NewLine, logs));

            var cols = shiftGrid.Columns.ToDictionary(x => x.Field, x => x);

            Assert.IsTrue(
                shiftGrid.DataCount == 10 &&

                shiftGrid.Data.FirstOrDefault().ID == 1 &&
                shiftGrid.Data.FirstOrDefault().Title == null &&
                shiftGrid.Data.FirstOrDefault().Aliace == null &&
                shiftGrid.Data.FirstOrDefault().TypeId == 1 &&
                shiftGrid.Data.FirstOrDefault().Price == 10m &&

                
                cols["ID"].Visible &&
                !cols["Title"].Visible &&
                !cols["Aliace"].Visible &&
                cols["TypeId"].Visible &&
                cols["Price"].Visible
            );
        }

        [TestMethod]
        public async Task ExcludeAFieldThatsInSummary()
        {
            await this.Utils.PopulateTestData(10);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new TestItemView
                {
                    ID = x.ID,
                    Title = x.Title,
                    CalculatedPrice = x.Price.Value,
                    TypeId = x.TypeId,
                    //Date = x.Date
                })
                .SelectSummary(x => new
                {
                    Count = x.Count(),
                    TotalID = x.Sum(y => y.ID),
                    TotalPrice = x.Sum(y => y.CalculatedPrice),
                    TotalMixed = x.Sum(y => y.CalculatedPrice * y.ID),
                    TotalMixed2 = x.Sum(y => y.CalculatedPrice) + x.Sum(y => y.CalculatedPrice),
                    MaxID = x.Max(y => y.ID).ToString()
                })
                .ToShiftGrid(new GridSort
                {
                    Field = "ID",
                    SortDirection = SortDirection.Ascending
                },
                new GridConfig
                {
                    Columns = new List<GridColumn>
                    {
                        new GridColumn {
                            Field = "CalculatedPrice",
                            Visible = false
                        }
                    }
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid.Summary, Newtonsoft.Json.Formatting.Indented));

            Console.WriteLine(string.Join(Environment.NewLine + Environment.NewLine + Environment.NewLine, this.Utils.Logs));

            var cols = shiftGrid.Columns.ToDictionary(x => x.Field, x => x);

            Assert.IsTrue(
                shiftGrid.DataCount == 10 &&

                shiftGrid.Data.FirstOrDefault().Title == "Title - 1" &&
                shiftGrid.Data.FirstOrDefault().ID == 1 &&
                shiftGrid.Data.FirstOrDefault().TypeId == 1 &&
                shiftGrid.Data.FirstOrDefault().CalculatedPrice == null &&

                cols["ID"].Visible &&
                !cols["CalculatedPrice"].Visible &&
                cols["Title"].Visible &&
                cols["Type"].Visible &&
                cols["Items"].Visible &&
                shiftGrid.Summary["TotalPrice"] == null &&
                shiftGrid.Summary["TotalMixed"] == null &&
                shiftGrid.Summary["MaxID"].Equals("10")
            );
        }

        class SummaryModel
        {
            public int Count { get; set; }
            public long TotalID { get; set; }
            public decimal? TotalPrice { get; set; }
            public decimal? TotalMixed { get; set; }
            public string MaxID { get; set; }
        }

        [TestMethod]
        public async Task ExcludeAFieldThatsInSummary_UsingAModel()
        {
            await this.Utils.PopulateTestData(10);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems
                .Select(x => new TestItemView
                {
                    ID = x.ID,
                    Title = x.Title,
                    CalculatedPrice = x.Price,
                    TypeId = x.TypeId
                })
                .SelectSummary(x => new SummaryModel
                {
                    Count = x.Count(),
                    TotalID = x.Sum(y => y.ID),
                    TotalPrice = x.Sum(y => y.CalculatedPrice),
                    TotalMixed = x.Sum(y => y.CalculatedPrice * y.ID),
                    MaxID = x.Max(y => y.ID).ToString()
                })
                .ToShiftGrid(new GridSort
                {
                    Field = "ID",
                    SortDirection = SortDirection.Ascending
                },
                new GridConfig
                {
                    Columns = new List<GridColumn>
                    {
                        new GridColumn {
                            Field = "CalculatedPrice",
                            Visible = false
                        }
                    }
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            //Console.WriteLine(string.Join(Environment.NewLine + Environment.NewLine + Environment.NewLine, logs));

            var cols = shiftGrid.Columns.ToDictionary(x => x.Field, x => x);

            Assert.IsTrue(
                shiftGrid.DataCount == 10 &&

                shiftGrid.Data.FirstOrDefault().Title == "Title - 1" &&
                shiftGrid.Data.FirstOrDefault().ID == 1 &&
                shiftGrid.Data.FirstOrDefault().TypeId == 1 &&
                shiftGrid.Data.FirstOrDefault().CalculatedPrice == null &&

                cols["ID"].Visible &&
                !cols["CalculatedPrice"].Visible &&
                cols["Title"].Visible &&
                cols["Type"].Visible &&
                cols["Items"].Visible &&
                shiftGrid.Summary["TotalPrice"] == null &&
                shiftGrid.Summary["TotalMixed"] == null &&
                shiftGrid.Summary["MaxID"].Equals("10") 
            );
        }
    }
}