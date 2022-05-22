using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ShiftGrid.Test.Shared.Models;
using Z.BulkOperations;
using ShiftGrid.Test.Shared.Insert;

namespace ShiftGrid.Test.NET.Controllers
{
    [RoutePrefix("api")]
    public class UtilController : ApiController
    {
        public System.Type DBType { get; set; }

        public UtilController()
        {

        }

        public UtilController(System.Type dbType)
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
            this.AssignDbType();

            var db = Utils.GetDBContext(this.DBType);

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

            return Ok();
        }

        private void AssignDbType()
        {
            if (DBType == null)
            {
                var db = this.Request.Headers.GetValues("database").FirstOrDefault()?.ToString();

                if (db == "SqlServer")
                    this.DBType = typeof(EF.DB);

                else if (db == "MySql")
                    this.DBType = typeof(EF.MySQLDb);
            }
        }

        [HttpPost, Route("insert-test-data")]
        public async Task<IHttpActionResult> InsertTestData(InsertPayload payload)
        {
            this.AssignDbType();

            var db = Utils.GetDBContext(this.DBType);

            var testItems = new Shared.Insert.Tools().GenerateTestItems(payload);

            db.TestItems.AddRange(testItems);

            await db.SaveChangesAsync();

            return Ok();

            var typeIds = (await db.Types.Select(x => x.ID).ToListAsync()).Select(x => (long?)x).ToList();

            var connectionString = db.Database.Connection.ConnectionString;
            var tableName = nameof(db.TestItems);

            var table = new System.Data.DataTable();

            table.Columns.AddRange(typeof(TestItem).GetProperties().Where(x => x.MemberType == System.Reflection.MemberTypes.Property).Select(x => new System.Data.DataColumn
            {
                ColumnName = x.Name
            }).ToArray());

            var price = payload.DataTemplate.Price;
            var step = 0;

            for (int i = 0; i < payload.DataCount; i++)
            {
                step++;

                var number = i + 1;

                if(payload.Increments.Step > 1)
                {
                    if (step == payload.Increments.Step)
                    {
                        step = 0;
                        price = price + payload.Increments.Step;
                    }
                }
                else
                {
                    price = payload.DataTemplate.Price + (i * payload.Increments.Price);
                }

                var row = table.NewRow();

                if (payload.ParentTestItemId.HasValue)
                    row[nameof(TestItem.ParentTestItemId)] = payload.ParentTestItemId;

                row[nameof(TestItem.Code)] = $"{payload.DataTemplate.Code} - {number}";
                row[nameof(TestItem.Title)] = $"{payload.DataTemplate.Title} - {number}";
                row[nameof(TestItem.Date)] = payload.DataTemplate.Date.AddDays(i * payload.Increments.Day).ToString("yyyy-MM-dd");
                row[nameof(TestItem.Price)] = price;
                row[nameof(TestItem.TypeId)] = typeIds.Count == 0 ? null : typeIds.ElementAt(i % 2);

                table.Rows.Add(row);
            }

            using (var connection = db.Database.Connection)
            {
                await connection.OpenAsync();

                using (var bulk = new BulkOperation(connection))
                {
                    bulk.DestinationTableName = tableName;
                    bulk.BatchSize = 5000;

                    await bulk.BulkInsertAsync(table);
                }
            }

            return Ok();
        }

        [HttpPost, Route("insert-types")]
        public async Task<IHttpActionResult> InsertTypes()
        {
            this.AssignDbType();

            var db = Utils.GetDBContext(this.DBType);

            var types = new Shared.Insert.Tools().GenerateTypes();

            db.Types.AddRange(types);

            await db.SaveChangesAsync();

            return Ok();
        }
    }
}