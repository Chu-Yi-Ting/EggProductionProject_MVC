namespace EggProductionProject_MVC.Areas.Backstage.ViewModels
{
    public class OrderViewModel
    {
        public int OrderSid { get; set; }

        public string? OrderNo { get; set; }

        public DateTime? OrderCreatedTime { get; set; }

        public string? MemberName { get; set; }

        public int? PaymentNo { get; set; }

        public string? Payment { get; set; }

        public int? AlreadyPaid { get; set; }

        public int? OrderStatusNo { get; set; }

        public string? OrderStatus { get; set; }

        public string? TrackingNum { get; set; }

        public decimal? Price { get; set; }

        public decimal? TotalPrice { get; set; }
    }
}
