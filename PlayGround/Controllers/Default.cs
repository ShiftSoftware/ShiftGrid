using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShiftSoftware.ShiftGrid.Core;
using PlayGround.EF;


namespace PlayGround.Controllers
{
    [Route("api")]
    [ApiController]
    public class Default : ControllerBase
    {
        [HttpPost("insert-test-data")]
        public async Task<ActionResult> InsertTestData()
        {
            var db = new EF.DB();

            await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Employees");
            await db.Database.ExecuteSqlRawAsync("delete Departments");
            await db.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT (Departments, RESEED, 0)");

            var departments = new string[] { "IT", "Finance", "HR", "Sales", "Marketing", "Customer Support" };

            foreach (var department in departments)
            {
                db.Departments.Add(new EF.Department { Name = department });
            }

            await db.SaveChangesAsync();

            var departmentIndex = 1;

            for (int i = 0; i < 1000; i++)
            {
                db.Employees.Add(new EF.Employee
                {
                    FirstName = $"First Name ({i + 1})",
                    LastName = $"Last Name ({i + 1})",
                    DepartmentId = departmentIndex++,
                });

                if (departmentIndex > departments.Length)
                    departmentIndex = 1;
            }

            await db.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("basic")]
        public async Task<ActionResult> Basic([FromBody] GridConfig gridConfig)
        {
            var db = new DB();

            var shiftGrid =
                await db
                .Employees
                .ToShiftGridAsync("ID", SortDirection.Ascending, gridConfig);

            return Ok(shiftGrid);
        }
    }
}
