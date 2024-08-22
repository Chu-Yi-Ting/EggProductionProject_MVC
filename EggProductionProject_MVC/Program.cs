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
                options.Cookie.Name = "EggProductionProject_MVCAuthCookie";  // �T�O�����Ϊ� Cookie �W�٤@�P
                options.Cookie.Path = "/";  // �N Cookie �����|�]�m���ڡA�o�˥��b�Ҧ� Area ��������
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.LoginPath = "/AccountLogin/Login";  // �n�J�������|
                options.LogoutPath = "/AccountLogin/Logout";  // �n�X�������|
				options.Cookie.SameSite = SameSiteMode.None;
				options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // �]�m�� None �ɻݭn Secure
			});

        builder.Services.AddControllersWithViews();
var authProperties = new AuthenticationProperties
{
    IsPersistent = true, // �O�_���[�� Cookie
    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // �]�m Cookie �L���ɶ�
};






//builder.Services.AddDefaultIdentity<EggUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
//builder.Services.AddControllersWithViews();
builder.Services.AddIdentity<EggUser, IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// �`�U�ۭq�� EmailSender �A��
builder.Services.AddTransient<IEmailSender, EmailSender>();

// ���U���v�A��
builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();


// ���URazor Pages�A��
builder.Services.AddRazorPages();




var app = builder.Build();

//app.UseCookiePolicy(new CookiePolicyOptions
//{
//    MinimumSameSitePolicy = SameSiteMode.Lax, // �Ϊ� None
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

//�|���n�Y��name�n�o��
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
