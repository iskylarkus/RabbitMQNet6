using RabbitMQ.Client;
using RabbitMQNet6.FileCreationWorkerService;
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

        services.AddSingleton<RabbitMQClientService>();
    })
    .Build();

await host.RunAsync();
