using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using ShiftSoftware.ShiftGrid.Core;

namespace ShiftGrid.Test.NET.Controllers
{
    [RoutePrefix("api")]
    public class DataController : ApiController
    {
        [HttpPost, Route("list-test-items")]
        public async Task<IHttpActionResult> ListItems(GridConfig payload)
        {
            var db = Utils.GetDBContext();

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

            var grid = db.TestItems
                .Select(x => new
                {
                    ID = x.ID,
                    Price = x.Price * 100,
                })
                //.SelectSummary(x => new
                //{
                //    Count = x.Count(),
                //    TotalID = x.Sum(y => y.ID),
                //    TotalPrice = x.Sum(y => y.Price)
                //})
                .ToShiftGrid(payload);

            return Json(new
            {
                grid,
                logs = logs
            });
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