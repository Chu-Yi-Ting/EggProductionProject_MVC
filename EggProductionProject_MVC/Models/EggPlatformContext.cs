﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EggProductionProject_MVC.Models;

public partial class EggPlatformContext : DbContext
{
    public EggPlatformContext(DbContextOptions<EggPlatformContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Advertisment> Advertisments { get; set; }

    public virtual DbSet<AreaFeed> AreaFeeds { get; set; }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<ArticleCategory> ArticleCategories { get; set; }

    public virtual DbSet<ArticleCreater> ArticleCreaters { get; set; }

    public virtual DbSet<Calendar> Calendars { get; set; }

    public virtual DbSet<Carrier> Carriers { get; set; }

    public virtual DbSet<CarrierAddress> CarrierAddresses { get; set; }

    public virtual DbSet<CarrierOpen> CarrierOpens { get; set; }

    public virtual DbSet<CarrierWay> CarrierWays { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Certification> Certifications { get; set; }

    public virtual DbSet<ChickDeath> ChickDeaths { get; set; }

    public virtual DbSet<ChickHouse> ChickHouses { get; set; }

    public virtual DbSet<ChickLotNo> ChickLotNos { get; set; }

    public virtual DbSet<CoinUseArea> CoinUseAreas { get; set; }

    public virtual DbSet<Collect> Collects { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<CouponStatus> CouponStatuses { get; set; }

    public virtual DbSet<CouponType> CouponTypes { get; set; }

    public virtual DbSet<Creator> Creators { get; set; }

    public virtual DbSet<DailyEggRe> DailyEggRes { get; set; }

    public virtual DbSet<EcImage> EcImages { get; set; }

    public virtual DbSet<Edit> Edits { get; set; }

    public virtual DbSet<EmpDepartment> EmpDepartments { get; set; }

    public virtual DbSet<EmpRank> EmpRanks { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<FlashSale> FlashSales { get; set; }

    public virtual DbSet<Friend> Friends { get; set; }

    public virtual DbSet<FriendAction> FriendActions { get; set; }

    public virtual DbSet<FriendActionType> FriendActionTypes { get; set; }

    public virtual DbSet<GoodorBad> GoodorBads { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<LogicalDeletion> LogicalDeletions { get; set; }

    public virtual DbSet<Manure> Manures { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MemberArea> MemberAreas { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Nature> Natures { get; set; }

    public virtual DbSet<Notify> Notifies { get; set; }

    public virtual DbSet<NotifyType> NotifyTypes { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductItem> ProductItems { get; set; }

    public virtual DbSet<ProductReview> ProductReviews { get; set; }

    public virtual DbSet<ProductSubcategory> ProductSubcategories { get; set; }

    public virtual DbSet<PublicStatus> PublicStatuses { get; set; }

    public virtual DbSet<Reply> Replies { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<ReportReason> ReportReasons { get; set; }

    public virtual DbSet<SalesBatch> SalesBatches { get; set; }

    public virtual DbSet<ScreenSummary> ScreenSummaries { get; set; }

    public virtual DbSet<ShoppingRank> ShoppingRanks { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<StoreCoin> StoreCoins { get; set; }

    public virtual DbSet<SubscriptionMasterList> SubscriptionMasterLists { get; set; }

    public virtual DbSet<Talk> Talks { get; set; }

    public virtual DbSet<Track> Tracks { get; set; }

    public virtual DbSet<TrackStatus> TrackStatuses { get; set; }

    public virtual DbSet<TrackTime> TrackTimes { get; set; }

    public virtual DbSet<Vaccinate> Vaccinates { get; set; }

    public virtual DbSet<VaccineTable> VaccineTables { get; set; }

    public virtual DbSet<VideoSummary> VideoSummaries { get; set; }

    public virtual DbSet<WebSiteType> WebSiteTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Advertisment>(entity =>
        {
            entity.HasKey(e => e.AdvertismentSid);

            entity.ToTable(tb => tb.HasTrigger("trg_UpdateAdBehavior"));

            entity.Property(e => e.AdContent).HasMaxLength(50);
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.UploadTime).HasColumnType("datetime");

            entity.HasOne(d => d.PublicStatusNoNavigation).WithMany(p => p.Advertisments)
                .HasForeignKey(d => d.PublicStatusNo)
                .HasConstraintName("FK_Advertisments_PublicStatus");

            entity.HasOne(d => d.StoreS).WithMany(p => p.Advertisments)
                .HasForeignKey(d => d.StoreSid)
                .HasConstraintName("FK_Advertisments_Stores");

            entity.HasOne(d => d.VideoS).WithMany(p => p.Advertisments)
                .HasForeignKey(d => d.VideoSid)
                .HasConstraintName("FK_Advertisments_VideoSummary");
        });

        modelBuilder.Entity<AreaFeed>(entity =>
        {
            entity.HasKey(e => e.FeedSid);

            entity.ToTable("AreaFeed");

            entity.Property(e => e.Cost).HasColumnType("money");
            entity.Property(e => e.LotNo).HasMaxLength(50);
            entity.Property(e => e.Weight).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.AreaS).WithMany(p => p.AreaFeeds)
                .HasForeignKey(d => d.AreaSid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AreaFeed_MemberArea");
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleSid);

            entity.ToTable("Article");

            entity.Property(e => e.ArticleDate).HasColumnType("datetime");
            entity.Property(e => e.ArticleTitle).HasMaxLength(50);
            entity.Property(e => e.ArticleUpdate).HasColumnType("datetime");

            entity.HasOne(d => d.ArticleCategoriesS).WithMany(p => p.Articles)
                .HasForeignKey(d => d.ArticleCategoriesSid)
                .HasConstraintName("FK_Article_ArticleCategories");

            entity.HasOne(d => d.ArticleCreaterS).WithMany(p => p.Articles)
                .HasForeignKey(d => d.ArticleCreaterSid)
                .HasConstraintName("FK_Article_Member");

            entity.HasOne(d => d.PublicStatusNoNavigation).WithMany(p => p.Articles)
                .HasForeignKey(d => d.PublicStatusNo)
                .HasConstraintName("FK_Article_PublicStatus");
        });

        modelBuilder.Entity<ArticleCategory>(entity =>
        {
            entity.HasKey(e => e.ArticleCategoriesSid);

            entity.Property(e => e.ArticleCategories).HasMaxLength(50);
            entity.Property(e => e.ArticleCategoriesImg).HasColumnType("image");
        });

        modelBuilder.Entity<ArticleCreater>(entity =>
        {
            entity.HasKey(e => e.ArticleCreaterSid);

            entity.ToTable("ArticleCreater");

            entity.Property(e => e.PersonalInfo).HasColumnName("Personal Info");
        });

        modelBuilder.Entity<Calendar>(entity =>
        {
            entity.HasKey(e => e.CalendarSid);

            entity.ToTable("Calendar");

            entity.HasOne(d => d.MemberS).WithMany(p => p.Calendars)
                .HasForeignKey(d => d.MemberSid)
                .HasConstraintName("FK_Calendar_Member");
        });

        modelBuilder.Entity<Carrier>(entity =>
        {
            entity.HasKey(e => e.CarrierNo);

            entity.Property(e => e.CarrierCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CarrierName).HasMaxLength(10);
        });

        modelBuilder.Entity<CarrierAddress>(entity =>
        {
            entity.HasKey(e => e.CarrierAddressSid).HasName("PK_CarrierStores");

            entity.ToTable("CarrierAddress");

            entity.Property(e => e.Adress).HasMaxLength(50);
            entity.Property(e => e.RecordName).HasMaxLength(50);
            entity.Property(e => e.RecordPhone)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StoreId)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.StoreName).HasMaxLength(20);

            entity.HasOne(d => d.CarrierWayNoNavigation).WithMany(p => p.CarrierAddresses)
                .HasForeignKey(d => d.CarrierWayNo)
                .HasConstraintName("FK_CarrierStores_CarrierWays");

            entity.HasOne(d => d.MemberS).WithMany(p => p.CarrierAddresses)
                .HasForeignKey(d => d.MemberSid)
                .HasConstraintName("FK_CarrierStores_Member");
        });

        modelBuilder.Entity<CarrierOpen>(entity =>
        {
            entity.HasKey(e => e.CarrierOpenNo);

            entity.HasOne(d => d.StoreS).WithMany(p => p.CarrierOpens)
                .HasForeignKey(d => d.StoreSid)
                .HasConstraintName("FK_CarrierOpens_Stores");
        });

        modelBuilder.Entity<CarrierWay>(entity =>
        {
            entity.HasKey(e => e.CarrierWayNo);

            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.Way).HasMaxLength(20);

            entity.HasOne(d => d.CarrierNoNavigation).WithMany(p => p.CarrierWays)
                .HasForeignKey(d => d.CarrierNo)
                .HasConstraintName("FK_CarrierWays_Carriers");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartSid).HasName("PK_Cart");

            entity.HasOne(d => d.MemberS).WithMany(p => p.Carts)
                .HasForeignKey(d => d.MemberSid)
                .HasConstraintName("FK_Carts_Member");

            entity.HasOne(d => d.ProductS).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductSid)
                .HasConstraintName("FK_Carts_Products");
        });

        modelBuilder.Entity<Certification>(entity =>
        {
            entity.HasKey(e => e.CertificationSid);

            entity.ToTable("Certification");

            entity.Property(e => e.CertificationNo)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.MemberS).WithMany(p => p.Certifications)
                .HasForeignKey(d => d.MemberSid)
                .HasConstraintName("FK_Certification_Member");
        });

        modelBuilder.Entity<ChickDeath>(entity =>
        {
            entity.HasKey(e => e.ChickDeathSid);

            entity.ToTable("ChickDeath");

            entity.HasOne(d => d.HouseS).WithMany(p => p.ChickDeaths)
                .HasForeignKey(d => d.HouseSid)
                .HasConstraintName("FK_ChickDeath_ChickHouse");
        });

        modelBuilder.Entity<ChickHouse>(entity =>
        {
            entity.HasKey(e => e.HouseSid);

            entity.ToTable("ChickHouse");

            entity.Property(e => e.HouseName).HasMaxLength(50);

            entity.HasOne(d => d.AreaS).WithMany(p => p.ChickHouses)
                .HasForeignKey(d => d.AreaSid)
                .HasConstraintName("FK_ChickHouse_MemberArea");

            entity.HasOne(d => d.SubcategoryNoNavigation).WithMany(p => p.ChickHouses)
                .HasForeignKey(d => d.SubcategoryNo)
                .HasConstraintName("FK_ChickHouse_ProductSubcategory");
        });

        modelBuilder.Entity<ChickLotNo>(entity =>
        {
            entity.HasKey(e => e.ChickSid);

            entity.ToTable("ChickLotNo");

            entity.Property(e => e.ChickLotNo1)
                .HasMaxLength(20)
                .HasColumnName("ChickLotNo");
            entity.Property(e => e.Cost).HasColumnType("money");

            entity.HasOne(d => d.HouseS).WithMany(p => p.ChickLotNos)
                .HasForeignKey(d => d.HouseSid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChickLotNo_ChickHouse");
        });

        modelBuilder.Entity<CoinUseArea>(entity =>
        {
            entity.HasKey(e => e.CoinUseAreaNo).HasName("PK_CoinChanges");

            entity.Property(e => e.UseArea).HasMaxLength(50);
        });

        modelBuilder.Entity<Collect>(entity =>
        {
            entity.HasKey(e => e.CollectSid);

            entity.ToTable("Collect");

            entity.HasOne(d => d.WebSiteTypeS).WithMany(p => p.Collects)
                .HasForeignKey(d => d.WebSiteTypeSid)
                .HasConstraintName("FK_Collect_WebSiteType");
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.HasKey(e => e.CouponSid);

            entity.Property(e => e.CollectionTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.CouponStatusNoNavigation).WithMany(p => p.Coupons)
                .HasForeignKey(d => d.CouponStatusNo)
                .HasConstraintName("FK_Coupons_CouponStatuses");

            entity.HasOne(d => d.CouponTypeNoNavigation).WithMany(p => p.Coupons)
                .HasForeignKey(d => d.CouponTypeNo)
                .HasConstraintName("FK_Coupons_CouponTypes");
        });

        modelBuilder.Entity<CouponStatus>(entity =>
        {
            entity.HasKey(e => e.CouponStatusNo);

            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<CouponType>(entity =>
        {
            entity.HasKey(e => e.CouponTypeNo);

            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.Minimum).HasColumnType("money");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.PublicStatusNoNavigation).WithMany(p => p.CouponTypes)
                .HasForeignKey(d => d.PublicStatusNo)
                .HasConstraintName("FK_CouponTypes_PublicStatus");
        });

        modelBuilder.Entity<Creator>(entity =>
        {
            entity.HasKey(e => e.CreatorSid);

            entity.ToTable("Creator");

            entity.Property(e => e.Image).HasColumnType("image");
            entity.Property(e => e.MemberName).HasMaxLength(50);
            entity.Property(e => e.PersonalProfile).HasMaxLength(50);

            entity.HasOne(d => d.MemberS).WithMany(p => p.Creators)
                .HasForeignKey(d => d.MemberSid)
                .HasConstraintName("FK_Creator_Member");

            entity.HasOne(d => d.SubscriptionS).WithMany(p => p.Creators)
                .HasForeignKey(d => d.SubscriptionSid)
                .HasConstraintName("FK_Creator_SubscriptionMasterList");
        });

        modelBuilder.Entity<DailyEggRe>(entity =>
        {
            entity.HasKey(e => e.EggSid);

            entity.ToTable("DailyEggRe");

            entity.Property(e => e.UnQamount).HasColumnName("UnQAmount");

            entity.HasOne(d => d.HouseS).WithMany(p => p.DailyEggRes)
                .HasForeignKey(d => d.HouseSid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DailyEggRe_ChickHouse");
        });

        modelBuilder.Entity<EcImage>(entity =>
        {
            entity.HasKey(e => e.EcImgSid);

            entity.ToTable("EcImage");

            entity.Property(e => e.EcImg).HasColumnType("image");

            entity.HasOne(d => d.ProductS).WithMany(p => p.EcImages)
                .HasForeignKey(d => d.ProductSid)
                .HasConstraintName("FK_EcImage_Products");

            entity.HasOne(d => d.StoreS).WithMany(p => p.EcImages)
                .HasForeignKey(d => d.StoreSid)
                .HasConstraintName("FK_EcImage_Stores");
        });

        modelBuilder.Entity<Edit>(entity =>
        {
            entity.HasKey(e => e.EditSid);

            entity.ToTable("Edit");

            entity.Property(e => e.EditTime).HasColumnType("datetime");

            entity.HasOne(d => d.ArticleS).WithMany(p => p.Edits)
                .HasForeignKey(d => d.ArticleSid)
                .HasConstraintName("FK_Edit_Article");

            entity.HasOne(d => d.ReplyS).WithMany(p => p.Edits)
                .HasForeignKey(d => d.ReplySid)
                .HasConstraintName("FK_Edit_Reply");
        });

        modelBuilder.Entity<EmpDepartment>(entity =>
        {
            entity.HasKey(e => e.EmpDepartmentSid);

            entity.ToTable("EmpDepartment");

            entity.Property(e => e.EmpDepartment1)
                .HasMaxLength(50)
                .HasColumnName("EmpDepartment");
            entity.Property(e => e.EmpDepartmentLocation).HasMaxLength(50);
            entity.Property(e => e.EmpDepartmentPhone)
                .HasMaxLength(10)
                .IsFixedLength();
        });

        modelBuilder.Entity<EmpRank>(entity =>
        {
            entity.HasKey(e => e.EmpRankSid);

            entity.ToTable("EmpRank");

            entity.Property(e => e.EmpRank1)
                .HasMaxLength(50)
                .HasColumnName("EmpRank");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeSid);

            entity.ToTable("Employee");

            entity.Property(e => e.Account)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmpName).HasMaxLength(50);
            entity.Property(e => e.EmpPhone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.EmpDepartmentS).WithMany(p => p.Employees)
                .HasForeignKey(d => d.EmpDepartmentSid)
                .HasConstraintName("FK_Employee_EmpDepartment");
        });

        modelBuilder.Entity<FlashSale>(entity =>
        {
            entity.HasKey(e => e.DiscountSid);

            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(3, 2)");

            entity.HasOne(d => d.PeriodS).WithMany(p => p.FlashSales)
                .HasForeignKey(d => d.PeriodSid)
                .HasConstraintName("FK_FlashSales_SalesBatches");

            entity.HasOne(d => d.ProductS).WithMany(p => p.FlashSales)
                .HasForeignKey(d => d.ProductSid)
                .HasConstraintName("FK_FlashSales_Products");
        });

        modelBuilder.Entity<Friend>(entity =>
        {
            entity.HasKey(e => e.FriendSid);

            entity.ToTable("Friend");

            entity.HasOne(d => d.MemberS).WithMany(p => p.Friends)
                .HasForeignKey(d => d.MemberSid)
                .HasConstraintName("FK_Friend_Member");
        });

        modelBuilder.Entity<FriendAction>(entity =>
        {
            entity.HasKey(e => e.FriendActionSid);

            entity.ToTable("FriendAction");

            entity.HasOne(d => d.ActionTypeS).WithMany(p => p.FriendActions)
                .HasForeignKey(d => d.ActionTypeSid)
                .HasConstraintName("FK_FriendAction_FriendActionType");

            entity.HasOne(d => d.FriendS).WithMany(p => p.FriendActions)
                .HasForeignKey(d => d.FriendSid)
                .HasConstraintName("FK_FriendAction_Friend");
        });

        modelBuilder.Entity<FriendActionType>(entity =>
        {
            entity.HasKey(e => e.ActionTypeSid);

            entity.ToTable("FriendActionType");

            entity.Property(e => e.ActionType).HasMaxLength(50);
        });

        modelBuilder.Entity<GoodorBad>(entity =>
        {
            entity.HasKey(e => e.GorBsid);

            entity.ToTable("GoodorBad");

            entity.Property(e => e.GorBsid).HasColumnName("GorBSid");
            entity.Property(e => e.GorBdate).HasColumnName("GorBDate");
            entity.Property(e => e.GorBtype).HasColumnName("GorBType");

            entity.HasOne(d => d.ArticleS).WithMany(p => p.GoodorBads)
                .HasForeignKey(d => d.ArticleSid)
                .HasConstraintName("FK_GoodorBad_Article");

            entity.HasOne(d => d.MemberNoNavigation).WithMany(p => p.GoodorBads)
                .HasForeignKey(d => d.MemberNo)
                .HasConstraintName("FK_GoodorBad_Member");

            entity.HasOne(d => d.ReplyS).WithMany(p => p.GoodorBads)
                .HasForeignKey(d => d.ReplySid)
                .HasConstraintName("FK_GoodorBad_Reply");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.LogSid);

            entity.ToTable("Log");

            entity.Property(e => e.LogTime).HasColumnType("datetime");

            entity.HasOne(d => d.MemberS).WithMany(p => p.Logs)
                .HasForeignKey(d => d.MemberSid)
                .HasConstraintName("FK_Log_Member");
        });

        modelBuilder.Entity<LogicalDeletion>(entity =>
        {
            entity.HasKey(e => e.LogicalDeletionNo);

            entity.ToTable("LogicalDeletion");

            entity.Property(e => e.LogicalDeletionNo).ValueGeneratedNever();
            entity.Property(e => e.DeletionStatus).HasMaxLength(50);
        });

        modelBuilder.Entity<Manure>(entity =>
        {
            entity.HasKey(e => e.ManureSid);

            entity.ToTable("Manure");

            entity.Property(e => e.Cost).HasColumnType("money");
            entity.Property(e => e.Weight).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.HouseS).WithMany(p => p.Manures)
                .HasForeignKey(d => d.HouseSid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Manure_ChickHouse");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberSid);

            entity.ToTable("Member");

            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PassWord).HasMaxLength(50);
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.ProfilePic).HasColumnType("image");
            entity.Property(e => e.UserName).HasMaxLength(50);

            entity.HasOne(d => d.ShoppingRankNoNavigation).WithMany(p => p.Members)
                .HasForeignKey(d => d.ShoppingRankNo)
                .HasConstraintName("FK_Member_ShoppingRank");
        });

        modelBuilder.Entity<MemberArea>(entity =>
        {
            entity.HasKey(e => e.AreaSid);

            entity.ToTable("MemberArea");

            entity.Property(e => e.MemberAddress).HasMaxLength(50);
            entity.Property(e => e.MemberName).HasMaxLength(50);

            entity.HasOne(d => d.MemberS).WithMany(p => p.MemberAreas)
                .HasForeignKey(d => d.MemberSid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MemberArea_Member");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageSid);

            entity.ToTable("Message", tb => tb.HasTrigger("sp"));

            entity.Property(e => e.MessageContent).HasMaxLength(100);
            entity.Property(e => e.MessageDate).HasColumnType("datetime");
            entity.Property(e => e.MessageDelete).HasDefaultValue(false);

            entity.HasOne(d => d.VideoS).WithMany(p => p.Messages)
                .HasForeignKey(d => d.VideoSid)
                .HasConstraintName("FK_Message_VideoSummary");
        });

        modelBuilder.Entity<Nature>(entity =>
        {
            entity.HasKey(e => e.NatureSid);

            entity.ToTable("Nature");

            entity.Property(e => e.ViedoNature).HasMaxLength(50);
        });

        modelBuilder.Entity<Notify>(entity =>
        {
            entity.HasKey(e => e.NotifySid);

            entity.ToTable("Notify");

            entity.HasOne(d => d.NotifyReciever).WithMany(p => p.Notifies)
                .HasForeignKey(d => d.NotifyRecieverid)
                .HasConstraintName("FK_Notify_Member");

            entity.HasOne(d => d.NotifyTypeS).WithMany(p => p.Notifies)
                .HasForeignKey(d => d.NotifyTypeSid)
                .HasConstraintName("FK_Notify_NotifyType");

            entity.HasOne(d => d.WebSiteTypeS).WithMany(p => p.Notifies)
                .HasForeignKey(d => d.WebSiteTypeSid)
                .HasConstraintName("FK_Notify_WebSiteType");
        });

        modelBuilder.Entity<NotifyType>(entity =>
        {
            entity.HasKey(e => e.NotifyTypeSid);

            entity.ToTable("NotifyType");

            entity.Property(e => e.NotifyType1)
                .HasMaxLength(50)
                .HasColumnName("NotifyType");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderSid).HasName("PK_Order");

            entity.Property(e => e.OrderCreatedTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OrderNo)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TotalPrice).HasColumnType("money");

            entity.HasOne(d => d.CouponS).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CouponSid)
                .HasConstraintName("FK_Orders_Coupons");

            entity.HasOne(d => d.MemberS).WithMany(p => p.Orders)
                .HasForeignKey(d => d.MemberSid)
                .HasConstraintName("FK_Orders_Member");

            entity.HasOne(d => d.OrderStatusNoNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderStatusNo)
                .HasConstraintName("FK_Orders_OrderStatuses");

            entity.HasOne(d => d.PaymentNoNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentNo)
                .HasConstraintName("FK_Orders_Payments");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailSid).HasName("PK_OrderDetail");

            entity.Property(e => e.BuyPrice).HasColumnType("money");

            entity.HasOne(d => d.OrderS).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderSid)
                .HasConstraintName("FK_OrderDetails_Orders");

            entity.HasOne(d => d.ProductS).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductSid)
                .HasConstraintName("FK_OrderDetails_Products");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.OrderStatusNo);

            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentNo);

            entity.Property(e => e.Pay).HasMaxLength(50);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductSid);

            entity.Property(e => e.Component).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(3, 2)");
            entity.Property(e => e.Origin).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.ProductName).HasMaxLength(50);
            entity.Property(e => e.ProductNo).HasMaxLength(50);

            entity.HasOne(d => d.PublicStatusNoNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.PublicStatusNo)
                .HasConstraintName("FK_Products_PublicStatus");

            entity.HasOne(d => d.StoreS).WithMany(p => p.Products)
                .HasForeignKey(d => d.StoreSid)
                .HasConstraintName("FK_Products_Stores");

            entity.HasOne(d => d.SubcategoryNoNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.SubcategoryNo)
                .HasConstraintName("FK_Products_ProductSubcategory");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.ProductCategoryNo);

            entity.ToTable("ProductCategory");

            entity.Property(e => e.ProductCategoryNo).ValueGeneratedNever();
            entity.Property(e => e.ProductCategoryName).HasMaxLength(50);
        });

        modelBuilder.Entity<ProductItem>(entity =>
        {
            entity.HasKey(e => e.ItemNo);

            entity.Property(e => e.ItemNo).ValueGeneratedNever();
            entity.Property(e => e.ItemDescription).HasMaxLength(50);
            entity.Property(e => e.ItemName).HasMaxLength(50);

            entity.HasOne(d => d.SubcategoryNoNavigation).WithMany(p => p.ProductItems)
                .HasForeignKey(d => d.SubcategoryNo)
                .HasConstraintName("FK_ProductItems_ProductSubcategory");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.ReviewSid);

            entity.Property(e => e.ReviewContent).HasMaxLength(50);

            entity.HasOne(d => d.ProductS).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.ProductSid)
                .HasConstraintName("FK_ProductReviews_Products");
        });

        modelBuilder.Entity<ProductSubcategory>(entity =>
        {
            entity.HasKey(e => e.SubcategoryNo);

            entity.ToTable("ProductSubcategory");

            entity.Property(e => e.SubcategoryNo).ValueGeneratedNever();
            entity.Property(e => e.SubcategoryName).HasMaxLength(50);

            entity.HasOne(d => d.ProductCategoryNoNavigation).WithMany(p => p.ProductSubcategories)
                .HasForeignKey(d => d.ProductCategoryNo)
                .HasConstraintName("FK_ProductSubcategory_ProductCategory");
        });

        modelBuilder.Entity<PublicStatus>(entity =>
        {
            entity.HasKey(e => e.PublicStatusNo);

            entity.ToTable("PublicStatus");

            entity.Property(e => e.StatusDescription).HasMaxLength(50);
        });

        modelBuilder.Entity<Reply>(entity =>
        {
            entity.HasKey(e => e.ReplySid);

            entity.ToTable("Reply");

            entity.Property(e => e.ReplyDate).HasColumnType("datetime");
            entity.Property(e => e.ReplyUpdate).HasColumnType("datetime");

            entity.HasOne(d => d.ArticleCreaterS).WithMany(p => p.Replies)
                .HasForeignKey(d => d.ArticleCreaterSid)
                .HasConstraintName("FK_Reply_Member");

            entity.HasOne(d => d.ArticleS).WithMany(p => p.Replies)
                .HasForeignKey(d => d.ArticleSid)
                .HasConstraintName("FK_Reply_Article");

            entity.HasOne(d => d.PublicStatusNoNavigation).WithMany(p => p.Replies)
                .HasForeignKey(d => d.PublicStatusNo)
                .HasConstraintName("FK_Reply_PublicStatus");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Report");

            entity.Property(e => e.ReportSid).ValueGeneratedOnAdd();

            entity.HasOne(d => d.ReportReasonS).WithMany()
                .HasForeignKey(d => d.ReportReasonSid)
                .HasConstraintName("FK_Reprot_ReportReason");

            entity.HasOne(d => d.WebSiteTypeS).WithMany()
                .HasForeignKey(d => d.WebSiteTypeSid)
                .HasConstraintName("FK_Reprot_WebSiteType");
        });

        modelBuilder.Entity<ReportReason>(entity =>
        {
            entity.HasKey(e => e.ReportReasonSid);

            entity.ToTable("ReportReason");

            entity.Property(e => e.ReportReason1)
                .HasMaxLength(50)
                .HasColumnName("ReportReason");
        });

        modelBuilder.Entity<SalesBatch>(entity =>
        {
            entity.HasKey(e => e.PeriodSid);

            entity.ToTable(tb => tb.HasTrigger("trg_UpdateRunningStatus"));

            entity.Property(e => e.EndTime).HasPrecision(0);
            entity.Property(e => e.StartTime).HasPrecision(0);
        });

        modelBuilder.Entity<ScreenSummary>(entity =>
        {
            entity.HasKey(e => e.ScreenTextSid);

            entity.ToTable("ScreenSummary");

            entity.Property(e => e.ScreenTextCategory).HasMaxLength(50);
        });

        modelBuilder.Entity<ShoppingRank>(entity =>
        {
            entity.HasKey(e => e.ShoppingRankNo);

            entity.ToTable("ShoppingRank");

            entity.Property(e => e.Eggimage).HasColumnType("image");
            entity.Property(e => e.ShoppingRank1)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("ShoppingRank");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.StoreSid);

            entity.Property(e => e.Company).HasMaxLength(50);
            entity.Property(e => e.StoreImg).HasColumnType("image");
            entity.Property(e => e.StoreIntroduction).HasMaxLength(50);

            entity.HasOne(d => d.MemberS).WithMany(p => p.Stores)
                .HasForeignKey(d => d.MemberSid)
                .HasConstraintName("FK_Stores_Member");

            entity.HasOne(d => d.PublicStatusNoNavigation).WithMany(p => p.Stores)
                .HasForeignKey(d => d.PublicStatusNo)
                .HasConstraintName("FK_Stores_PublicStatus");
        });

        modelBuilder.Entity<StoreCoin>(entity =>
        {
            entity.HasKey(e => e.StoreCoinSid);

            entity.Property(e => e.ChangeTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Money).HasColumnType("money");

            entity.HasOne(d => d.AreaS).WithMany(p => p.StoreCoins)
                .HasForeignKey(d => d.AreaSid)
                .HasConstraintName("FK_StoreCoins_Orders");

            entity.HasOne(d => d.CoinUseAreaNoNavigation).WithMany(p => p.StoreCoins)
                .HasForeignKey(d => d.CoinUseAreaNo)
                .HasConstraintName("FK_StoreCoins_CoinUseAreas");

            entity.HasOne(d => d.MemberS).WithMany(p => p.StoreCoins)
                .HasForeignKey(d => d.MemberSid)
                .HasConstraintName("FK_StoreCoins_Member");
        });

        modelBuilder.Entity<SubscriptionMasterList>(entity =>
        {
            entity.HasKey(e => e.SubscriptionSid);

            entity.ToTable("SubscriptionMasterList");
        });

        modelBuilder.Entity<Talk>(entity =>
        {
            entity.HasKey(e => e.TalkSid);

            entity.ToTable("Talk");

            entity.Property(e => e.Attachment).HasColumnType("image");
            entity.Property(e => e.MessageContent).HasMaxLength(50);

            entity.HasOne(d => d.FriendS).WithMany(p => p.Talks)
                .HasForeignKey(d => d.FriendSid)
                .HasConstraintName("FK_Talk_Friend");
        });

        modelBuilder.Entity<Track>(entity =>
        {
            entity.HasKey(e => e.TrackSid);

            entity.Property(e => e.TrackingNum)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.OrderS).WithMany(p => p.Tracks)
                .HasForeignKey(d => d.OrderSid)
                .HasConstraintName("FK_Tracks_Orders");

            entity.HasOne(d => d.ReceiveSourceS).WithMany(p => p.TrackReceiveSources)
                .HasForeignKey(d => d.ReceiveSourceSid)
                .HasConstraintName("FK_Tracks_CarrierStores1");

            entity.HasOne(d => d.SendSouceS).WithMany(p => p.TrackSendSouces)
                .HasForeignKey(d => d.SendSouceSid)
                .HasConstraintName("FK_Tracks_CarrierStores");
        });

        modelBuilder.Entity<TrackStatus>(entity =>
        {
            entity.HasKey(e => e.TrackStatusNo);

            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<TrackTime>(entity =>
        {
            entity.HasKey(e => e.TrackTimeSid);

            entity.Property(e => e.CreatedTime).HasColumnType("datetime");

            entity.HasOne(d => d.TrackS).WithMany(p => p.TrackTimes)
                .HasForeignKey(d => d.TrackSid)
                .HasConstraintName("FK_TrackTimes_Tracks");

            entity.HasOne(d => d.TrackStatusNoNavigation).WithMany(p => p.TrackTimes)
                .HasForeignKey(d => d.TrackStatusNo)
                .HasConstraintName("FK_TrackTimes_TrackStatuses");
        });

        modelBuilder.Entity<Vaccinate>(entity =>
        {
            entity.HasKey(e => e.VaccinateSid);

            entity.ToTable("Vaccinate");

            entity.Property(e => e.VaccinateSid).ValueGeneratedNever();
            entity.Property(e => e.ChickLotNo).HasMaxLength(20);

            entity.HasOne(d => d.ChickS).WithMany(p => p.Vaccinates)
                .HasForeignKey(d => d.ChickSid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vaccinate_ChickLotNo");

            entity.HasOne(d => d.VaccineS).WithMany(p => p.Vaccinates)
                .HasForeignKey(d => d.VaccineSid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vaccinate_VaccineTable");
        });

        modelBuilder.Entity<VaccineTable>(entity =>
        {
            entity.HasKey(e => e.VaccineSid);

            entity.ToTable("VaccineTable");

            entity.Property(e => e.Cost).HasColumnType("money");
            entity.Property(e => e.VaccineName).HasMaxLength(20);
        });

        modelBuilder.Entity<VideoSummary>(entity =>
        {
            entity.HasKey(e => e.VideoSid);

            entity.ToTable("VideoSummary");

            entity.Property(e => e.AdSource).HasDefaultValue(false);
            entity.Property(e => e.InformationColumn).HasMaxLength(50);
            entity.Property(e => e.UploadDate).HasColumnType("datetime");
            entity.Property(e => e.VideoTitle).HasMaxLength(50);

            entity.HasOne(d => d.CreatorS).WithMany(p => p.VideoSummaries)
                .HasForeignKey(d => d.CreatorSid)
                .HasConstraintName("FK_VideoSummary_Creator");

            entity.HasOne(d => d.NatureS).WithMany(p => p.VideoSummaries)
                .HasForeignKey(d => d.NatureSid)
                .HasConstraintName("FK_VideoSummary_Nature");

            entity.HasOne(d => d.PublicStatusNoNavigation).WithMany(p => p.VideoSummaries)
                .HasForeignKey(d => d.PublicStatusNo)
                .HasConstraintName("FK_VideoSummary_PublicStatus");

            entity.HasOne(d => d.ScreenTextS).WithMany(p => p.VideoSummaries)
                .HasForeignKey(d => d.ScreenTextSid)
                .HasConstraintName("FK_VideoSummary_ScreenSummary");
        });

        modelBuilder.Entity<WebSiteType>(entity =>
        {
            entity.HasKey(e => e.WebSiteTypeSid);

            entity.ToTable("WebSiteType");

            entity.Property(e => e.WebSiteType1)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName("WebSiteType");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
