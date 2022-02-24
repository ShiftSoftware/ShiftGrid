using ShiftGrid.Test.NET.EF;
using System;
using System.Threading.Tasks;

namespace ShiftGrid.Test.NET
{
    public class Utils
    {
        public static async Task DataInserter(int count, int? subCount = null)
        {
            var controller = new Controllers.UtilController();

            await controller.DeleteAll();

            await controller.InsertTypes();

            await controller.InsertTestData(new InsertPayload
            {
                DataCount = count,
                DataTemplate = new DataTemplate
                {
                    Code = "Code",
                    Title = "Title",
                    Date = DateTime.Now,
                    Price = 10m,
                },
                Increments = new Increments
                {
                    Day = 1,
                    Price = 10
                }
            });

            if (subCount != null)
            {
                await controller.InsertTestData(new InsertPayload
                {
                    DataCount = subCount.Value,
                    ParentTestItemId = 1,
                    DataTemplate = new DataTemplate
                    {
                        Code = "Sub Code",
                        Title = "Sub Title",
                        Date = DateTime.Now,
                        Price = 10m
                    },
                    Increments = new Increments
                    {
                        Day = 1,
                        Price = 10
                    }
                });
            }
        }
        public static MySQLDb GetDBContext()
        {
            return new MySQLDb();
        }
    }
}