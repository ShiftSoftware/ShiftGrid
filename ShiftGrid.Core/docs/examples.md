### Filtering
There are many ways of filtering. Here are some examples of it.
#### Starts With
`StartsWith` can be used to filter the data depending of what value the data begins with.
``` C#
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
```
