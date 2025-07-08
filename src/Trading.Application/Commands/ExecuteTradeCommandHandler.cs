using MediatR;
using Trading.Application.DTOs;
using Trading.Application.Interfaces;
using Trading.Domain;
using Trading.Messaging.Service;
using Trading.Messaging.Contracts;
using Mapster;

namespace Trading.Application.Commands
{
    public class ExecuteTradeCommandHandler(ITradeRepository tradeRepository, ITradeMessageProducer messageProducer)
        : IRequestHandler<ExecuteTradeCommand, TradeDto>
    {
        public async Task<TradeDto> Handle(ExecuteTradeCommand request, CancellationToken cancellationToken)
        {
            var trade = Trade.Create(
                Guid.Parse(request.UserId),
                request.Asset,
                request.Quantity,
                request.Price,
                Enum.Parse<TradeType>(request.TradeType, true)
            );

            var savedTrade = await tradeRepository.AddAsync(trade);

            var dto = new TradeDto
            {
                TradeId = savedTrade.TradeId.ToString(),
                UserId = savedTrade.UserId.ToString(),
                Asset = savedTrade.Asset,
                Quantity = savedTrade.Quantity,
                Price = savedTrade.Price,
                TradeType = savedTrade.TradeType.ToString(),
                Timestamp = savedTrade.Timestamp.ToString("o"),
                Status = savedTrade.Status.ToString(),
                FailureReason = savedTrade.FailureReason
            };

            var message = dto.Adapt<TradeExecutedMessage>();
            await messageProducer.PublishTradeExecutedAsync(message);

            return dto;
        }
    }
} 