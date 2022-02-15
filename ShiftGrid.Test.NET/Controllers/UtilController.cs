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
        [HttpGet, Route("test")]
        public IHttpActionResult Test()
        {
            return Ok("Ok");
        }

        [HttpDelete, Route("delete-all")]
        public async Task<IHttpActionResult> DeleteAll()
        {
            var db = Utils.GetDBContext();

            foreach (var item in (await db.TestItems.ToListAsync()))
            {
                db.TestItems.Remove(item);
            }

            await db.SaveChangesAsync();

            await db.Database.ExecuteSqlCommandAsync("DBCC CHECKIDENT (TestItems, RESEED, 0)");

            return Ok();
        }

        [HttpPost, Route("insert-test-data")]
        public async Task<IHttpActionResult> InsertTestData(InsertPayload payload)
        {
            var db = Utils.GetDBContext();

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
                    TypeId = payload.DataTemplate.TypeId,
                };

                db.TestItems.Add(testItem);
            }

            await db.SaveChangesAsync();

            return Ok();
        }
    }
}