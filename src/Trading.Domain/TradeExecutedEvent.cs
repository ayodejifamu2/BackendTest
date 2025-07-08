namespace Trading.Domain
{
    public class TradeExecutedEvent
    {
        public Guid TradeId { get; }
        public DateTime ExecutedAt { get; }

        public TradeExecutedEvent(Guid tradeId, DateTime executedAt)
        {
            TradeId = tradeId;
            ExecutedAt = executedAt;
        }
    }
} 