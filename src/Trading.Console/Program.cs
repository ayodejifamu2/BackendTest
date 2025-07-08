using System;
using System.Threading;
using System.Threading.Tasks;
using Trading.Messaging.Contracts;

namespace Trading.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            global::System.Console.WriteLine("[Console] Starting RabbitMQ consumer. Press Ctrl+C to exit.\n");

            var cts = new CancellationTokenSource();
            global::System.Console.CancelKeyPress += (s, e) => {
                e.Cancel = true;
                cts.Cancel();
            };

            using var consumer = new RabbitMqTradeMessageConsumer();

            consumer.StartConsuming(message =>
            {
                global::System.Console.WriteLine($"[Trade] Id={message.TradeId}, User={message.UserId}, Asset={message.Asset}, Qty={message.Quantity}, Price={message.Price}, Type={message.TradeType}, Time={message.Timestamp}, Status={message.Status}, FailureReason={message.FailureReason}");
            }, cts.Token);

            global::System.Console.WriteLine("[Console] Exiting.");
        }
    }
}
