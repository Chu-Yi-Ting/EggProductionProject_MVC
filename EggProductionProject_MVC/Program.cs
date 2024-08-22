using EggProductionProject_MVC.Data;
using EggProductionProject_MVC.Models;
using EggProductionProject_MVC.Models.MemberVM;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDbContext<EggPlatformContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("EggPlatform"));
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "EggProductionProject_MVCAuthCookie";  // 確保全應用的 Cookie 名稱一致
                options.Cookie.Path = "/";  // 將 Cookie 的路徑設置為根，這樣它在所有 Area 中都有效
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.LoginPath = "/AccountLogin/Login";  // 登入頁面路徑
                options.LogoutPath = "/AccountLogin/Logout";  // 登出頁面路徑
				options.Cookie.SameSite = SameSiteMode.None;
				options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // 設置為 None 時需要 Secure
			});

        builder.Services.AddControllersWithViews();
var authProperties = new AuthenticationProperties
{
    IsPersistent = true, // 是否持久化 Cookie
    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // 設置 Cookie 過期時間
};






//builder.Services.AddDefaultIdentity<EggUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
//builder.Services.AddControllersWithViews();
builder.Services.AddIdentity<EggUser, IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// 注冊自訂的 EmailSender 服務
builder.Services.AddTransient<IEmailSender, EmailSender>();

// 註冊授權服務
builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();


// 註冊Razor Pages服務
builder.Services.AddRazorPages();




var app = builder.Build();

//app.UseCookiePolicy(new CookiePolicyOptions
//{
//    MinimumSameSitePolicy = SameSiteMode.Lax, // 或者 None
//});



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
app.UseStaticFiles();

app.UseRouting();

//會員要吃到name要這行
app.UseAuthentication();
app.UseAuthorization();

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
