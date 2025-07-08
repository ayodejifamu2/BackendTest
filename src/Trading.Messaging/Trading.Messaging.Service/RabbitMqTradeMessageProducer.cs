using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Trading.Messaging.Contracts;

namespace Trading.Messaging.Service
{
    public class RabbitMqTradeMessageProducer : ITradeMessageProducer, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string QueueName = "trades.executed";

        public RabbitMqTradeMessageProducer(IConfiguration configuration)
        {
            var hostName = configuration["RabbitMQ:HostName"] ?? "localhost";
            var userName = configuration["RabbitMQ:UserName"] ?? "guest";
            var password = configuration["RabbitMQ:Password"] ?? "guest";
            var port = int.TryParse(configuration["RabbitMQ:Port"], out var portValue) ? portValue : 5672;
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password,
                Port = port
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            
            _channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public Task PublishTradeExecutedAsync(TradeExecutedMessage message)
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(
                exchange: "",
                routingKey: QueueName,
                basicProperties: null,
                body: body);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
} 