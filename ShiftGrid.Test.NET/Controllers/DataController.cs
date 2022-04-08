using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using ShiftSoftware.ShiftGrid.Core;

namespace ShiftGrid.Test.NET.Controllers
{
    [RoutePrefix("api")]
    public class DataController : ApiController
    {
        public System.Type DBType { get; set; }

        public DataController()
        {
            if (System.Web.HttpContext.Current.Request.Headers["database"] != null)
                this.AssignDB(System.Web.HttpContext.Current.Request.Headers["database"].ToString());
        }
        private void AssignDB(string db)
        {
            if (db == "SqlServer")
                this.DBType = typeof(EF.DB);

            else if (db == "MySql")
                this.DBType = typeof(EF.MySQLDb);
        }

        [HttpPost, Route("list-test-items")]
        public async Task<IHttpActionResult> ListItems(GridConfig payload)
        {
            var db = Utils.GetDBContext(this.DBType);

            var logs = new List<string>();

            SetupLogger(db, logs);

            //var grid = await db.TestItems
            //    .Select(x => new Models.TestItemView
            //    {
            //        ID = x.ID,
            //        CalculatedPrice = x.Price * 100,
            //        Title = x.Title,
            //        TypeId = x.TypeId,
            //        Type = x.Type.Name,
            //        Items = x.ChildTestItems.Select(y => new Models.SubTestItemView
            //        {
            //            Title = y.Title
            //        })
            //    })
            //    .SelectSummary(x => new SummaryModel
            //    {
            //        DataCount = x.Count(),
            //        TotalID = x.Sum(y => y.ID),
            //        TotalPrice = x.Sum(y => y.CalculatedPrice)
            //    })
            //    .ToShiftGridAsync(payload);

            var grid = await db.TestItems
                .Select(x => new Models.TestItemView
                {
                    ID = x.ID,
                    CalculatedPrice = x.Price,
                    Title = x.Title,
                })
                .SelectSummary(x => new
                {
                    Count = x.Count(),
                    TotalID = x.Sum(y => y.ID),
                    TotalPrice = x.Sum(y => y.CalculatedPrice)
                })
                .ToShiftGridAsync(new GridSort
                {
                    Field = nameof(Models.TestItem.ID),
                    SortDirection = SortDirection.Ascending
                }, payload);

            return Json(new
            {
                grid,
                logs = logs
            });
        }

        [HttpPost, Route("export-data/{mode}")]
        public async Task<IHttpActionResult> ExportData(string mode, [FromBody] GridConfig payload)
        {
            var db = Utils.GetDBContext(this.DBType);

            var logs = new List<string>();

            SetupLogger(db, logs);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var grid = await db.TestItems
                .AsNoTracking()
                .Select(x => new Models.TestItemView
                {
                    ID = x.ID,
                    CalculatedPrice = x.Price,
                    Type = x.Type.Name,
                    Title = x.Title,
                })
                .SelectSummary(x => new
                {
                    Count = x.Count(),
                    TotalID = x.Sum(y => y.ID),
                    TotalPrice = x.Sum(y => y.CalculatedPrice)
                })
                .ToShiftGridAsync(new GridSort
                {
                    Field = "ID",
                    SortDirection = SortDirection.Ascending
                }, payload);

            stopWatch.Stop();

            var ts = stopWatch.Elapsed;

            var loadTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);

            stopWatch.Restart();

            if (mode == "download")
            {
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(grid.ToCSVStream().GetBuffer())
                };

                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "Detailed Order History.csv"
                };

                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                var response = ResponseMessage(result);

                return response;
            }

            else if (mode == "save")
            {
                stopWatch.Start();

                grid.SaveCSV(System.Web.Hosting.HostingEnvironment.MapPath("/exported.csv"));

                stopWatch.Stop();

                var ts2 = stopWatch.Elapsed;

                // Format and display the TimeSpan value.
                var csvTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts2.Hours, ts2.Minutes, ts2.Seconds,
                    ts2.Milliseconds / 10);

                return Ok(new
                {
                    CsvTime = csvTime,
                    LoadTime = loadTime,
                    DataCount = string.Format("{0:#,0}", grid.Data.Count),
                    Logs = logs
                });
            }

            return Ok();
        }

        public static void SetupLogger(DbContext db, List<string> logs)
        {
            db.Database.Log = System.Console.Write;

            db.Database.Log = (s) =>
            {
                if (!(s.StartsWith("Opened connection") || s.StartsWith("Closed") || s.StartsWith("--")))
                {
                    if (!string.IsNullOrWhiteSpace(s))
                        logs.Add(s.Replace("\r", "").Replace("\n", ""));

                    System.Diagnostics.Debug.WriteLine(s);
                }
            };
        }
    }
}