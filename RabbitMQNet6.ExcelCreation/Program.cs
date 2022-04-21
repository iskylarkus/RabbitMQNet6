using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RabbitMQNet6.ExcelCreation.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

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



using (var scope = app.Services.CreateScope())
{
    var identityDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    identityDbContext.Database.Migrate();

    if (!userManager.Users.Any())
    {
        userManager.CreateAsync(new IdentityUser() { UserName = "user1", Email = "user1@hotmail.com" }, "Password12*").Wait();
        userManager.CreateAsync(new IdentityUser() { UserName = "user2", Email = "user2@hotmail.com" }, "Password12*").Wait();
        userManager.CreateAsync(new IdentityUser() { UserName = "user3", Email = "user3@hotmail.com" }, "Password12*").Wait();
        userManager.CreateAsync(new IdentityUser() { UserName = "user4", Email = "user4@hotmail.com" }, "Password12*").Wait();
        userManager.CreateAsync(new IdentityUser() { UserName = "user5", Email = "user5@hotmail.com" }, "Password12*").Wait();
    }
}

app.Run();
