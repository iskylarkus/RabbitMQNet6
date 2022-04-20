using RabbitMQ.Client;

namespace RabbitMQNet6.Watermark.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ILogger<RabbitMQClientService> _logger;
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        public static string ExchangeName = "direct-watermark-image";
        public static string RouteKey = "route-watermark-image";
        public static string QueueName = "queue-watermark-image";

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

            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, true, false);

            _channel.QueueDeclare(QueueName, true, false, false, null);

            _channel.QueueBind(QueueName, ExchangeName, RouteKey, null);

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
