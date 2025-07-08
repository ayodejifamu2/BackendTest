namespace Trading.Api.Models
{
    public class TradeResponse
    {
        public required string TradeId { get; set; }
        public required string UserId { get; set; }
        public required string Asset { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public required string TradeType { get; set; }
        public required string Timestamp { get; set; }
        public required string Status { get; set; }
        public string? FailureReason { get; set; }
    }
} 