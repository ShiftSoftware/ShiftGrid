using ShiftGrid.Test.Shared.Insert;
using ShiftGrid.Test.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.NETCore.EF;
using Microsoft.EntityFrameworkCore;

namespace Test.NETCore
{
    public class Utils : ShiftGrid.Test.Shared.Utils
    {
        public System.Type DBType { get; set; }
        public List<string> Logs { get; set; }

        public Utils(System.Type dbType)
        {
            this.DBType = dbType;
            this.Logs = new List<string>();
        }

        public DBBase GetDBContext()
        {
            if (this.DBType == typeof(MySqlDB))
                return new MySqlDB();

            else if (this.DBType == typeof(DB))
                return new DB();

            return null;
        }

        public async Task DeleteAll()
        {
            var db = this.GetDBContext();

            await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE TestItems");

            await db.Database.ExecuteSqlRawAsync("insert into Types (Name) values ('')");
            await db.Database.ExecuteSqlRawAsync("delete from Types");

            //SQL Server
            if (db.GetType() == typeof(EF.DB))
            {
                await db.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT (Types, RESEED, 0)");
            }
            //MySQL
            else if (db.GetType() == typeof(EF.MySqlDB))
            {
                await db.Database.ExecuteSqlRawAsync("ALTER TABLE Types AUTO_INCREMENT = 1");
            }
        }

        public IQueryable<TestItem>? GetTestItems()
        {
            var db = this.GetDBContext();

            this.Logs = db.Logs;

            return db?.TestItems?.AsQueryable();
        }

        public async Task InsertTestItems(InsertPayload payload)
        {
            var db = this.GetDBContext();

            var testItems = new Tools().GenerateTestItems(payload);

            db.TestItems.AddRange(testItems);

            await db.SaveChangesAsync();
        }

        public async Task InsertTypes()
        {
            var db = this.GetDBContext();

            var types = new Tools().GenerateTypes();

            db.Types.AddRange(types);

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
