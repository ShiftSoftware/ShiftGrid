using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShiftGrid.Test.NET.Models;
using ShiftSoftware.ShiftGrid.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShiftGrid.Test.NET.Tests
{
    public class Pagination
    {
        public System.Type DBType { get; set; }
        public Pagination(System.Type type)
        {
            this.DBType = type;
        }

        [TestMethod]
        public async Task Pagination1()
        {
            await Utils.DataInserter(DBType, 100);

            var db = Utils.GetDBContext(DBType);

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(new GridConfig
            {
                DataPageSize = 5,
                DataPageIndex = 11,
                Pagination = new PaginationConfig
                {
                    PageSize = 5
                }
            });

            var data = shiftGrid.Data.Select(x => (TestItemView)x);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Pagination.Count == 20 &&
                shiftGrid.Pagination.PageSize == 5 &&
                shiftGrid.Pagination.PageStart == 10 &&
                shiftGrid.Pagination.PageEnd == 14 &&
                shiftGrid.Pagination.PageIndex == 1 &&
                shiftGrid.Pagination.HasPreviousPage == true &&
                shiftGrid.Pagination.HasNextPage == true &&
                shiftGrid.Pagination.LastPageIndex == 19 &&
                shiftGrid.Pagination.DataStart == 56 &&
                shiftGrid.Pagination.DataEnd == 60 &&

                shiftGrid.Summary["Count"].ToString() == "100" &&

                shiftGrid.DataPageIndex == 11 &&
                shiftGrid.DataPageSize == 5 &&

                shiftGrid.Sort.First().Field == "ID" &&
                shiftGrid.Sort.First().SortDirection == SortDirection.Ascending &&

                data.First().Title == "Title - 56" &&
                data.Last().Title == "Title - 60"
            );
        }

        [TestMethod]
        public async Task Pagination2()
        {
            await Utils.DataInserter(DBType, 100);

            var db = Utils.GetDBContext(DBType);

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(new GridConfig
            {
                DataPageSize = 3,
                DataPageIndex = 22,
                Pagination = new PaginationConfig
                {
                    PageSize = 5
                }
            });

            var data = shiftGrid.Data.Select(x => (TestItemView)x);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Pagination.Count == 34 &&
                shiftGrid.Pagination.PageSize == 5 &&
                shiftGrid.Pagination.PageStart == 20 &&
                shiftGrid.Pagination.PageEnd == 24 &&
                shiftGrid.Pagination.PageIndex == 2 &&
                shiftGrid.Pagination.HasPreviousPage == true &&
                shiftGrid.Pagination.HasNextPage == true &&
                shiftGrid.Pagination.LastPageIndex == 33 &&
                shiftGrid.Pagination.DataStart == 67 &&
                shiftGrid.Pagination.DataEnd == 69 &&

                shiftGrid.Summary["Count"].ToString() == "100" &&

                shiftGrid.DataPageIndex == 22 &&
                shiftGrid.DataPageSize == 3 &&

                shiftGrid.Sort.First().Field == "ID" &&
                shiftGrid.Sort.First().SortDirection == SortDirection.Ascending &&

                data.First().Title == "Title - 67" &&
                data.Last().Title == "Title - 69"
            );
        }

        [TestMethod]
        public async Task Pagination3()
        {
            await Utils.DataInserter(DBType, 100);

            var db = Utils.GetDBContext(DBType);

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(new GridConfig
            {
                DataPageSize = 15,
                DataPageIndex = 0,
                Pagination = new PaginationConfig
                {
                    PageSize = 10
                }
            });

            var data = shiftGrid.Data.Select(x => (TestItemView)x);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Pagination.Count == 7 &&
                shiftGrid.Pagination.PageSize == 10 &&
                shiftGrid.Pagination.PageStart == 0 &&
                shiftGrid.Pagination.PageEnd == 6 &&
                shiftGrid.Pagination.PageIndex == 0 &&
                shiftGrid.Pagination.HasPreviousPage == false &&
                shiftGrid.Pagination.HasNextPage == false &&
                shiftGrid.Pagination.LastPageIndex == 6 &&
                shiftGrid.Pagination.DataStart == 1 &&
                shiftGrid.Pagination.DataEnd == 15 &&

                shiftGrid.Summary["Count"].ToString() == "100" &&

                shiftGrid.DataPageIndex == 0 &&
                shiftGrid.DataPageSize == 15 &&

                shiftGrid.Sort.First().Field == "ID" &&
                shiftGrid.Sort.First().SortDirection == SortDirection.Ascending &&

                data.First().Title == "Title - 1" &&
                data.Last().Title == "Title - 15"
            );
        }

        [TestMethod]
        public async Task OutOfBound()
        {
            await Utils.DataInserter(DBType, 100);

            var db = Utils.GetDBContext(DBType);

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(new GridConfig
            {
                DataPageSize = 10,
                DataPageIndex = 90,
            });

            var data = shiftGrid.Data.Select(x => (TestItemView)x);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Pagination.LastPageIndex == 9 &&
                shiftGrid.Pagination.DataStart == 91 &&
                shiftGrid.Pagination.DataEnd == 100 &&

                shiftGrid.DataCount == 100 &&
                shiftGrid.DataPageIndex == 9 &&

                data.First().Title == "Title - 91" &&
                data.Last().Title == "Title - 100"
            );
        }
    }
}