using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Member
{
    public int MemberSid { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public DateOnly? BirthDate { get; set; }

    public int? IsChickFarm { get; set; }

    public int? ShoppingRankNo { get; set; }

    public string? PassWord { get; set; }

    public string? UserName { get; set; }

    public byte[]? ProfilePic { get; set; }

    public byte? IsBlocked { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual ICollection<Calendar> Calendars { get; set; } = new List<Calendar>();

    public virtual ICollection<CarrierAddress> CarrierAddresses { get; set; } = new List<CarrierAddress>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Certification> Certifications { get; set; } = new List<Certification>();

    public virtual ICollection<Creator> Creators { get; set; } = new List<Creator>();

    public virtual ICollection<Friend> Friends { get; set; } = new List<Friend>();

    public virtual ICollection<GoodorBad> GoodorBads { get; set; } = new List<GoodorBad>();

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual ICollection<MemberArea> MemberAreas { get; set; } = new List<MemberArea>();

    public virtual ICollection<Notify> Notifies { get; set; } = new List<Notify>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Reply> Replies { get; set; } = new List<Reply>();

    public virtual ShoppingRank? ShoppingRankNoNavigation { get; set; }

    public virtual ICollection<StoreCoin> StoreCoins { get; set; } = new List<StoreCoin>();

    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
}
