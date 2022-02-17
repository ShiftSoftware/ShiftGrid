using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using ShiftGrid.Test.NET.Models;
using ShiftSoftware.ShiftGrid.Core;

namespace ShiftGrid.Test.NET.Tests
{
    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public async Task BasicInsertTest()
        {
            await Utils.DataInserter(100);

            var db = Utils.GetDBContext();

            var itemToTest = await db.TestItems.FindAsync(50);

            Assert.AreEqual(itemToTest.Title, "Title - 50");
        }

        [TestMethod]
        public async Task NoConfig()
        {
            await Utils.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.ToShiftGrid();

            var objectProps = shiftGrid.Data.First().GetType().GetProperties().Where(x=> x.MemberType == System.Reflection.MemberTypes.Property).ToList();

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

                objectProps.FirstOrDefault(x=> x.Name == "Title").GetValue(shiftGrid.Data.FirstOrDefault()).ToString() == "Title - 1"
            );
        }

        [TestMethod]
        public async Task NoConfigWithModel()
        {
            await Utils.DataInserter(100);

            var db = Utils.GetDBContext();

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
                data.Last().Title == "Title - 20"
            );
        }
    }
}