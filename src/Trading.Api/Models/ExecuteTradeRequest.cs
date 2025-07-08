using System.ComponentModel.DataAnnotations;

namespace Trading.Api.Models
{
    public class ExecuteTradeRequest
    {
        [Required]
        public required string UserId { get; set; }
        [Required]
        public required string Asset { get; set; }
        [Range(0.0001, double.MaxValue)]
        public decimal Quantity { get; set; }
        [Range(0.0001, double.MaxValue)]
        public decimal Price { get; set; }
        [Required]
        public required string TradeType { get; set; }
    }
} 