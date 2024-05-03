namespace ECommereceApi.DTOs
{
    public class PagedResult<T> where T : class
    {
        public List<T> Items { get; set; } = [];
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious  { get; set; }
        public bool HasNext  { get; set; }
    }
}
