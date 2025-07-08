using Mapster;
using Trading.Api.Models;
using Trading.Application.Commands;
using Trading.Application.DTOs;

namespace Trading.Api.Mappers
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ExecuteTradeRequest, ExecuteTradeCommand>();
            config.NewConfig<TradeDto, TradeResponse>();
        }
    }
} 