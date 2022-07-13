### ToShiftGridAsync | ToShiftGrid
The ``Grid`` can be initialized by calling the ``ToShiftGridAsync`` or ``ToShiftGrid`` extension methods on an ``IQueryable``

``` C#
var shiftGridAsync = await db.Employees.ToShiftGridAsync("ID");
```
or
``` C#
var shiftGrid = db.Employees.ToShiftGrid("ID");
```

Parameters:

| Parameter                  | Description                                                                                          |
| ----------------------     | ---------------------------------------------------------------------------------------------------- |
| `stableSortField`          | `String` <br/> The unique field in the dataset for sorting the result.<br/> [More about Stable Sorting](/philosophy/#stable-sort)   |
| `stableSortDirection`      | [`SortDirection`](/reference/#gridsort) <br/> The sort direction for the Stable Sorting. <br/> Defaults to `Ascending` |
| `gridConfig`               | [`GridConfig`](/reference/#gridconfig) <br/> This is how the grid is controlled. Page Size, Page Index, Filters, Sorting ...etc |


### SelectAggregate
Used for Aggregating data.  
The Aggregation works on the entire data set (Not the paginated Data.). And all the aggregation is performed from the Database side.

``` C#
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
```

!!! tip
    
    When aggregating. It's very important to Include the total count of the data. Something like ``Count = x.Count()``.   
    If you do this. We'll use your ``Count`` as the ``DataCount`` for the [`Grid`](/reference/#grid).
       
    This means there'll be 2 Database calls. One for getting the paginated data. And one for getting the aggrecated data.   
       
    If you don't Include the ``Count``. We'll add another database call for getting the ``Count``. And you'll have 3 Database calls instead of 2.


!!! danger
    
    If you do include the ``Count``. Make sure you do a full count and not a conditional count. If you something like below for example, the ``Grid`` will use your count as the ``DataCount`` leaving you with unexpected behaviour. 
    ``` C#
    .SelectAggregate(x => new
        {
            //This is very dangerous
            Count = x.Count(y=> y.ID > 10),
        }
    ```

    Do below instead
    ``` C#
    .SelectAggregate(x => new
        {
            //This is safe and recommended
            Count = x.Count(),
        }
    ```
    