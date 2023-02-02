using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using AuthMvc.Data;
using AuthMvc.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
    builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedEmail = false;
        options.Password.RequireDigit = false;
    }).AddEntityFrameworkStores<ApplicationDbContext>();

    builder.Services.ConfigureApplicationCookie(options =>
        options.LoginPath = "/Auth/Login"
    );

    // Add services to the container.
    builder.Services.AddNotyf(config => { config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.BottomRight; });
    builder.Services.AddControllersWithViews();

}

var app = builder.Build();
{
    app.UseNotyf();
    app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}

