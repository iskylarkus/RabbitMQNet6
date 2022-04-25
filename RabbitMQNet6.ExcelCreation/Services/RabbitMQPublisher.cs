﻿using RabbitMQ.Client;
using RabbitMQNet6.Shared;
using System.Text;
using System.Text.Json;

namespace RabbitMQNet6.ExcelCreation.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService _rabbitMQClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitMQClientService)
        {
            _rabbitMQClientService = rabbitMQClientService;
        }

        public void Publish(CreatedExcelMessage createdExcelMessage)
        {
            var channel = _rabbitMQClientService.Connect();

            var bodyJson = JsonSerializer.Serialize(createdExcelMessage);

            var bodyByte = Encoding.UTF8.GetBytes(bodyJson);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: RabbitMQClientService.ExchangeName,
                routingKey: RabbitMQClientService.RouteKey,
                basicProperties: properties,
                body: bodyByte);
        }
    }
}
