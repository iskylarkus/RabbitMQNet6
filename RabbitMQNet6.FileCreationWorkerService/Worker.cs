using ClosedXML.Excel;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQNet6.FileCreationWorkerService.Models;
using RabbitMQNet6.FileCreationWorkerService.Services;
using RabbitMQNet6.Shared;
using System.Data;
using System.Text;
using System.Text.Json;

namespace RabbitMQNet6.FileCreationWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly IServiceProvider _serviceProvider;

        private readonly RabbitMQClientService _rabbitMQClientService;

        private IModel _channel;

        public Worker(ILogger<Worker> logger, RabbitMQClientService rabbitMQClientService, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _rabbitMQClientService = rabbitMQClientService;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMQClientService.Connect();

            _channel.BasicQos(0, 1, false);

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            _channel.BasicConsume(RabbitMQClientService.QueueName, false, consumer);

            consumer.Received += Consumer_Received;

            await Task.CompletedTask;
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            await Task.Delay(5000);

            string body = Encoding.UTF8.GetString(@event.Body.ToArray());

            var createdExcelMessage = JsonSerializer.Deserialize<CreatedExcelMessage>(body);


            using var ms = new MemoryStream();

            var wb = new XLWorkbook();

            var ds = new DataSet();

            ds.Tables.Add(GetTable("products"));

            wb.Worksheets.Add(ds);

            wb.SaveAs(ms);


            MultipartFormDataContent multipartFormDataContent = new();

            multipartFormDataContent.Add(new ByteArrayContent(ms.ToArray()), "file", Guid.NewGuid().ToString() + ".xlsx");


            var baseUrl = $"https://localhost:7064/api/files?fileId={createdExcelMessage.FileId}";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(baseUrl, multipartFormDataContent);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"File Id : {createdExcelMessage.FileId} was created by successful");

                    _channel.BasicAck(@event.DeliveryTag, false);
                }
            }
        }

        private DataTable GetTable(string tableName)
        {
            List<Product> products;

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                products = context.Products.ToList();
            }

            DataTable table = new DataTable() { TableName = tableName };
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Price", typeof(decimal));
            table.Columns.Add("Stock", typeof(int));

            products.ForEach(x =>
            {
                table.Rows.Add(x.Id, x.Name, x.Price, x.Stock);
            });

            return table;
        }
    }
}
