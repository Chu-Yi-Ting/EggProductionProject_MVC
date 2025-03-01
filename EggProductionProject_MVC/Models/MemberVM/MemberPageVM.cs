﻿using System.ComponentModel.DataAnnotations;

namespace EggProductionProject_MVC.Models.MemberVM
{
    public partial class MemberPageVM
    {
        public int MemberSid { get; set; }
        [Required(ErrorMessage ="姓名為必填欄位")]
        public string Name { get; set; }

        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "請輸入有效的電子郵件地址")]
        [Required(ErrorMessage = "信箱為必填欄位")]

        public string Email { get; set; }
        [Required(ErrorMessage = "電話為必填欄位")]

        [RegularExpression(@"^09\d{8}$", ErrorMessage = "電話號碼應為09開頭的10碼數字")]
		[StringLength(10, ErrorMessage = "電話號碼應為09開頭的10碼數字", MinimumLength = 10)]
		public string Phone { get; set; }

        public DateOnly? BirthDate { get; set; }

        public int? IsChickFarm { get; set; }

        public int? ShoppingRankNo { get; set; }

        public string PassWord { get; set; }

        public string UserName { get; set; }

        public byte? IsBlocked { get; set; }
        [StringLength(5, ErrorMessage = "雞農驗證碼長度為5個字", MinimumLength = 5)]
        public string Chickcode { get; set; }

        public string AspUserId { get; set; }

        public string ProfilePic { get; set; }

        public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

        public virtual AspNetUser AspUser { get; set; }

        public virtual ICollection<Calendar> Calendars { get; set; } = new List<Calendar>();

        public virtual ICollection<CarrierAddress> CarrierAddresses { get; set; } = new List<CarrierAddress>();

        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

        public virtual ICollection<Certification> Certifications { get; set; } = new List<Certification>();

        public virtual ICollection<ChatRoom> ChatRooms { get; set; } = new List<ChatRoom>();

        public virtual ICollection<Creator> Creators { get; set; } = new List<Creator>();

        public virtual ICollection<Friend> Friends { get; set; } = new List<Friend>();

        public virtual ICollection<GoodorBad> GoodorBads { get; set; } = new List<GoodorBad>();

        public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

        public virtual ICollection<MemberArea> MemberAreas { get; set; } = new List<MemberArea>();

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

        public virtual ICollection<Notify> Notifies { get; set; } = new List<Notify>();

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public virtual ICollection<Reply> Replies { get; set; } = new List<Reply>();

        public virtual ShoppingRank ShoppingRankNoNavigation { get; set; }

        public virtual ICollection<StoreCoin> StoreCoins { get; set; } = new List<StoreCoin>();

        public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
    }
}
