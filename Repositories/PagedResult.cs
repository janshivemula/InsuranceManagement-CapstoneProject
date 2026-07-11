namespace InsuranceManagementSystem.Repositories
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();

        public int TotalRecords { get; set; }
    }
}
