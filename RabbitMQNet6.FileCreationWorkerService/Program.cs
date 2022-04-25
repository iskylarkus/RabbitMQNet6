using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQNet6.FileCreationWorkerService;
using RabbitMQNet6.FileCreationWorkerService.Models;
using RabbitMQNet6.FileCreationWorkerService.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();

        services.AddSingleton(sp => new ConnectionFactory()
        {
            Uri = new Uri(hostContext.Configuration.GetConnectionString("RabbitMQ")),
            DispatchConsumersAsync = true
        });

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("SqlServer"));
        });

        services.AddSingleton<RabbitMQClientService>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    context.Database.Migrate();

    if (!context.Products.Any())
    {
        Enumerable.Range(1, 30).ToList().ForEach(x =>
        {
            context.Products.Add(new Product() { Name = $"Kalem {x}", Price = x * 19, Stock = x * 11 });
        });

        context.SaveChanges();
    }
}

await host.RunAsync();
