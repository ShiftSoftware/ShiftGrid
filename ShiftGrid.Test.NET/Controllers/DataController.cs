using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ShiftGrid.Core;

namespace ShiftGrid.Test.NET.Controllers
{
    [RoutePrefix("api")]
    public class DataController : ApiController
    {
        [HttpPost, Route("list-test-items")]
        public async Task<IHttpActionResult> ListItems(ShiftGrid.Core.Grid<Models.TestItemView> shiftGrid)
        {
            var db = Utils.GetDBContext();

            var logs = new List<string>();

            db.Database.Log = (s) => logs.Add(s);

            var grid =
                db.TestItems.Select(x => new Models.TestItemView
                {
                    ID = x.ID,
                    Title = x.Title,
                    TypeId = x.TypeId,
                    Type = x.Type.Name
                })
                .ToShiftGrid(shiftGrid);

            return Ok(new
            {
                //logs,
                shiftGrid
            });
        }
    }
}