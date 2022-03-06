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
            var inMemoryData = new List<object> { 
                new {
                  ID = 1,
                  Name = "One"
                },
                new {
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

            var data = shiftGrid.Data;

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            var jsonData = Newtonsoft.Json.Linq.JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid.Data));

            Assert.IsTrue(
                shiftGrid.DataCount == 2 &&
                shiftGrid.Columns.First().Field == "ID" &&
                shiftGrid.Columns.Last().Field == "Name" &&
                jsonData[0]["ID"].ToObject<int>() == 2 &&
                jsonData[1]["ID"].ToObject<int>() == 1
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
                        Columns = new List<GridColumn> {
                            new GridColumn
                            {
                                Field = "Name"
                            }
                        },
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

            var jsonData = Newtonsoft.Json.Linq.JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid.Data));

            Assert.IsTrue(
                shiftGrid.DataCount == 11 &&
                shiftGrid.Columns.First().Field == "Name" &&
                jsonData[0]["Name"].ToObject<string>() == "Name - 14" &&
                jsonData[4]["Name"].ToObject<string>() == "Name - 10" &&
                jsonData[1]["ID"] == null
            );
        }
    }
}