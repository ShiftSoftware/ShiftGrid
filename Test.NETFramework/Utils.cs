using ShiftGrid.Test.Shared.Insert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.NETFramework.EF;

namespace Test.NETFramework
{
    public class Utils : ShiftGrid.Test.Shared.Utils
    {
        public Type DBType { get; set; }
        public List<string> Logs { get; set; }

        public Utils(Type dbType)
        {
            this.DBType = dbType;
            this.Logs = new List<string>();
        }

        //public static async Task DataInserter(Type dbType, int count, int? subCount = null, int step = 1)
        //{
        //    var controller = new Controllers.UtilController(dbType);

        //    await controller.DeleteAll();

        //    await controller.InsertTypes();

        //    await controller.InsertTestData(new InsertPayload
        //    {
        //        DataCount = count,
        //        DataTemplate = new DataTemplate
        //        {
        //            Code = "Code",
        //            Title = "Title",
        //            Date = DateTime.Now,
        //            Price = 10m,
        //        },
        //        Increments = new Increments
        //        {
        //            Day = 1,
        //            Price = 10,
        //            Step = step
        //        }
        //    });

        //    if (subCount != null)
        //    {
        //        await controller.InsertTestData(new InsertPayload
        //        {
        //            DataCount = subCount.Value,
        //            ParentTestItemId = 1,
        //            DataTemplate = new DataTemplate
        //            {
        //                Code = "Sub Code",
        //                Title = "Sub Title",
        //                Date = DateTime.Now,
        //                Price = 10m
        //            },
        //            Increments = new Increments
        //            {
        //                Day = 1,
        //                Price = 10
        //            }
        //        });
        //    }
        //}

        public DBBase GetDBContext()
        {
            if (this.DBType == typeof(MySQLDb))
                return new MySQLDb();

            else if (this.DBType == typeof(DB))
                return new DB();

            return null;
        }

        public IQueryable<ShiftGrid.Test.Shared.Models.TestItem> GetTestItems()
        {
            var db = this.GetDBContext();

            db.Database.Log = System.Console.Write;

            db.Database.Log = (s) =>
            {
                if (!(s.StartsWith("Opened connection") || s.StartsWith("Closed") || s.StartsWith("--")))
                {
                    if (!string.IsNullOrWhiteSpace(s))
                        this.Logs.Add(s.Replace("\r", "").Replace("\n", ""));

                    System.Diagnostics.Debug.WriteLine(s);
                }
            };

            return db.TestItems.AsQueryable();
        }

        public async Task DeleteAll()
        {
            var db = this.GetDBContext();

            await db.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE TestItems");
            await db.Database.ExecuteSqlCommandAsync("delete from Types");

            //SQL Server
            if (db.GetType() == typeof(EF.DB))
            {
                await db.Database.ExecuteSqlCommandAsync("DBCC CHECKIDENT (Types, RESEED, 0)");
            }
            //MySQL
            else if (db.GetType() == typeof(EF.MySQLDb))
            {
                await db.Database.ExecuteSqlCommandAsync("ALTER TABLE Types AUTO_INCREMENT = 1");
            }
        }

        public async Task InsertTypes()
        {
            var db = this.GetDBContext();

            var types = new Tools().GenerateTypes();

            db.Types.AddRange(types);

            await db.SaveChangesAsync();
        }

        public async Task InsertTestItems(InsertPayload payload)
        {
            var db = this.GetDBContext();

            var testItems = new Tools().GenerateTestItems(payload);

            db.TestItems.AddRange(testItems);

            await db.SaveChangesAsync();
        }

        public async Task PopulateTestData(int count, int? subCount = null, int step = 1)
        {
            await this.DeleteAll();

            await this.InsertTypes();

            await this.InsertTestItems(new InsertPayload
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
                await this.InsertTestItems(new InsertPayload
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
    }
}