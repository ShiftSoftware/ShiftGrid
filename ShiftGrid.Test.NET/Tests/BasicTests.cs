using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using ShiftGrid.Test.NET.Models;
using ShiftSoftware.ShiftGrid.Core;
using System.Collections.Generic;

namespace ShiftGrid.Test.NET.Tests
{
    public class BasicTests
    {
        public System.Type DBType { get; set; }
        public BasicTests(System.Type type)
        {
            this.DBType = type;
        }

        [TestMethod]
        public async Task BasicInsertTest()
        {
            await Utils.DataInserter(this.DBType, 100);

            var db = Utils.GetDBContext(this.DBType);

            var itemToTest = await db.TestItems.FindAsync(50);

            Assert.AreEqual(itemToTest.Title, "Title - 50");
        }

        [TestMethod]
        public async Task NoConfig()
        {
            await Utils.DataInserter(this.DBType, 100, 6);

            var db = Utils.GetDBContext(this.DBType);

            var logs = new List<string>();

            Controllers.DataController.SetupLogger(db, logs);

            var shiftGrid = db.TestItems.ToShiftGrid();

            var objectProps = shiftGrid.Data.First().GetType().GetProperties().Where(x=> x.MemberType == System.Reflection.MemberTypes.Property).ToList();

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Console.WriteLine(string.Join(Environment.NewLine + Environment.NewLine + Environment.NewLine, logs));
            //Console.WriteLine($"Log Count is: {logs.Count}");


            Assert.IsTrue(
                shiftGrid.Pagination.Count == 6 &&
                shiftGrid.Pagination.PageSize == 10 &&
                shiftGrid.Pagination.PageStart == 0 &&
                shiftGrid.Pagination.PageEnd == 5 &&
                shiftGrid.Pagination.PageIndex == 0 &&
                shiftGrid.Pagination.HasPreviousPage == false &&
                shiftGrid.Pagination.HasNextPage == false &&
                shiftGrid.Pagination.LastPageIndex == 5 &&
                shiftGrid.Pagination.DataStart == 1 &&
                shiftGrid.Pagination.DataEnd == 20 &&

                shiftGrid.Summary["Count"].ToString() == "106" &&

                shiftGrid.DataPageIndex == 0 &&
                shiftGrid.DataPageSize == 20 &&

                shiftGrid.Sort.First().Field == "ID" &&
                shiftGrid.Sort.First().SortDirection == SortDirection.Ascending &&

                objectProps.FirstOrDefault(x=> x.Name == "Title").GetValue(shiftGrid.Data.FirstOrDefault()).ToString() == "Title - 1" &&

                //1 for listing the table
                //1 for counting
                //2 for getting the Types (Type -1) and (Type - 2)
                //20 for Checking sub items. For each of the 20 items that are listed.
                //6 for getting each of the sub-items of the first item (ID = 1)
                logs.Count == 30
            );
        }

        [TestMethod]
        public async Task NoConfigWithModel()
        {
            await Utils.DataInserter(this.DBType, 100);

            var db = Utils.GetDBContext(this.DBType);

            var logs = new List<string>();
            Controllers.DataController.SetupLogger(db, logs);

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid();

            var data = shiftGrid.Data.Select(x => (TestItemView)x);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Pagination.Count == 5 &&
                shiftGrid.Pagination.PageSize == 10 &&
                shiftGrid.Pagination.PageStart == 0 &&
                shiftGrid.Pagination.PageEnd == 4 &&
                shiftGrid.Pagination.PageIndex == 0 &&
                shiftGrid.Pagination.HasPreviousPage == false &&
                shiftGrid.Pagination.HasNextPage == false &&
                shiftGrid.Pagination.LastPageIndex == 4 &&
                shiftGrid.Pagination.DataStart == 1 &&
                shiftGrid.Pagination.DataEnd == 20 &&

                shiftGrid.Summary["Count"].ToString() == "100" &&

                shiftGrid.DataPageIndex == 0 &&
                shiftGrid.DataPageSize == 20 &&

                shiftGrid.Sort.First().Field == "ID" &&
                shiftGrid.Sort.First().SortDirection == SortDirection.Ascending &&

                data.First().Title == "Title - 1" &&
                data.Last().Title == "Title - 20" &&

                logs.Count() == 2
            );
        }

        [TestMethod]
        public async Task Export()
        {
            await Utils.DataInserter(this.DBType, 100);

            var db = Utils.GetDBContext(this.DBType);

            var logs = new List<string>();
            Controllers.DataController.SetupLogger(db, logs);

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title,
                CalculatedPrice = x.Price * 10m,
                TypeId = x.TypeId,
                Type = x.Type.Name
            })
            .ToShiftGrid(new GridConfig
            {
                Filters = new List<GridFilter> {
                    new GridFilter {
                        Field = "ID",
                        Operator = "<=",
                        Value = 50
                    }
                },
                ExportConfig = new ExportConfig
                {
                    Export = true
                }
            });

            var data = shiftGrid.ToCSV();

            Console.WriteLine(data);

            Assert.IsTrue(
                shiftGrid.Data.Count() == 50 &&

                data.StartsWith("ID") &&
                data.TrimEnd().EndsWith("Title - 50,2,Type - 2") &&

                logs.Count() == 2
            );
        }

        [TestMethod]
        public async Task ExportWithHiddenFields()
        {
            await Utils.DataInserter(this.DBType, 100);

            var db = Utils.GetDBContext(this.DBType);

            var logs = new List<string>();
            Controllers.DataController.SetupLogger(db, logs);

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title,
                CalculatedPrice = x.Price * 10m,
                Type = x.Type.Name
            })
            .ToShiftGrid(new GridConfig
            {
                Filters = new List<GridFilter> {
                    new GridFilter {
                        Field = "ID",
                        Operator = "<=",
                        Value = 50
                    }
                },
                ExportConfig = new ExportConfig
                {
                    Export = true
                },
                Columns = new List<GridColumn>
                {
                    new GridColumn {
                        Field = "ID",
                    },
                    new GridColumn {
                        Field = "Title",
                    }
                }
            });

            var data = shiftGrid.ToCSV();

            Console.WriteLine(data);

            Assert.IsTrue(
                shiftGrid.Data.Count() == 50 &&

                data.StartsWith("ID") &&
                data.TrimEnd().EndsWith("Title - 50") &&

                logs.Count() == 2
            );
        }
    }
}