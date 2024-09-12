using EggProductionProject_MVC.Data;
using EggProductionProject_MVC.Models;
using EggProductionProject_MVC.Models.MemberVM;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.HTTPModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//跨域測試
builder.Services.AddCors(option =>
{
    option.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
//跨域測試
//關閉傳送Json駝峰式命名
//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        // 關閉默認的駝峰命名規則，保持屬性名稱與 C# 模型一致
//        options.JsonSerializerOptions.PropertyNamingPolicy = null;
//    });
//關閉傳送Json駝峰式命名

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDbContext<EggPlatformContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("EggPlatform"));
});

builder.Services.AddTransient<IEmailSender, EmailSender>();



builder.Services.AddDefaultIdentity<IdentityUser>(options => 
options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();




////設定會員登入失敗會鎖住
//builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
//{
//    options.SignIn.RequireConfirmedAccount = true;
//    鎖定配置
//    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1); //// 鎖定時間，我改成1分鐘
//    options.Lockout.MaxFailedAccessAttempts = 5; //// 最大登入失敗次數
//    options.Lockout.AllowedForNewUsers = true; //// 是否允許新用戶被鎖定
//})
//.AddEntityFrameworkStores<ApplicationDbContext>()
//.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();


//留給前台會員用
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//            .AddCookie(options =>
//            {
//                options.LoginPath = "/AccountLogin/Login"; // �n�J�������|
//                options.LogoutPath = "/AccountLogin/Logout"; // �n�X�������|
//            });

//        builder.Services.AddControllersWithViews();
//var authProperties = new AuthenticationProperties
//{
//    //IsPersistent = true, // �O�_���[�� Cookie
//    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // �]�m Cookie �L���ɶ�
//};


builder.Services.AddSession(options =>
{
    options.Cookie.Name = "GoodEgg.Session";
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
});



// �`�U�ۭq�� EmailSender �A��
builder.Services.AddTransient<IEmailSender, EmailSender>();

// ���U���v�A��
builder.Services.AddAuthorization();

builder.Services.AddHttpClient<WebScrapingService>();
builder.Services.AddControllersWithViews();


// ���URazor Pages�A��
builder.Services.AddRazorPages();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
//跨域測試
app.UseCors("AllowAll");
//跨域測試
app.UseStaticFiles();









app.UseRouting();

app.UseAuthorization();

app.UseSession();


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=BkHome}/{action=Index}/{id?}");
app.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=FtHome}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
