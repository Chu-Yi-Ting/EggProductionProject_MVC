namespace EggProductionProject_MVC.Areas.Backstage.ViewModels
{
    public class TrackViewModel
    {
        public int TrackSid { get; set; }

        public int? OrderSid { get; set; }
        public string? TrackingNum { get; set; }

        public string? OrderStatus { get; set; }

        public DateTime? CreatedTime { get; set; }

        public string? CarrierWay { get; set; }

        public string? CarrierName { get; set; }
    }
}
