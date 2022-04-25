using RabbitMQ.Client;

namespace RabbitMQNet6.FileCreationWorkerService.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ILogger<RabbitMQClientService> _logger;
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        public const string QueueName = "queue-excel-file";

        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();

            if (_channel is { IsOpen: true }) //if (_channel.IsOpen)
            {
                return _channel;
            }

            _channel = _connection.CreateModel();



            _logger.LogInformation("Connected RabbitMQ");

            return _channel;
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();

            _connection?.Close();
            _connection?.Dispose();

            _logger.LogInformation("Disconnected RabbitMQ");
        }
    }
}
