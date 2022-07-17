### Filtering
There are many ways of filtering. Here are some examples of it.
#### Equals
`Equals` can be used to get only one record with the given specification.
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
                           Field = nameof(Employee.ID),
                           Operator = GridFilterOperator.Equals,
                           Value = 1
                       }
                   }
                });

            //It's better to use nameof. When targetting fields in Filters and Columns.
            return Ok(shiftGrid);
        }
```
The above example (when using EF Core and SQL Server) generates an SQL like below
``` SQL 
SELECT [e].[ID], [e].[FirstName], [e].[LastName], [e].[Birthdate], [d].[Name] AS [Department]
FROM [Employees] AS [e]
LEFT JOIN [Departments] AS [d] ON [e].[DepartmentId] = [d].[ID]
WHERE [e].[ID] = CAST(1 AS bigint)
ORDER BY [e].[ID]
```
#### In
`In` can be used to get multiple records with the given specification.
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
                           Field = nameof(Employee.ID),
                           Operator = GridFilterOperator.In,
                           Value = new List<long> { 1, 4, 10 }
                       }
                   }
                });

            //It's better to use nameof. When targetting fields in Filters and Columns.
            return Ok(shiftGrid);
        }
```
The above example (when using EF Core and SQL Server) generates an SQL like below
``` SQL 
SELECT [e].[ID], [e].[FirstName], [e].[LastName], [e].[Birthdate], [d].[Name] AS [Department]
FROM [Employees] AS [e]
LEFT JOIN [Departments] AS [d] ON [e].[DepartmentId] = [d].[ID]
WHERE [e].[ID] IN (CAST(1 AS bigint), CAST(4 AS bigint), CAST(10 AS bigint))
ORDER BY [e].[ID]
```
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
The above example (when using EF Core and SQL Server) generates an SQL like below
``` SQL 
SELECT [e].[ID], [e].[FirstName], [e].[LastName], [e].[Birthdate], [d].[Name] AS [Department]
FROM [Employees] AS [e]
LEFT JOIN [Departments] AS [d] ON [e].[DepartmentId] = [d].[ID]
WHERE [e].[FirstName] LIKE N'First Name (1%'
ORDER BY [e].[ID]
```
