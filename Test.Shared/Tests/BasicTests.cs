using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using ShiftGrid.Test.Shared.Models;
using ShiftSoftware.ShiftGrid.Core;
using System.Collections.Generic;

namespace ShiftGrid.Test.Shared.Tests
{
    public class BasicTests
    {
        public System.Type DBType { get; set; }
        public Shared.Utils Utils { get; set; }

        public BasicTests(System.Type type, Utils utils)
        {
            this.DBType = type;
            this.Utils = utils;
        }

        [TestMethod]
        public async Task BasicInsertTest()
        {
                    await this.Utils.PopulateTestData(100, 6);

            var testItems = this.Utils.GetTestItems();

            var itemToTest = testItems.FirstOrDefault(x => x.ID == 50);

            Assert.AreEqual(itemToTest.Title, "Title - 50");
        }

        [TestMethod]
        public async Task NoConfig()
        {
            await this.Utils.PopulateTestData(100, 6);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems.ToShiftGrid(nameof(TestItem.ID));

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Console.WriteLine(string.Join(Environment.NewLine + Environment.NewLine + Environment.NewLine, this.Utils.Logs));
            Console.WriteLine($"Log Count is: {this.Utils.Logs.Count }");


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

                shiftGrid.DataCount == 106 &&

                shiftGrid.DataPageIndex == 0 &&
                shiftGrid.DataPageSize == 20 &&

                shiftGrid.Sort.First().Field == "ID" &&
                shiftGrid.Sort.First().SortDirection == SortDirection.Ascending &&

                shiftGrid.Data.FirstOrDefault().Title == "Title - 1" &&

                //1 for listing the table
                //1 for counting
                //2 for getting the Types (Type -1) and (Type - 2)
                //20 for Checking sub items. For each of the 20 items that are listed.
                //6 for getting each of the sub-items of the first item (ID = 1)
                this.Utils.Logs.Count == 30
            );
        }

        [TestMethod]
        public async Task NoConfigWithModel()
        {
            await this.Utils.PopulateTestData(100);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(nameof(TestItemView.ID));

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

                shiftGrid.DataCount == 100 &&

                shiftGrid.DataPageIndex == 0 &&
                shiftGrid.DataPageSize == 20 &&

                shiftGrid.Sort.First().Field == "ID" &&
                shiftGrid.Sort.First().SortDirection == SortDirection.Ascending &&

                shiftGrid.Data.First().Title == "Title - 1" &&
                shiftGrid.Data.Last().Title == "Title - 20" &&

                this.Utils.Logs.Count() == 2
            );
        }

        [TestMethod]
        public async Task Export()
        {
            await this.Utils.PopulateTestData(100);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title,
                CalculatedPrice = x.Price * 10m,
                TypeId = x.TypeId,
                Type = x.Type.Name
            })
            .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
            new GridConfig
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

            var data = shiftGrid.ToCSVString(); 

            Console.WriteLine(data);

            Assert.IsTrue(
                shiftGrid.Data.Count() == 50 &&

                data.StartsWith("ID") &&
                data.TrimEnd().EndsWith("Title - 50,Type - 2") &&

                this.Utils.Logs.Count() == 1
            );
        }

        [TestMethod]
        public async Task ExportWithExcludedFields()
        {
            await this.Utils.PopulateTestData(100);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title,
                CalculatedPrice = x.Price * 10m,
                Type = x.Type.Name
            })
            .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
            new GridConfig
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
                        Field = "CalculatedPrice",
                        Visible = false,
                    },
                    new GridColumn {
                        Field = "Type",
                        Visible = false,
                    }
                },
            });

            var data = shiftGrid.ToCSVString();

            Console.WriteLine(data);

            Assert.IsTrue(
                shiftGrid.Data.Count() == 50 &&

                data.StartsWith("ID") &&
                data.TrimEnd().EndsWith("Title - 50") &&

                this.Utils.Logs.Count() == 1
            );
        }

        [TestMethod]
        public async Task ExportWithDifferentDelimiter()
        {
            await this.Utils.PopulateTestData(100);

            var testItems = this.Utils.GetTestItems();

            var shiftGrid = testItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title,
                CalculatedPrice = x.Price * 10m,
                Type = x.Type.Name
            })
            .ToShiftGrid(nameof(TestItemView.ID), SortDirection.Ascending,
            new GridConfig
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
                    Export = true,
                    Delimiter = "|"
                }
            });

            var data = shiftGrid.ToCSVString();

            Console.WriteLine(data);

            Assert.IsTrue(
                shiftGrid.Data.Count() == 50 &&

                data.StartsWith("ID|") &&
                data.TrimEnd().EndsWith("|Title - 50|Type - 2") &&

                this.Utils.Logs.Count() == 1
            );
        }
    }
}