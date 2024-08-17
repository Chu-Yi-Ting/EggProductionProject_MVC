namespace EggProductionProject_MVC.Areas.Backstage.ViewModels
{
    public class CoinViewModel
    {
        public int StoreCoinSid { get; set; }


        public string? MemberName { get; set; }

        public int? CoinUseAreaNo { get; set; }

        public string? CoinUseArea { get; set; }

        public int? IsPositive { get; set; }

        public decimal? Money { get; set; }

        public DateTime? ChangeTime { get; set; }
    }
}
