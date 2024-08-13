namespace EggProductionProject_MVC.Models.MemberVM
{
    public class FriendVM
    {
        public int FriendSid { get; set; }

        public string MemberName1 { get; set; }

        public string MemberName2 { get; set; }

        public DateOnly? DateAdded { get; set; }

        //public virtual ICollection<FriendAction> FriendActions { get; set; } = new List<FriendAction>();

        //public virtual Member MemberS { get; set; }

        //public virtual ICollection<Talk> Talks { get; set; } = new List<Talk>();
    }
}
