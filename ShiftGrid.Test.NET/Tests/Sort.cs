using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShiftGrid.Test.NET.Models;
using ShiftSoftware.ShiftGrid.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShiftGrid.Test.NET.Tests
{
    public class Sort
    {
        public System.Type DBType { get; set; }
        public Sort(System.Type type)
        {
            this.DBType = type;
        }

        [TestMethod]
        public async Task StableSort()
        {
            await Utils.DataInserter(DBType, 100, 0, 10);

            var db = Utils.GetDBContext(DBType);

            var logs = new List<string>();
            Controllers.DataController.SetupLogger(db, logs);

            var shiftGrid = db.TestItems.Select(x => new TestItemView
            {
                ID = x.ID,
                Title = x.Title,
                CalculatedPrice = x.Price
            })
            .ToShiftGrid(new GridSort
            {
                Field = "ID",
                SortDirection = SortDirection.Ascending
            },
            new GridConfig
            {
                DataPageSize = 10,
                DataPageIndex = 0,
                Sort = new List<GridSort> { 
                    new GridSort { 
                        Field = "CalculatedPrice",
                        SortDirection = SortDirection.Ascending
                    }
                }
            });

            var data = shiftGrid.Data.Select(x => (TestItemView)x);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                data.ElementAt(0).Title == "Title - 1" &&
                data.ElementAt(1).Title == "Title - 2" &&
                data.ElementAt(2).Title == "Title - 3" &&
                data.ElementAt(3).Title == "Title - 4" &&
                data.ElementAt(4).Title == "Title - 5" &&
                data.ElementAt(5).Title == "Title - 6" &&
                data.ElementAt(6).Title == "Title - 7" &&
                data.ElementAt(7).Title == "Title - 8" &&
                data.ElementAt(8).Title == "Title - 9" &&
                data.ElementAt(9).Title == "Title - 10"
            );
        }
    }
}