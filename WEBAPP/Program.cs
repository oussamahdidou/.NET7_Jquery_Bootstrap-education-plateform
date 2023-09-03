using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WEBAPP.Data;

using WEBAPP.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//DATABASE CONFIGURATION 
builder.Services.AddDbContext<Database>(options =>
{
   
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
;
});



//Auth config
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<Database>();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
var app = builder.Build();
//Seed Config
if (args.Length >= 2 && args[0].Length == 1 && args[1].ToLower() == "seeddata")
{
    await Seed.SeedUsersAndRolesAsync(app);
}
else
{
    Console.WriteLine("Invalid arguments or missing command.");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
