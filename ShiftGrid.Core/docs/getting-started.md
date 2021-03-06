This guide will get you started quickly. Then dive into the docs to learn more.

### Installation

The Core package ```ShiftSoftware.ShiftGrid.Core``` is available on [nuget.org](https://www.nuget.org/packages/ShiftSoftware.ShiftGrid.Core).

=== "Package Manager Console"
    ```
    Install-Package ShiftSoftware.ShiftGrid.Core
    ```

=== "Nuget Package Manager (UI)"
    !!! note ""
        * **Right Click** on the Solution Name In Visual Studio
        * Click **Manage NuGet Packages**
        * Click **Browse**
        * Find ```ShiftSoftware.ShiftGrid.Core``` and Install it.


### Usage (Import)

**Shift Grid** provides extension methods to ```IQueryable```. You need to import it via the ```using``` statement to access the extension methods.
``` C#
using ShiftSoftware.ShiftGrid.Core;
```

### Basic Example

This example shows the Shift Grid being used in a Web API.

!!! info ""
    === "C# / Endpoint"
        ``` C#
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
        ```
        &nbsp;
        This Endpoint accepts a GridConfig from the client. The GridConfig can control the Pagination, Sorting, Filtering of data.
        <br/>
        <br/>
    === "Request Body (POST)"
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
            "columns": [],
            "pagination": {
                "pageSize": 10
            }
        }
        ```
        The ``dataPageIndex`` is set to ``0`` to load the first page. and the ``dataPageSize`` is set to ``5`` to load 5 items in each page.
        <br/>
    === "Response (Omitted)"
        ``` JSON
        {
            "dataPageIndex": 0,
            "dataPageSize": 5,
            "dataCount": 1000,
            "data": [...],
            "aggregate": null,
            "sort": [...],
            "stableSort": {...},
            "filters": [],
            "columns": [...],
            "pagination": {...},
            "beforeLoadingData": "2022-07-13T07:31:05.8462685Z",
            "afterLoadingData": "2022-07-13T07:31:05.8474664Z"
        }
        ```
        Objects and Arrays are omitted here so you can see the entire JSON in one preview. See the next tab for the full response.
    === "Response (Full)"
        ``` JSON
        {
            "dataPageIndex": 0,
            "dataPageSize": 5,
            "dataCount": 1000,
            "data": [
                {
                    "id": 1,
                    "firstName": "First Name (1)",
                    "lastName": "Last Name (1)",
                    "birthdate": null,
                    "departmentId": 1,
                    "department": null
                },
                {
                    "id": 2,
                    "firstName": "First Name (2)",
                    "lastName": "Last Name (2)",
                    "birthdate": null,
                    "departmentId": 2,
                    "department": null
                },
                {
                    "id": 3,
                    "firstName": "First Name (3)",
                    "lastName": "Last Name (3)",
                    "birthdate": null,
                    "departmentId": 3,
                    "department": null
                },
                {
                    "id": 4,
                    "firstName": "First Name (4)",
                    "lastName": "Last Name (4)",
                    "birthdate": null,
                    "departmentId": 4,
                    "department": null
                },
                {
                    "id": 5,
                    "firstName": "First Name (5)",
                    "lastName": "Last Name (5)",
                    "birthdate": null,
                    "departmentId": 5,
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
                    "visible": true,
                    "order": 4
                },
                {
                    "headerText": "Department",
                    "field": "Department",
                    "visible": true,
                    "order": 5
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
            "beforeLoadingData": "2022-07-13T07:31:05.8462685Z",
            "afterLoadingData": "2022-07-13T07:31:05.8474664Z"
        }
        ```
        &nbsp;