namespace EggProductionProject_MVC.Areas.Backstage.ViewModels
{
    internal class SaleViewModel
    {
        public int? PeriodSid { get; set; }
        public DateOnly? PeriodDate { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public int? RunningStatusNo { get; set; }
        public string? RunningStatusDescription { get; set; }
    }
}