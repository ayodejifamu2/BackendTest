using MediatR;
using Trading.Application.DTOs;

namespace Trading.Application.Queries
{
    public class GetTradesQuery : IRequest<List<TradeDto>>;
} 