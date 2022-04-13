using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ShiftSoftware.ShiftGrid.Core;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;

namespace ShiftGrid.Test.NET.Tests
{
    [TestClass]
    public class InMemory
    {
        class DataModel
        {
            public long ID { get; set; }
            public string Name { get; set; }
        }

        private List<DataModel> GenerateData(int count)
        {
            var list = new List<DataModel>();

            for (int i = 0; i < count; i++)
            {
                list.Add(new DataModel
                {
                    ID = i,
                    Name = $"Name - {(i + 1) % count}",
                });
            }

            return list;
        }

        [TestMethod]
        public void Basic()
        {
            var inMemoryData = new List<DataModel> { 
                new DataModel {
                  ID = 1,
                  Name = "One"
                },
                new DataModel{
                  ID = 2,
                  Name = "Two"
                },
            };

            var shiftGrid = inMemoryData.AsQueryable()
            .ToShiftGrid(new GridSort
            {
                Field = "ID",
                SortDirection = SortDirection.Descending
            });

            Assert.IsTrue(
                shiftGrid.DataCount == 2 &&
                shiftGrid.Columns.First().Field == "ID" &&
                shiftGrid.Columns.Last().Field == "Name" &&
                shiftGrid.Data.First().ID == 2 &&
                shiftGrid.Data.ElementAt(1).ID == 1
            );
        }

        [TestMethod]
        public void FilteringSortingAndPagination()
        {
            var data = this.GenerateData(100);

            var shiftGrid = data
                .AsQueryable()
                .ToShiftGrid(
                    new GridSort
                    {
                        Field = "Name",
                        SortDirection = SortDirection.Descending
                    },
                    new GridConfig
                    {
                        DataPageIndex = 1,
                        DataPageSize = 5,
                        Filters = new List<GridFilter>
                        {
                            new GridFilter
                            {
                                Field = "Name",
                                Operator = GridFilterOperator.Contains,
                                Value = "Name - 1"
                            }
                        }
                    }
                );

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.DataCount == 11 &&
                shiftGrid.Columns.First().Field == "ID" &&
                shiftGrid.Columns.Last().Field == "Name" &&
                shiftGrid.Data.ElementAt(0).Name == "Name - 14" &&
                shiftGrid.Data.ElementAt(4).Name == "Name - 10" &&
                shiftGrid.Data.ElementAt(1).ID == 12
            );
        }
    }
}