namespace ECommereceApi.DTOs
{
    public class MultipleProductsCardDTO
    {
        public int UserId { get; set; }
        public HashSet<int> ProductsIds { get; set; } = new HashSet<int>();
        public List<int> Amounts { get; set; } = new List<int>();
    }
}
