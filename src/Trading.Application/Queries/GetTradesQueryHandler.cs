using MediatR;
using Trading.Application.DTOs;
using Trading.Application.Interfaces;

namespace Trading.Application.Queries
{
    public class GetTradesQueryHandler(ITradeRepository tradeRepository)
        : IRequestHandler<GetTradesQuery, List<TradeDto>>
    {
        public async Task<List<TradeDto>> Handle(GetTradesQuery request, CancellationToken cancellationToken)
        {
            var trades = await tradeRepository.GetAllAsync();
            return trades.Select(trade => new TradeDto
            {
                TradeId = trade.TradeId.ToString(),
                UserId = trade.UserId.ToString(),
                Asset = trade.Asset,
                Quantity = trade.Quantity,
                Price = trade.Price,
                TradeType = trade.TradeType.ToString(),
                Timestamp = trade.Timestamp.ToString("o"),
                Status = trade.Status.ToString(),
                FailureReason = trade.FailureReason
            }).ToList();
        }
    }
} 