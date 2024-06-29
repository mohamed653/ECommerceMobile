namespace ECommereceApi.DTOs.Order
{
    public class OrderStatsDTO
    {
        public int TotalPendingOrders { get; set; }
        public int TotalAcceptedOrders { get; set; }
        public int TotalShippedOrders { get; set; }
        public int TotalDeliveredOrders { get; set; }
        public int TotalCancelledOrders { get; set; }
        public int TotalOrders { get; set; } = 0;
    }
}
