using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EggProductionProject_MVC.Models.MemberVM
{
    public class MemberVM
    {
        [Key]
     public int MemberSid { get; set; }

        [Display(Name ="會員名稱")]
            public string Name { get; set; }
        [Display(Name = "會員信箱")]

        public string Email { get; set; }
        [Display(Name = "聯絡電話")]

        public string Phone { get; set; }
        [Display(Name = "會員生日")]

        public DateOnly? BirthDate { get; set; }
        [Display(Name = "會員身分")]

        public int? IsChickFarm { get; set; }
        [Display(Name = "會員購物等級")]

        public int? ShoppingRankNo { get; set; }

        [Display(Name = "會員頭貼")]

        public byte[]? ProfilePic { get; set; }
        [Display(Name = "是否被禁用")]

        public byte? IsBlocked { get; set; }
        [NotMapped]
        public IFormFile? ProfilePicFile { get; set; }
        //public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

        //public virtual ICollection<Calendar> Calendars { get; set; } = new List<Calendar>();

        //public virtual ICollection<CarrierAddress> CarrierAddresses { get; set; } = new List<CarrierAddress>();

        //public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

        //public virtual ICollection<Certification> Certifications { get; set; } = new List<Certification>();

        //public virtual ICollection<Creator> Creators { get; set; } = new List<Creator>();

        //public virtual ICollection<Friend> Friends { get; set; } = new List<Friend>();

        //public virtual ICollection<GoodorBad> GoodorBads { get; set; } = new List<GoodorBad>();

        //public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

        //public virtual ICollection<MemberArea> MemberAreas { get; set; } = new List<MemberArea>();

        //public virtual ICollection<Notify> Notifies { get; set; } = new List<Notify>();

        //public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        //public virtual ICollection<Reply> Replies { get; set; } = new List<Reply>();

        //public virtual ShoppingRank ShoppingRankNoNavigation { get; set; }

        //public virtual ICollection<StoreCoin> StoreCoins { get; set; } = new List<StoreCoin>();

        //public virtual ICollection<Store> Stores { get; set; } = new List<Store>();

    }
}
