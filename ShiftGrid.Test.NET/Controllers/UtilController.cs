using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ShiftGrid.Test.NET.Controllers
{
    [RoutePrefix("api")]
    public class UtilController : ApiController
    {
        public Type DBType { get; set; }

        public UtilController()
        {
            var db = System.Web.HttpContext.Current.Request.Headers["database"].ToString();

            if (db == "SqlServer")
                this.DBType = typeof(EF.DB);

            else if (db == "MySql")
                this.DBType = typeof(EF.MySQLDb);
        }

        public UtilController(Type dbType)
        {
            this.DBType = dbType;
        }

        [HttpGet, Route("test")]
        public IHttpActionResult Test()
        {
            return Ok("Ok");
        }

        [HttpDelete, Route("delete-all")]
        public async Task<IHttpActionResult> DeleteAll()
        {
            var db = Utils.GetDBContext(this.DBType);

            foreach (var item in (await db.TestItems.ToListAsync()))
                db.TestItems.Remove(item);

            foreach (var item in (await db.Types.ToListAsync()))
                db.Types.Remove(item);

            await db.SaveChangesAsync();

            //SQL Server
            if (db.GetType() == typeof(EF.DB))
            {
                await db.Database.ExecuteSqlCommandAsync("DBCC CHECKIDENT (TestItems, RESEED, 0)");
                await db.Database.ExecuteSqlCommandAsync("DBCC CHECKIDENT (Types, RESEED, 0)");
            }
            //MySQL
            else if(db.GetType() == typeof(EF.MySQLDb))
            {
                await db.Database.ExecuteSqlCommandAsync("ALTER TABLE TestItems AUTO_INCREMENT = 1");
                await db.Database.ExecuteSqlCommandAsync("ALTER TABLE Types AUTO_INCREMENT = 1");
            }

            return Ok();
        }

        [HttpPost, Route("insert-test-data")]
        public async Task<IHttpActionResult> InsertTestData(InsertPayload payload)
        {
            if (this.DBType == null)
                this.DBType = null;

            var db = Utils.GetDBContext(this.DBType);

            var typeIds = (await db.Types.Select(x => x.ID).ToListAsync()).Select(x => (long?)x).ToList();

            for (int i = 0; i < payload.DataCount; i++)
            {
                var number = i + 1;
                var testItem = new Models.TestItem()
                {
                    ParentTestItemId = payload.ParentTestItemId,
                    Code = $"{payload.DataTemplate.Code} - {number}",
                    Title = $"{payload.DataTemplate.Title} - {number}",
                    Date = payload.DataTemplate.Date.AddDays(i * payload.Increments.Day),
                    Price = payload.DataTemplate.Price + (i * payload.Increments.Price),
                    TypeId = typeIds.Count == 0 ? null : typeIds.ElementAt(i % 2)
                };

                db.TestItems.Add(testItem);
            }

            await db.SaveChangesAsync();

            return Ok();
        }

        [HttpPost, Route("insert-types")]
        public async Task<IHttpActionResult> InsertTypes()
        {
            var db = Utils.GetDBContext(this.DBType);

            for (int i = 0; i < 2; i++)
            {
                var type = new Models.Type
                {
                    Name = $"Type - {(i + 1)}"
                };

                db.Types.Add(type);
            }

            await db.SaveChangesAsync();

            return Ok();
        }
    }
}