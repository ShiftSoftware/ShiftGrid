using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShiftGrid.Test.Shared.Models;
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

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shiftGrid, Newtonsoft.Json.Formatting.Indented));

            Assert.IsTrue(
                shiftGrid.Data.ElementAt(0).Title == "Title - 1" &&
                shiftGrid.Data.ElementAt(1).Title == "Title - 2" &&
                shiftGrid.Data.ElementAt(2).Title == "Title - 3" &&
                shiftGrid.Data.ElementAt(3).Title == "Title - 4" &&
                shiftGrid.Data.ElementAt(4).Title == "Title - 5" &&
                shiftGrid.Data.ElementAt(5).Title == "Title - 6" &&
                shiftGrid.Data.ElementAt(6).Title == "Title - 7" &&
                shiftGrid.Data.ElementAt(7).Title == "Title - 8" &&
                shiftGrid.Data.ElementAt(8).Title == "Title - 9" &&
                shiftGrid.Data.ElementAt(9).Title == "Title - 10"
            );
        }
    }
}