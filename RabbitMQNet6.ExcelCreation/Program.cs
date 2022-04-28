using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQNet6.ExcelCreation.Hubs;
using RabbitMQNet6.ExcelCreation.Models;
using RabbitMQNet6.ExcelCreation.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton(sp => new ConnectionFactory()
{
    Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ")),
    DispatchConsumersAsync = true
});


builder.Services.AddSingleton<RabbitMQClientService>();
builder.Services.AddSingleton<RabbitMQPublisher>();



builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>();



builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();

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


app.MapHub<MyHub>("/MyHub");


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
