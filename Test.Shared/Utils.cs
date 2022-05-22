using ShiftGrid.Test.Shared.Insert;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShiftGrid.Test.Shared
{
    public interface Utils
    {
        System.Type DBType { get; set; }
        List<string> Logs { get; set; }

        IQueryable<Models.TestItem> GetTestItems();

        Task DeleteAll();
        Task InsertTypes();
        Task InsertTestItems(InsertPayload payload);

        Task PopulateTestData(int count, int? subCount = null, int step = 1);
    }
}
