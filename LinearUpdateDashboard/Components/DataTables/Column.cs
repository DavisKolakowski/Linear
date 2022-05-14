namespace LinearUpdateDashboard.Components.DataTables
{
    public class Column
    {
        public string Data { get; set; } = string.Empty;
        public string Name { get; set; } = "Name";
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public Search Search { get; set; } = new Search();
    }
}
