using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShiftGrid.Test.Shared.Insert
{
    public class Tools
    {
        public List<Models.Type> GenerateTypes()
        {
            var types = new List<Models.Type>();

            for (int i = 0; i < 2; i++)
            {
                var type = new Shared.Models.Type
                {
                    Name = $"Type - {(i + 1)}"
                };

                types.Add(type);
            }

            return types;
        }

        public List<Models.TestItem> GenerateTestItems(InsertPayload payload)
        {
            var price = payload.DataTemplate.Price;
            var step = 0;

            var items = new List<Models.TestItem>();

            for (int i = 0; i < payload.DataCount; i++)
            {
                step++;

                var number = i + 1;

                if (payload.Increments.Step > 1)
                {
                    if (step == payload.Increments.Step)
                    {
                        step = 0;
                        price = price + payload.Increments.Step;
                    }
                }
                else
                {
                    price = payload.DataTemplate.Price + (i * payload.Increments.Price);
                }

                var testItem = new Models.TestItem
                {
                    Code = $"{payload.DataTemplate.Code} - {number}",
                    Title = $"{payload.DataTemplate.Title} - {number}",
                    Date = payload.DataTemplate.Date.AddDays(i * payload.Increments.Day),
                    Price = price,
                    TypeId = i % 2 == 0 ? 1 : 2,
                    ParentTestItemId = payload.ParentTestItemId
                };


                items.Add(testItem);
            }

            return items;
        }
    }
}
