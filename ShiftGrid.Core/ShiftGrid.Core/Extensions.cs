using System;
using System.Linq;
using System.Reflection;

namespace ShiftGrid.Core
{
    public static class Extensions
    {
        public static Grid<T> ToShiftGrid<T>(this IQueryable<T> select, Grid<T> grid = null)
        {
            //If no grid is passed. Initialize a new grid with default PageSize = 20. And sort by the first colum ascending
            if (grid == null)
            {
                grid = new Grid<T>();
            }

            if(grid.Sort.Count == 0)
            {
                grid.Sort = new System.Collections.ObjectModel.ObservableCollection<GridSort> {
                    new GridSort {
                        Field = typeof(T).GetProperties().Where(x=> x.MemberType == MemberTypes.Property).First().Name,
                        SortDirection = SortDirection.Ascending
                    }
                };
            }

            if (grid.DataPageSize == 0)
                grid.DataPageSize = 20;

            grid.ShiftQL = select;
            grid.ShiftQLInitialized = true;

            grid.Init();

            return grid;
        }

        public static T ToType<T>(this object obj)
        {

            //create instance of T type object:
            var tmp = Activator.CreateInstance(typeof(T));

            //loop through the properties of the object you want to covert:          
            foreach (PropertyInfo pi in obj.GetType().GetProperties())
            {
                try
                {

                    //get the value of property and try 
                    //to assign it to the property of T type object:
                    tmp.GetType().GetProperty(pi.Name).SetValue(tmp,
                                              pi.GetValue(obj, null), null);
                }
                catch { }
            }

            //return the T type object:         
            return (T)tmp;

        }
    }
}
