namespace LinearUpdateDashboard.Components.DataTables
{
    public class TableBase
    {
        public int Draw { get; set; }
        public IEnumerable<Column> Columns { get; set; } = Enumerable.Empty<Column>();
        public IEnumerable<Order> Order { get; set; } = Enumerable.Empty<Order>();
        public int Start { get; set; }
        public int Length { get; set; }
        public Search Search { get; set; } = new Search();
    }
}
