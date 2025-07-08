using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Trading.Messaging.Contracts;

namespace Trading.Console
{
    public class RabbitMqTradeMessageConsumer : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string QueueName = "trades.executed";

        public RabbitMqTradeMessageConsumer(string hostName = "localhost", string userName = "guest", string password = "guest")
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password
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

        public void StartConsuming(Action<TradeExecutedMessage> onMessageReceived, CancellationToken cancellationToken = default)
        {
            var consumer = new EventingBasicConsumer(_channel);
            
            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var tradeMessage = JsonSerializer.Deserialize<TradeExecutedMessage>(message);
                    
                    if (tradeMessage != null)
                    {
                        onMessageReceived(tradeMessage);
                    }
                    
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"[Error] Failed to process message: {ex.Message}");
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(
                queue: QueueName,
                autoAck: false,
                consumer: consumer);

            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
} 