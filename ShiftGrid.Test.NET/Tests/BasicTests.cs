using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShiftGrid.Core;
using ShiftGrid.Test.NET.Models;

namespace ShiftGrid.Test.NET.Tests
{
    [TestClass]
    public class BasicTests
    {
        private async Task DataInserter(int count)
        {
            var controller = new Controllers.UtilController();

            await controller.DeleteAll();

            await controller.InsertTestData(new InsertPayload
            {
                DataCount = count,
                DataTemplate = new DataTemplate
                {
                    Code = "Code",
                    Title = "Title",
                    Date = DateTime.Now,
                    Price = 10m,
                    TypeId = 1
                },
                Increments = new Increments
                {
                    Day = 1,
                    Price = 10
                }
            });
        }

        [TestMethod]
        public async Task BasicInsertTest()
        {
            await this.DataInserter(100);

            var db = Utils.GetDBContext();

            var itemToTest = await db.TestItems.FindAsync(50);

            Assert.AreEqual(itemToTest.Title, "Title - 50");
        }

        [TestMethod]
        public async Task NoConfig()
        {
            await this.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid();

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

                shiftGrid.Summary.DataCount == 100 &&

                shiftGrid.DataPageIndex == 0 &&
                shiftGrid.DataPageSize == 20 &&

                shiftGrid.Sort.First().Field == "ID" &&
                shiftGrid.Sort.First().SortDirection == SortDirection.Ascending &&

                shiftGrid.Data.First().Title == "Title - 1" &&
                shiftGrid.Data.Last().Title == "Title - 20"
            );
        }

        [TestMethod]
        public async Task Sorting()
        {
            await this.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(new Grid<TestItemView>
            {
                Sort = new System.Collections.ObjectModel.ObservableCollection<GridSort> {
                    new GridSort {
                        Field = nameof(TestItem.ID),
                        SortDirection = SortDirection.Descending,
                    }
                }
            });

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

                shiftGrid.Summary.DataCount == 100 &&

                shiftGrid.DataPageIndex == 0 &&
                shiftGrid.DataPageSize == 20 &&

                shiftGrid.Sort.First().Field == "ID" &&
                shiftGrid.Sort.First().SortDirection == SortDirection.Descending &&

                shiftGrid.Data.First().Title == "Title - 100" &&
                shiftGrid.Data.Last().Title == "Title - 81"
            );
        }

        [TestMethod]
        public async Task Pagination()
        {
            await this.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(new Grid<TestItemView>
            {
                DataPageSize = 5,
                DataPageIndex = 11,
                Pagination = new GridPagination
                {
                    PageSize = 5
                }
            });

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

                shiftGrid.Summary.DataCount == 100 &&

                shiftGrid.DataPageIndex == 11 &&
                shiftGrid.DataPageSize == 5 &&

                shiftGrid.Sort.First().Field == "ID" &&
                shiftGrid.Sort.First().SortDirection == SortDirection.Ascending &&

                shiftGrid.Data.First().Title == "Title - 56" &&
                shiftGrid.Data.Last().Title == "Title - 60"
            );
        }

        [TestMethod]
        public async Task Pagination2()
        {
            await this.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(new Grid<TestItemView>
            {
                DataPageSize = 3,
                DataPageIndex = 22,
                Pagination = new GridPagination
                {
                    PageSize = 5
                }
            });

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

                shiftGrid.Summary.DataCount == 100 &&

                shiftGrid.DataPageIndex == 22 &&
                shiftGrid.DataPageSize == 3 &&

                shiftGrid.Sort.First().Field == "ID" &&
                shiftGrid.Sort.First().SortDirection == SortDirection.Ascending &&

                shiftGrid.Data.First().Title == "Title - 67" &&
                shiftGrid.Data.Last().Title == "Title - 69"
            );
        }

        [TestMethod]
        public async Task Pagination3()
        {
            await this.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(new Grid<TestItemView>
            {
                DataPageSize = 15,
                DataPageIndex = 0,
                Pagination = new GridPagination
                {
                    PageSize = 10
                }
            });

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

                shiftGrid.Summary.DataCount == 100 &&

                shiftGrid.DataPageIndex == 0 &&
                shiftGrid.DataPageSize == 15 &&

                shiftGrid.Sort.First().Field == "ID" &&
                shiftGrid.Sort.First().SortDirection == SortDirection.Ascending &&

                shiftGrid.Data.First().Title == "Title - 1" &&
                shiftGrid.Data.Last().Title == "Title - 15"
            );
        }

        [TestMethod]
        public async Task Filters_Equals()
        {
            await this.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(new Grid<TestItemView>
            {
                Filters = new System.Collections.ObjectModel.ObservableCollection<GridFilter> {
                   new GridFilter
                   {
                       Field = nameof(TestItem.ID),
                       Operator = Core.GridFilterOperator.Equals,
                       Value = 1
                   }
               }
            });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Data.Count == 1 &&
                shiftGrid.Data.ElementAt(0).Title == "Title - 1"
            );
        }

        [TestMethod]
        public async Task Filters_In()
        {
            await this.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(new Grid<TestItemView>
            {
                Filters = new System.Collections.ObjectModel.ObservableCollection<GridFilter> {
                   new GridFilter
                   {
                       Field = nameof(TestItem.ID),
                       Operator = Core.GridFilterOperator.In,
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
        public async Task Filters_StartsWith()
        {
            await this.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(new Grid<TestItemView>
            {
                Filters = new System.Collections.ObjectModel.ObservableCollection<GridFilter> {
                   new GridFilter
                   {
                       Field = nameof(TestItem.Title),
                       Operator = Core.GridFilterOperator.StartsWith,
                       Value = "Title - 1"
                   }
               }
            });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Data.Count == 12 &&
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
            await this.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(new Grid<TestItemView>
            {
                Filters = new System.Collections.ObjectModel.ObservableCollection<GridFilter> {
                   new GridFilter
                   {
                       Field = nameof(TestItem.Title),
                       Operator = Core.GridFilterOperator.EndsWith,
                       Value = "1"
                   }
               }
            });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Data.Count == 10 &&
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
            await this.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(new Grid<TestItemView>
            {
                Filters = new System.Collections.ObjectModel.ObservableCollection<GridFilter> {
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
                shiftGrid.Data.Count == 3 &&
                shiftGrid.Data.ElementAt(0).Title == "Title - 1" &&
                shiftGrid.Data.ElementAt(1).Title == "Title - 21" &&
                shiftGrid.Data.ElementAt(2).Title == "Title - 31"
            );
        }

        [TestMethod]
        public async Task Filters_Or_InAndEquals()
        {
            await this.DataInserter(100);

            var db = Utils.GetDBContext();

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title
            })
            .ToShiftGrid(new Grid<TestItemView>
            {
                Filters = new System.Collections.ObjectModel.ObservableCollection<GridFilter> {
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
                shiftGrid.Data.Count == 6 &&
                shiftGrid.Data.ElementAt(0).Title == "Title - 14" &&
                shiftGrid.Data.ElementAt(1).Title == "Title - 19" &&
                shiftGrid.Data.ElementAt(2).Title == "Title - 33" &&
                shiftGrid.Data.ElementAt(3).Title == "Title - 44" &&
                shiftGrid.Data.ElementAt(4).Title == "Title - 55" &&
                shiftGrid.Data.ElementAt(5).Title == "Title - 66"
            );
        }
    }
}