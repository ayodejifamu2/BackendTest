using MediatR;
using Trading.Application.DTOs;

namespace Trading.Application.Commands
{
    public class ExecuteTradeCommand : IRequest<TradeDto>
    {
        public required string UserId { get; set; }
        public required string Asset { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public required string TradeType { get; set; }
    }
} 