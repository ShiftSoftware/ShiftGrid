﻿using Microsoft.AspNetCore.Mvc;
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

            var birthdDate = new DateTime(1955, 1, 1);

            for (int i = 0; i < 1000; i++)
            {
                db.Employees.Add(new EF.Employee
                {
                    FirstName = $"First Name ({i + 1})",
                    LastName = $"Last Name ({i + 1})",
                    DepartmentId = departmentIndex++,
                    Birthdate = birthdDate,
                });

                if (departmentIndex > departments.Length)
                    departmentIndex = 1;

                birthdDate = birthdDate.AddDays(15);
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

        [HttpPost("aggregate")]
        public async Task<ActionResult> Aggregate([FromBody] GridConfig gridConfig)
        {
            var db = new DB();

            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;

            var shiftGrid =
                await db
                .Employees
                .Select(x => new
                {
                    x.ID,
                    x.FirstName,
                    x.Birthdate
                })
                .SelectAggregate(x => new
                {
                    Count = x.Count(),
                    OldestEmployeeBirthdate = x.Min(y => y.Birthdate),
                    YoungestEmployeeBirthdate = x.Max(y => y.Birthdate),
                    NumberOfEmployeesBetween30And40 = x.Count(y => DbF.DateDiffYear(y.Birthdate, DateTime.Now) >= 30 && DbF.DateDiffYear(y.Birthdate, DateTime.Now) <= 40)
                })
                .ToShiftGridAsync("ID", SortDirection.Ascending, gridConfig);

            return Ok(shiftGrid);
        }

        [HttpPost("columns")]
        public async Task<ActionResult> Columns()
        {
            var db = new DB();

            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;

            var shiftGrid =
                await db
                .Employees
                .Select(x => new
                {
                    x.ID,
                    x.FirstName,
                    x.LastName,
                    x.Birthdate,
                    Department = x.Department.Name
                })
                .ToShiftGridAsync("ID", SortDirection.Ascending, new GridConfig
                {
                    Columns = new List<GridColumn>
                    {
                        new GridColumn
                        {
                            Field = "FirstName",
                            Visible = false
                        },
                        new GridColumn
                        {
                            Field = "LastName",
                            Visible = false
                        }
                    }
                });


            //Hiding the department will also ommit the Join that's made to the Department table. This should also be shown clearly in the doc

            return Ok(shiftGrid);
        }

        [HttpPost("filters")]
        public async Task<ActionResult> Filters()
        {
            var db = new DB();

            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;

            var shiftGrid =
                await db
                .Employees
                .Select(x => new
                {
                    x.ID,
                    x.FirstName,
                    x.LastName,
                    x.Birthdate,
                    Department = x.Department.Name
                })
                .ToShiftGridAsync("ID", SortDirection.Ascending, new GridConfig
                {
                    Filters = new List<GridFilter> {
                       new GridFilter
                       {
                           Field = nameof(Employee.FirstName),
                           Operator = GridFilterOperator.StartsWith,
                           Value = "First Name (1"
                       }
                   }
                });

            //It's better to use nameof. When targetting fields in Filters and Columns.
            return Ok(shiftGrid);
        }
    }
}
