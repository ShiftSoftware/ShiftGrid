using ShiftGrid.Test.NET.EF;
using System;
using System.Threading.Tasks;

namespace ShiftGrid.Test.NET
{
    public class Utils
    {
        public static async Task DataInserter(Type dbType, int count, int? subCount = null, int step = 1)
        {
            var controller = new Controllers.UtilController(dbType);

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
                    Price = 10,
                    Step = step
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
        public static DBBase GetDBContext(Type dbType)
        {
            if (dbType == typeof(MySQLDb))
                return new MySQLDb();

            else if (dbType == typeof(DB))
                return new DB();

            return null;
        }
    }
}