### Filtering
There are many ways of filtering, an object of [`GridFilter`](/reference/#gridfilter) is required. if multiple [`GridFilter`](/reference/#gridfilter) is given the filters will be AND ed.
The follwings are some exmaples.
#### Equals
`Equals` can be used to get only one record with the given specification.
=== "C#"
    ``` C# hl_lines="21 22 23 24 25 26 27 28"
    [HttpPost("filters_equals")]
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
    ``` JSON hl_lines="10 11 12 13 14 15 16"
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
=== "Response (Omitted)"
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
        "sort": [...],
        "stableSort": {...},
        "filters": [...],
        "columns": [...],
        "pagination": {...},
        "beforeLoadingData": "2022-07-17T08:54:52.9137933Z",
        "afterLoadingData": "2022-07-17T08:54:52.9196707Z"
    }
    ```
=== "Response (Full)"
    ``` JSON hl_lines="25 26 27 28 29 30 31 32"
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
=== "Response (Omitted)"
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
    "sort": [...],
    "stableSort": {...},
    "filters": [...],
    "columns": [...],
    "pagination": {...},
    "beforeLoadingData": "2022-07-17T09:15:15.9393452Z",
    "afterLoadingData": "2022-07-17T09:15:15.9884659Z"
    }
    ```
=== "Response (Full)"
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

#### Filtering subitems

=== "C#"
    ``` C#
        [HttpPost("filters_subitems")]
        public async Task<ActionResult> Filters_SubItems()
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
                    x.Department
                })
                .ToShiftGridAsync("ID", SortDirection.Ascending, new GridConfig
                {
                    Filters = new List<GridFilter> {
                       new GridFilter
                       {
                           Field = "Department.Name",
                           Operator = GridFilterOperator.Equals,
                           Value = "IT"
                       }
                   }
                });

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
=== "Response (Omitted)"
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
    "sort": [...],
    "stableSort": {...},
    "filters": [...],
    "columns": [...],
    "pagination": {...},
    "beforeLoadingData": "2022-07-17T09:15:15.9393452Z",
    "afterLoadingData": "2022-07-17T09:15:15.9884659Z"
    }
    ```
=== "Response (Full)"
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

### Grid Column exclusion
A column can be excluded in the generated SQL query using the [`GridColumn`](/reference/#gridcolumn) object when visible is set to false, if the column comes from a table join, the join will be omitted

=== "C#"
    ``` C#
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
                            Field = "Department",
                            Visible = false
                        }
                    }
                });
            return Ok(shiftGrid);
        }
    ```
=== "SQL"
    ``` SQL 
    SELECT [e].[ID], NULL AS [FirstName], [e].[LastName], [e].[Birthdate]
    FROM [Employees] AS [e]
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
        "columns": [
            {
            "field":"FirstName",
            "visible":false
            },
            {
            "field":"Department",
            "visible":false
            }
        
        ],
        "pagination": {
            "pageSize": 10
        },
        "filters": [
        ]
    }
    ```
=== "Response (Omitted)"
    ``` JSON
    {
        "dataPageIndex": 0,
        "dataPageSize": 5,
        "dataCount": 1000,
        "data": [
            {
                "id": 1,
                "firstName": null,
                "lastName": "Last Name (1)",
                "birthdate": "1955-01-01T00:00:00",
                "department": null
            },
            ...
        ],
        "aggregate": null,
        "sort": [...],
        "stableSort": {...},
        "filters": [],
        "columns": [...],
        "pagination": {...},
        "beforeLoadingData": "2022-07-17T13:35:54.5310457Z",
        "afterLoadingData": "2022-07-17T13:35:54.5351803Z"
    }
    ```
=== "Response (Full)"
    ``` JSON
    {
        "dataPageIndex": 0,
        "dataPageSize": 5,
        "dataCount": 1000,
        "data": [
            {
                "id": 1,
                "firstName": null,
                "lastName": "Last Name (1)",
                "birthdate": "1955-01-01T00:00:00",
                "department": null
            },
            {
                "id": 2,
                "firstName": null,
                "lastName": "Last Name (2)",
                "birthdate": "1955-01-16T00:00:00",
                "department": null
            },
            {
                "id": 3,
                "firstName": null,
                "lastName": "Last Name (3)",
                "birthdate": "1955-01-31T00:00:00",
                "department": null
            },
            {
                "id": 4,
                "firstName": null,
                "lastName": "Last Name (4)",
                "birthdate": "1955-02-15T00:00:00",
                "department": null
            },
            {
                "id": 5,
                "firstName": null,
                "lastName": "Last Name (5)",
                "birthdate": "1955-03-02T00:00:00",
                "department": null
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
        "filters": [],
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
                "visible": false,
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
                "visible": false,
                "order": 4
            }
        ],
        "pagination": {
            "count": 200,
            "pageSize": 10,
            "pageStart": 0,
            "pageEnd": 9,
            "pageIndex": 0,
            "hasPreviousPage": false,
            "hasNextPage": true,
            "lastPageIndex": 199,
            "dataStart": 1,
            "dataEnd": 5
        },
        "beforeLoadingData": "2022-07-17T13:47:40.8863074Z",
        "afterLoadingData": "2022-07-17T13:47:40.8881376Z"
    }
    ```
???+ warning
    the `select` method must exist for the exclusion to be done successfully
???+ info
    if the excluded column is the same as the one used for sorting its value will be 0 not null, in our exampe if ID is excluded it will be 0.
#### Excluding Collections
Collections can be excluded, if the collection comes from table join the join will be omitted too.

=== "C#"
    ``` C#
    [HttpPost("columns-exclude-collections")]
    public async Task<ActionResult> Columns_Exclude_Collections()
    {
        var db = new DB();

        var shiftGrid =
            await db
            .Departments
            .Select(x => new
            {
                x.ID,
                x.Name,
                Employee = x.Employees,
            })
            .ToShiftGridAsync("ID", SortDirection.Ascending, new GridConfig
            {
                Columns = new List<GridColumn>
                {
                    new GridColumn
                    {
                        Field = "Employee",
                        Visible = false
                    },
                }
            });

        return Ok(shiftGrid);
    }
    ```
=== "SQL"
    ``` SQL 
    SELECT [d].[ID], [d].[Name]
    FROM [Departments] AS [d]
    ORDER BY [d].[ID]
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
        "columns": [
            {
                "field": "Employee",
                "visible": false
            }
        ],
        "pagination": {
            "pageSize": 10
        }
    }
    ```
=== "Response (Omitted)"
    ``` JSON
    {
    "dataPageIndex": 0,
    "dataPageSize": 5,
    "dataCount": 6,
    "data": [
        {
            "id": 1,
            "name": "IT",
            "employee": null
        },
        {
            "id": 2,
            "name": "Finance",
            "employee": null
        },
        {
            "id": 3,
            "name": "HR",
            "employee": null
        },
        {
            "id": 4,
            "name": "Sales",
            "employee": null
        },
        {
            "id": 5,
            "name": "Marketing",
            "employee": null
        }
    ],
    "aggregate": null,
    "sort": [...],
    "stableSort": {...},
    "filters": [],
    "columns": [...],
    "pagination": {...},
    "beforeLoadingData": "2022-07-18T18:46:12.2480272Z",
    "afterLoadingData": "2022-07-18T18:46:12.2493327Z"
    }
    ```
=== "Response (Full)"
    ``` JSON
    {
    "dataPageIndex": 0,
    "dataPageSize": 5,
    "dataCount": 6,
    "data": [
        {
            "id": 1,
            "name": "IT",
            "employee": null
        },
        {
            "id": 2,
            "name": "Finance",
            "employee": null
        },
        {
            "id": 3,
            "name": "HR",
            "employee": null
        },
        {
            "id": 4,
            "name": "Sales",
            "employee": null
        },
        {
            "id": 5,
            "name": "Marketing",
            "employee": null
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
    "filters": [],
    "columns": [
        {
        "headerText": "ID",
        "field": "ID",
        "visible": true,
        "order": 0
        },
        {
        "headerText": "Name",
        "field": "Name",
        "visible": true,
        "order": 1
        },
        {
        "headerText": "Employee",
        "field": "Employee",
        "visible": false,
        "order": 2
        }
        ],
        "pagination": {
            "count": 2,
            "pageSize": 10,
            "pageStart": 0,
            "pageEnd": 1,
            "pageIndex": 0,
            "hasPreviousPage": false,
            "hasNextPage": false,
            "lastPageIndex": 1,
            "dataStart": 1,
            "dataEnd": 5
        },
        "beforeLoadingData": "2022-07-18T18:46:12.2480272Z",
        "afterLoadingData": "2022-07-18T18:46:12.2493327Z"
    }
    ```
#### Excluding identical columns
Two identical column, one is a normal column and the other one is inside the collection.

=== "C#"
    ``` C#
    [HttpPost("columns-exclude-identical")]
    public async Task<ActionResult> Columns_Exclude_Identical()
        {
            var db = new DB();

            var shiftGrid =
                await db
                .Departments
                .Select(x => new
                {
                    x.ID,
                    x.Name,
                    Employee = x.Employees.Select(y => new
                    {
                       Department =  y.Department.Name
                    }),
                })
                .ToShiftGridAsync("ID", SortDirection.Ascending, new GridConfig
                {
                    Columns = new List<GridColumn>
                    {
                        new GridColumn
                        {
                            Field = "Name",
                            Visible = false 
                        },
                    }
                });

            return Ok(shiftGrid);
        }
    ```
=== "SQL"
    ``` SQL 
    SELECT [d].[ID], [t].[Department], [t].[ID], [t].[ID0]
    FROM [Departments] AS [d]
    LEFT JOIN (
                SELECT [d0].[Name] AS [Department], [e].[ID], [d0].[ID] AS [ID0], [e].[DepartmentId]
                FROM [Employees] AS [e]
                LEFT JOIN [Departments] AS [d0] ON [e].[DepartmentId] = [d0].[ID]
                ) AS [t] ON [d].[ID] = [t].[DepartmentId]
    ORDER BY [d].[ID], [t].[ID]
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
        "columns": [
            {
                "field": "Department",
                "visible": false
            }
        ],
        "pagination": {
            "pageSize": 10
        }
    }
    ```
=== "Response (Omitted)"
    ``` JSON
    {
    "dataPageIndex": 0,
    "dataPageSize": 20,
    "dataCount": 6,
    "data": [
        {
            "id": 1,
            "name": null,
            "employee": [
                {
                    "department": "IT"
                },
                ...
            ]
        },
        ...
    ],
    "aggregate": null,
    "sort": [...],
    "stableSort": {...},
    "filters": [],
    "columns": [...],
    "pagination": {...},
    "beforeLoadingData": "2022-07-18T19:24:05.3233637Z",
    "afterLoadingData": "2022-07-18T19:24:05.4283594Z"
    }
    ```
=== "Response (Full)"
    ``` JSON
    {
    "dataPageIndex": 0,
    "dataPageSize": 20,
    "dataCount": 6,
    "data": [
        {
            "id": 1,
            "name": null,
            "employee": [
                {
                    "department": "IT"
                },
                ...
            ]
        },
        {
            "id": 2,
            "name": null,
            "employee": [
                {
                    "department": "Finance"
                },
                ...
            ]
        },
        {
            "id": 3,
            "name": null,
            "employee": [
                {
                    "department": "HR"
                },
                ...
            ]
        },
        {
            "id": 4,
            "name": null,
            "employee": [
                {
                    "department": "Sales"
                },
                ...
            ]
        },
        {
            "id": 5,
            "name": null,
            "employee": [
                {
                    "department": "Marketing"
                },
                ...
            ]
        },
        {
            "id": 6,
            "name": null,
            "employee": [
                {
                    "department": "Customer Support"
                },
                ...
            ]
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
    "filters": [],
    "columns": [
        {
            "headerText": "ID",
            "field": "ID",
            "visible": true,
            "order": 0
        },
        {
            "headerText": "Name",
            "field": "Name",
            "visible": false,
            "order": 1
        },
        {
            "headerText": "Employee",
            "field": "Employee",
            "visible": true,
            "order": 2
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
        "dataEnd": 6
    },
    "beforeLoadingData": "2022-07-18T19:24:05.3233637Z",
    "afterLoadingData": "2022-07-18T19:24:05.4283594Z"
    }
    ```
#### Excluding Annonymous Object

=== "C#"
    ``` C#
    [HttpPost("columns-exclude-annynomous-object")]
    public async Task<ActionResult> Columns_Exclude_AnnonymousObject()
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
                FullName = x.FirstName + x.LastName,
                x.Birthdate,
                x.DepartmentId
            })
            .ToShiftGridAsync("ID", SortDirection.Ascending, new GridConfig
            {
                Columns = new List<GridColumn>
                {
                    new GridColumn
                    {
                        Field = "FullName",
                        Visible = false
                    },
                }
            });
        return Ok(shiftGrid);
    }
    ```
=== "SQL"
    ``` SQL 
    SELECT [e].[ID], [e].[FirstName], [e].[LastName], NULL AS [FullName], [e].[Birthdate], [e].[DepartmentId]
    FROM [Employees] AS [e]
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
        "columns": [
            {
                "field": "FullName",
                "visible": false
            }
        ],
        "pagination": {
            "pageSize": 10
        }
    }
    ```
=== "Response (Omitted)"
    ``` JSON
    {
    "dataPageIndex": 0,
    "dataPageSize": 20,
    "dataCount": 1000,
    "data": [
        {
            "id": 1,
            "firstName": "First Name (1)",
            "lastName": "Last Name (1)",
            "fullName": null,
            "birthdate": "1955-01-01T00:00:00",
            "departmentId": 1
        },
        ...
    ],
    "aggregate": null,
    "sort": [...],
    "stableSort": {
        "field": "ID",
        "sortDirection": 0
    },
    "filters": [],
    "columns": [...],
    "pagination": {...},
    "beforeLoadingData": "2022-07-18T19:48:09.0032974Z",
    "afterLoadingData": "2022-07-18T19:48:09.1028997Z"
    }
    ```
=== "Response (Full)"
    ``` JSON
    {
    "dataPageIndex": 0,
    "dataPageSize": 20,
    "dataCount": 1000,
    "data": [
        {
            "id": 1,
            "firstName": "First Name (1)",
            "lastName": "Last Name (1)",
            "fullName": null,
            "birthdate": "1955-01-01T00:00:00",
            "departmentId": 1
        },
        ...
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
    "filters": [],
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
            "headerText": "FullName",
            "field": "FullName",
            "visible": false,
            "order": 3
        },
        {
            "headerText": "Birthdate",
            "field": "Birthdate",
            "visible": true,
            "order": 4
        },
        {
            "headerText": "DepartmentId",
            "field": "DepartmentId",
            "visible": true,
            "order": 5
        }
    ],
    "pagination": {
        "count": 50,
        "pageSize": 10,
        "pageStart": 0,
        "pageEnd": 9,
        "pageIndex": 0,
        "hasPreviousPage": false,
        "hasNextPage": true,
        "lastPageIndex": 49,
        "dataStart": 1,
        "dataEnd": 20
    },
    "beforeLoadingData": "2022-07-18T19:48:09.0032974Z",
    "afterLoadingData": "2022-07-18T19:48:09.1028997Z"
    }
    ```
#### Excluding a column that is in summary

=== "C#"
    ``` C#
    [HttpPost("columns-exclude-field-in-summary")]
    public async Task<ActionResult> Columns_Exclude_AFieldThatsInSummary()
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
                x.DepartmentId
            })
            .SelectAggregate(x => new
            {
                Count = x.Count(),
                TotalDepartmentId = x.Sum(y => y.DepartmentId),
                MaxDepartmentId = x.Max(y => y.DepartmentId)
            })
            .ToShiftGridAsync("ID", SortDirection.Ascending, new GridConfig
            {
                Columns = new List<GridColumn>
                {
                    new GridColumn
                    {
                        Field = "DepartmentId",
                        Visible = false
                    },
                }
            });
        return Ok(shiftGrid);
    }
    ```
=== "SQL"
    ``` SQL 
    SELECT [e].[ID], [e].[FirstName], [e].[LastName], [e].[Birthdate]
    FROM [Employees] AS [e]
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
        "columns": [
            {
                "field": "DepartmentId",
                "visible": false
            }
        ],
        "pagination": {
            "pageSize": 10
        }
    }
    ```
=== "Response (Omitted)"
    ``` JSON
    {
    "dataPageIndex": 0,
    "dataPageSize": 20,
    "dataCount": 1000,
    "data": [
        {
            "id": 1,
            "firstName": "First Name (1)",
            "lastName": "Last Name (1)",
            "birthdate": "1955-01-01T00:00:00",
            "departmentId": null
        },
        ...
    ],
    "aggregate": {
        "count": 1000,
        "totalDepartmentId": null,
        "maxDepartmentId": null
    },
    "sort": [...],
    "stableSort": {...},
    "filters": [],
    "columns": [...],
    "pagination": {...},
    "beforeLoadingData": "2022-07-18T20:04:43.3240903Z",
    "afterLoadingData": "2022-07-18T20:04:43.3687013Z"
    }
    ```
=== "Response (Full)"
    ``` JSON
    {
    "dataPageIndex": 0,
    "dataPageSize": 20,
    "dataCount": 1000,
    "data": [
        {
            "id": 1,
            "firstName": "First Name (1)",
            "lastName": "Last Name (1)",
            "birthdate": "1955-01-01T00:00:00",
            "departmentId": null
        },
        ...
    ],
    "aggregate": {
        "count": 1000,
        "totalDepartmentId": null,
        "maxDepartmentId": null
    },
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
    "filters": [],
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
            "headerText": "DepartmentId",
            "field": "DepartmentId",
            "visible": false,
            "order": 4
        }
    ],
    "pagination": {
        "count": 50,
        "pageSize": 10,
        "pageStart": 0,
        "pageEnd": 9,
        "pageIndex": 0,
        "hasPreviousPage": false,
        "hasNextPage": true,
        "lastPageIndex": 49,
        "dataStart": 1,
        "dataEnd": 20
    },
    "beforeLoadingData": "2022-07-18T20:04:43.3240903Z",
    "afterLoadingData": "2022-07-18T20:04:43.3687013Z"
    }
    ```
#### Excluding a column that is in summary using a model

=== "C#"
    ``` C#
    class SummaryModel
    {
        public int Count { get; set; }
        public long? TotalDepartmentId { get; set; }
        public long? MaxDepartmentId { get; set; }
    }
    [HttpPost("columns-exclude-field-in-summary-using-a-model")]
    public async Task<ActionResult> Columns_Exclude_AFieldThatsInSummary_UsingAModel()
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
                x.DepartmentId
            })
            .SelectAggregate(x => new SummaryModel
            {
                Count = x.Count(),
                TotalDepartmentId = x.Sum(y => y.DepartmentId),
                MaxDepartmentId = x.Max(y => y.DepartmentId)
            })
            .ToShiftGridAsync("ID", SortDirection.Ascending, new GridConfig
            {
                Columns = new List<GridColumn>
                {
                    new GridColumn
                    {
                        Field = "DepartmentId",
                        Visible = false
                    },
                }
            });
        return Ok(shiftGrid);
    }
    ```
=== "SQL"
    ``` SQL 
    SELECT [e].[ID], [e].[FirstName], [e].[LastName], [e].[Birthdate]
    FROM [Employees] AS [e]
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
    "columns": [
        {
            "field": "DepartmentId",
            "visible": false
        }
    ],
    "pagination": {
        "pageSize": 10
    }
    }
    ```
=== "Response (Omitted)"
    ``` JSON
    {
    "dataPageIndex": 0,
    "dataPageSize": 20,
    "dataCount": 1000,
    "data": [
        {
            "id": 1,
            "firstName": "First Name (1)",
            "lastName": "Last Name (1)",
            "birthdate": "1955-01-01T00:00:00",
            "departmentId": null
        },
        ...
    ],
    "aggregate": {
        "count": 1000,
        "totalDepartmentId": null,
        "maxDepartmentId": null
    },
    "sort": [...],
    "stableSort": {
        "field": "ID",
        "sortDirection": 0
    },
    "filters": [],
    "columns": [...],
    "pagination": {...},
    "beforeLoadingData": "2022-07-19T11:07:58.1705378Z",
    "afterLoadingData": "2022-07-19T11:07:58.2177103Z"
    }
    ```
=== "Response (Full)"
    ``` JSON
    {
    "dataPageIndex": 0,
    "dataPageSize": 20,
    "dataCount": 1000,
    "data": [
        {
            "id": 1,
            "firstName": "First Name (1)",
            "lastName": "Last Name (1)",
            "birthdate": "1955-01-01T00:00:00",
            "departmentId": null
        },
        ...
    ],
    "aggregate": {
        "count": 1000,
        "totalDepartmentId": null,
        "maxDepartmentId": null
    },
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
    "filters": [],
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
            "headerText": "DepartmentId",
            "field": "DepartmentId",
            "visible": false,
            "order": 4
        }
    ],
    "pagination": {
        "count": 50,
        "pageSize": 10,
        "pageStart": 0,
        "pageEnd": 9,
        "pageIndex": 0,
        "hasPreviousPage": false,
        "hasNextPage": true,
        "lastPageIndex": 49,
        "dataStart": 1,
        "dataEnd": 20
    },
    "beforeLoadingData": "2022-07-19T11:07:58.1705378Z",
    "afterLoadingData": "2022-07-19T11:07:58.2177103Z"
    }
    ```
