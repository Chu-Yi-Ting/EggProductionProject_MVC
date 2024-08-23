namespace EggProductionProject_MVC.Areas.Backstage.ViewModels
{
    public class CouponViewModel
    {
        public int CouponSid { get; set; }

        public int? CouponTypeNo { get; set; }

        public string? Name { get; set; }

        public int? CouponStatusNo { get; set; }

        public string? Status { get; set; }

        public DateTime? CollectionTime { get; set; }

        public string? MemberName { get; set; }
    }
}
