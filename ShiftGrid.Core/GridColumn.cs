namespace ShiftSoftware.ShiftGrid.Core
{
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptIn)]
    public  class GridColumn
    {
        [Newtonsoft.Json.JsonProperty]
        public string HeaderText { get; set; }

        [Newtonsoft.Json.JsonProperty]
        public string Field { get; set; }

        [Newtonsoft.Json.JsonProperty]
        public bool Visible { get; set; } = true;

        [Newtonsoft.Json.JsonProperty]
        public int? Order { get; set; } = null;
    }

    public class GridColumnAttribute : System.Attribute
    {
        public string HeaderText { get; set; }
        public bool Visible { get; set; }
        public int Order { get; set; } = int.MinValue;
    };
}
