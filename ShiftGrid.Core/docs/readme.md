# ShiftSoftware.ShiftGrid.Core
Data Grid adapter for Entity Framework and Entity Framework Core with support for Pagination, Sorting, Filtering, and Data Exporting.

# Usage
Web Controller Example:
``` C#
[HttpPost, Route("users")]
public async Task<IHttpActionResult> Users([FromBody] GridConfig config)
{
    var shiftGrid = await db.Users
        .Select(x => new
        {
            ID = x.UserId,
            x.Name,
            x.Email
        })
        .ToShiftGridAsync("ID", SortDirection.Ascending, config);

    return Ok(shiftGrid);
}
```

where the request payload is the __GridConfig__ in JSON
``` JSON
{
    "DataPageIndex": 0,
    "DataPageSize": 5,
    "Filters": [
        {
            "Field": "Name",
            "Operator": "Contains",
            "Value": "Admin"
        }
    ]
}
```

And below is the response in JSON
``` JSON
{
    "DataPageIndex": 0,
    "DataPageSize": 5,
    "DataCount": 8,
    "Data": [
        {
            "ID": 1,
            "Name": "System Admin",
            "Email": "info@shift.software"
        },
        {
            "ID": 60054,
            "Name": "Admin 5",
            "Email": null
        },
        {
            "ID": 60057,
            "Name": "Admin 6",
            "Email": null
        },
        {
            "ID": 60059,
            "Name": "Admin",
            "Email": null
        },
        {
            "ID": 60068,
            "Name": "Admin 7",
            "Email": null
        }
    ],
    "Summary": {
        "Count": 8
    },
    "Sort": [
        {
            "Field": "ID",
            "SortDirection": 0
        }
    ],
    "StableSort": {
        "Field": "ID",
        "SortDirection": 0
    },
    "Filters": [
        {
            "Field": "Name",
            "Value": "Admin",
            "Operator": "Contains",
            "OR": null
        }
    ],
    "Columns": [
        {
            "HeaderText": "ID",
            "Field": "ID",
            "Visible": true,
            "Order": 0
        },
        {
            "HeaderText": "Name",
            "Field": "Name",
            "Visible": true,
            "Order": 1
        },
        {
            "HeaderText": "Email",
            "Field": "Email",
            "Visible": true,
            "Order": 2
        }
    ],
    "Pagination": {
        "Count": 2,
        "PageSize": 10,
        "PageStart": 0,
        "PageEnd": 1,
        "PageIndex": 0,
        "HasPreviousPage": false,
        "HasNextPage": false,
        "LastPageIndex": 1,
        "DataStart": 1,
        "DataEnd": 5
    },
    "ExportConfig": null,
    "BeforeDataLoading": "2022-05-22T08:59:34.4203655Z",
    "AfterDataLoading": "2022-05-22T08:59:34.4253183Z",
    "ExportMode": false
}
```