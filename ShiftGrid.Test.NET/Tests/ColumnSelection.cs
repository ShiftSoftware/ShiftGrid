﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShiftSoftware.ShiftGrid.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ShiftGrid.Test.NET.Tests
{
    public class ColumnSelection
    {
        public System.Type DBType { get; set; }
        public ColumnSelection(System.Type type)
        {
            this.DBType = type;
        }

        [TestMethod]
        [ExpectedException(typeof(ColumnHidingException))]
        public async Task ExcludeFieldFromDBSet()
        {
            await Utils.DataInserter(this.DBType, 10);

            var db = Utils.GetDBContext(this.DBType);

            var shiftGrid = db.TestItems
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
            await Utils.DataInserter(this.DBType, 10);

            var db = Utils.GetDBContext(this.DBType);

            var logs = new List<string>();

            Controllers.DataController.SetupLogger(db, logs);

            var shiftGrid = db.TestItems
                .Select(x => new Models.TestItemView
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

            Console.WriteLine(string.Join(Environment.NewLine + Environment.NewLine + Environment.NewLine, logs));

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
            await Utils.DataInserter(this.DBType, 10);

            var db = Utils.GetDBContext(this.DBType);

            var logs = new List<string>();

            Controllers.DataController.SetupLogger(db, logs);

            var shiftGrid = db.TestItems
                .Select(x => new Models.TestItemView
                {
                    ID = x.ID,
                    Title = x.Title,
                    CalculatedPrice = x.Price,
                    TypeId = x.TypeId,
                    Items = x.ChildTestItems.Select(y => new Models.SubTestItemView {
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

            Console.WriteLine(string.Join(Environment.NewLine + Environment.NewLine + Environment.NewLine, logs));

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
            await Utils.DataInserter(this.DBType, 10, 10);

            var db = Utils.GetDBContext(this.DBType);

            var logs = new List<string>();

            Controllers.DataController.SetupLogger(db, logs);

            var shiftGrid = db.TestItems
                .Select(x => new Models.TestItemView
                {
                    ID = x.ID,
                    Title = x.Title,
                    CalculatedPrice = x.Price,
                    TypeId = x.TypeId,
                    Items = x.ChildTestItems.Select(y => new Models.SubTestItemView
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

            Console.WriteLine(string.Join(Environment.NewLine + Environment.NewLine + Environment.NewLine, logs));

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

        //This was a dirty fix to prevent people from trying to Hide Columns
        //When a Custom Model is not provided.
        //This worked (Somewhat fine). But they can use db.TestItems.ToShiftGrid()
        //In this case the exception is not thrown
        //[TestMethod]
        //[ExpectedException(typeof(AnonymousColumnHidingException))]
        //public async Task PreventExclusion_WhenNoModelProvided()
        //{
        //    await Utils.DataInserter(this.DBType, 10, 0);

        //    var db = Utils.GetDBContext(this.DBType);

        //    var logs = new List<string>();

        //    Controllers.DataController.SetupLogger(db, logs);

        //    var shiftGrid = db.TestItems
        //        .Select(x => new
        //        {
        //            ID = x.ID,
        //            Title = x.Title
        //        })
        //        .ToShiftGrid(new GridSort
        //        {
        //            Field = "ID",
        //            SortDirection = SortDirection.Ascending
        //        },
        //        new GridConfig
        //        {
        //            Columns = new List<GridColumn>
        //            {
        //                new GridColumn {
        //                    Field = "Title",
        //                    Visible = false
        //                }
        //            }
        //        });

        //    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

        //    Console.WriteLine(string.Join(Environment.NewLine + Environment.NewLine + Environment.NewLine, logs));
        //}

        [TestMethod]
        public async Task ExcludeAFieldThatsInSummary()
        {
            await Utils.DataInserter(this.DBType, 10);

            var db = Utils.GetDBContext(this.DBType);

            var logs = new List<string>();

            Controllers.DataController.SetupLogger(db, logs);

            var shiftGrid = db.TestItems
                .Select(x => new Models.TestItemView
                {
                    ID = x.ID,
                    Title = x.Title,
                    CalculatedPrice = x.Price,
                    TypeId = x.TypeId
                })
                .SelectSummary(x => new
                {
                    Count = x.Count(),
                    TotalID = x.Sum(y => y.ID),
                    TotalPrice = x.Sum(y => y.CalculatedPrice),
                    TotalMixed = x.Sum(y => y.CalculatedPrice * y.ID)
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
                shiftGrid.Summary["TotalMixed"] == null
            );
        }
    }
}