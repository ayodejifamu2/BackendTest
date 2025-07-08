namespace Trading.Messaging.Contracts
{
    public class TradeExecutedMessage
    {
        public string TradeId { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string Asset { get; set; } = default!;
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string TradeType { get; set; } = default!;
        public string Timestamp { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string? FailureReason { get; set; }
    }
} 