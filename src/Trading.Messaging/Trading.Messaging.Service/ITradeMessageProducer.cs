using Trading.Messaging.Contracts;

namespace Trading.Messaging.Service
{
    public interface ITradeMessageProducer
    {
        Task PublishTradeExecutedAsync(TradeExecutedMessage message);
    }
} 