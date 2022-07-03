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
            // https://msdn.microsoft.com/en-us/library/ms176057(v=sql.120).aspx
            // Current identity value is set to the new_reseed_value. If no rows have been inserted into the table since the table
            // was created, or if all rows have been removed by using the TRUNCATE TABLE statement, the first row inserted after you
            // run DBCC CHECKIDENT uses new_reseed_value as the identity.If rows are present in the table, or if all rows have been
            // removed by using the DELETE statement, the next row inserted uses new_reseed_value +the current increment value.
            await db.Database.ExecuteSqlRawAsync("INSERT INTO Departments (Name) VALUES ('dep')");
            await db.Database.ExecuteSqlRawAsync("DELETE Departments");
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
