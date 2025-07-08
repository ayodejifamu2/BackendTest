namespace Trading.Domain
{
    public class Trade
    {
        public Guid TradeId { get; private set; }
        public Guid UserId { get; private set; }
        public string Asset { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal Price { get; private set; }
        public TradeType TradeType { get; private set; }
        public DateTime Timestamp { get; private set; }
        public TradeStatus Status { get; private set; }
        public string? FailureReason { get; private set; }

        private Trade(Guid tradeId, Guid userId, string asset, decimal quantity, decimal price, TradeType tradeType, DateTime timestamp, TradeStatus status)
        {
            TradeId = tradeId;
            UserId = userId;
            Asset = asset;
            Quantity = quantity;
            Price = price;
            TradeType = tradeType;
            Timestamp = timestamp;
            Status = status;
        }

        public static Trade Create(Guid userId, string asset, decimal quantity, decimal price, TradeType tradeType)
        {
            if (string.IsNullOrWhiteSpace(asset))
                throw new ArgumentException("Asset cannot be null or empty.", nameof(asset));
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive.", nameof(quantity));
            if (price <= 0)
                throw new ArgumentException("Price must be positive.", nameof(price));

            return new Trade(Guid.NewGuid(), userId, asset, quantity, price, tradeType, DateTime.UtcNow, TradeStatus.Pending);
        }

        public void MarkAsExecuted()
        {
            if (Status != TradeStatus.Pending)
                throw new InvalidOperationException("Only pending trades can be executed.");
            Status = TradeStatus.Executed;
        }

        public void MarkAsFailed(string reason)
        {
            if (Status != TradeStatus.Pending)
                throw new InvalidOperationException("Only pending trades can be failed.");
            Status = TradeStatus.Failed;
            FailureReason = reason;
        }

        public void Cancel()
        {
            if (Status != TradeStatus.Pending)
                throw new InvalidOperationException("Only pending trades can be cancelled.");
            Status = TradeStatus.Failed;
            FailureReason = "Cancelled by user.";
        }
    }

    public enum TradeType
    {
        Buy,
        Sell
    }

    public enum TradeStatus
    {
        Pending,
        Executed,
        Failed
    }
}
