namespace ShiftSoftware.ShiftGrid.Core
{
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptIn)]
    public  class GridColumn : System.Attribute
    {
        [Newtonsoft.Json.JsonProperty]
        public string HeaderText { get; set; }

        [Newtonsoft.Json.JsonProperty]
        public string Field { get; set; }

        [Newtonsoft.Json.JsonProperty]
        public int Order { get; set; }
    }
}
