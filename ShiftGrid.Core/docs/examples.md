### Filtering
There are many ways of filtering, for filtering an object of [`GridFilter`](/reference/#gridfilter) is required. Here are some examples of it.
#### Equals
`Equals` can be used to get only one record with the given specification.
=== "C#"
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
=== "SQL"
    ``` SQL 
    SELECT [e].[ID], [e].[FirstName], [e].[LastName], [e].[Birthdate], [d].[Name] AS [Department]
    FROM [Employees] AS [e]
    LEFT JOIN [Departments] AS [d] ON [e].[DepartmentId] = [d].[ID]
    WHERE [e].[ID] = CAST(1 AS bigint)
    ORDER BY [e].[ID]
    ```
=== "Request"
    ``` JSON
    {
        "dataPageIndex": 0,
        "dataPageSize": 5,
        "sort": [
            {
                "field": "ID",
                "sortDirection": 0
            }
        ],
        "filters": [
            {
            "field": "ID",
            "operator": "=",
            "value" : 1
            }
        ],
        "columns": [],
        "pagination": {
            "pageSize": 10
        }
    }
    ```
=== "Response"
    ``` JSON
    {
        "dataPageIndex": 0,
        "dataPageSize": 20,
        "dataCount": 1,
        "data": [
            {
                "id": 1,
                "firstName": "First Name (1)",
                "lastName": "Last Name (1)",
                "birthdate": "1955-01-01T00:00:00",
                "department": "IT"
            }
        ],
        "aggregate": null,
        "sort": [
            {
                "field": "ID",
                "sortDirection": 0
            }
        ],
        "stableSort": {
            "field": "ID",
            "sortDirection": 0
        },
        "filters": [
            {
                "field": "ID",
                "operator": "=",
                "value": 1,
                "or": null
            }
        ],
        "columns": [
            {
                "headerText": "ID",
                "field": "ID",
                "visible": true,
                "order": 0
            },
            {
                "headerText": "FirstName",
                "field": "FirstName",
                "visible": true,
                "order": 1
            },
            {
                "headerText": "LastName",
                "field": "LastName",
                "visible": true,
                "order": 2
            },
            {
                "headerText": "Birthdate",
                "field": "Birthdate",
                "visible": true,
                "order": 3
            },
            {
                "headerText": "Department",
                "field": "Department",
                "visible": true,
                "order": 4
            }
        ],
        "pagination": {
            "count": 1,
            "pageSize": 10,
            "pageStart": 0,
            "pageEnd": 0,
            "pageIndex": 0,
            "hasPreviousPage": false,
            "hasNextPage": false,
            "lastPageIndex": 0,
            "dataStart": 1,
            "dataEnd": 1
        },
        "beforeLoadingData": "2022-07-17T08:54:52.9137933Z",
        "afterLoadingData": "2022-07-17T08:54:52.9196707Z"
    }
    ```
#### Or
`Or` can be used for multiple filtering options that will be Or ed with the current filtering options.
=== "C#"
    ``` C#
    [HttpPost("filters_or")]
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
                     .ToShiftGridAsync("ID", SortDirection.Ascending, new GridConfig {
                       Filters = new List<GridFilter> {
                           new GridFilter{
                               Field = nameof(Employee.ID),
                               Operator = GridFilterOperator.Equals,
                               Value = "1",
                               OR = new List<GridFilter> {
                                   new GridFilter
                                   {
                                       Field = nameof(Employee.ID),
                                       Operator = GridFilterOperator.Equals,
                                       Value = "5"
                                   },
                                   new GridFilter
                                   {
                                       Field = nameof(Employee.ID),
                                       Operator = GridFilterOperator.Equals,
                                       Value = "12"
                                   }
                               }
                           }
                    }
                    });

                //It's better to use nameof. When targetting fields in Filters and Columns.
                return Ok(shiftGrid);
            }
    ```
=== "SQL"
    ``` SQL 
    SELECT [e].[ID], [e].[FirstName], [e].[LastName], [e].[Birthdate], [d].[Name] AS [Department]
    FROM [Employees] AS [e]
    LEFT JOIN [Departments] AS [d] ON [e].[DepartmentId] = [d].[ID]
    WHERE [e].[ID] IN (CAST(1 AS bigint), CAST(5 AS bigint), CAST(12 AS bigint))
    ORDER BY [e].[ID]
    ```
=== "Request"
    ``` JSON
    {
    "dataPageIndex": 0,
    "dataPageSize": 5,
    "sort": [
        {
            "field": "ID",
            "sortDirection": 0
        }
    ],
    "filters": [
        {
        "field": "ID",
        "operator": "=",
        "value" : 1,
        "or": [
            {
            "field": "ID",
            "operator": "=",
            "value" : 5
            },
            {
            "field": "ID",
            "operator": "=",
            "value" : 12
            }
        ]
        }
        ],
        "columns": [],
        "pagination": {
            "pageSize": 10
        }
    }
    ```
=== "Response"
    ``` JSON
    {
    "dataPageIndex": 0,
    "dataPageSize": 20,
    "dataCount": 3,
    "data": [
        {
            "id": 1,
            "firstName": "First Name (1)",
            "lastName": "Last Name (1)",
            "birthdate": "1955-01-01T00:00:00",
            "department": "IT"
        },
        {
            "id": 5,
            "firstName": "First Name (5)",
            "lastName": "Last Name (5)",
            "birthdate": "1955-03-02T00:00:00",
            "department": "Marketing"
        },
        {
            "id": 12,
            "firstName": "First Name (12)",
            "lastName": "Last Name (12)",
            "birthdate": "1955-06-15T00:00:00",
            "department": "Customer Support"
        }
    ],
    "aggregate": null,
    "sort": [
        {
            "field": "ID",
            "sortDirection": 0
        }
    ],
    "stableSort": {
        "field": "ID",
        "sortDirection": 0
    },
    "filters": [
        {
            "field": "ID",
            "operator": "=",
            "value": "1",
            "or": [
                {
                    "field": "ID",
                    "operator": "=",
                    "value": "5",
                    "or": null
                },
                {
                    "field": "ID",
                    "operator": "=",
                    "value": "12",
                    "or": null
                }
            ]
        }
    ],
    "columns": [
        {
            "headerText": "ID",
            "field": "ID",
            "visible": true,
            "order": 0
        },
        {
            "headerText": "FirstName",
            "field": "FirstName",
            "visible": true,
            "order": 1
        },
        {
            "headerText": "LastName",
            "field": "LastName",
            "visible": true,
            "order": 2
        },
        {
            "headerText": "Birthdate",
            "field": "Birthdate",
            "visible": true,
            "order": 3
        },
        {
            "headerText": "Department",
            "field": "Department",
            "visible": true,
            "order": 4
        }
    ],
    "pagination": {
        "count": 1,
        "pageSize": 10,
        "pageStart": 0,
        "pageEnd": 0,
        "pageIndex": 0,
        "hasPreviousPage": false,
        "hasNextPage": false,
        "lastPageIndex": 0,
        "dataStart": 1,
        "dataEnd": 3
    },
    "beforeLoadingData": "2022-07-17T09:15:15.9393452Z",
    "afterLoadingData": "2022-07-17T09:15:15.9884659Z"
    }
    ```
???+ note

    Using multiple filters while the filtering operator is equals as the same as using `IN` operator for filtering multiple values
