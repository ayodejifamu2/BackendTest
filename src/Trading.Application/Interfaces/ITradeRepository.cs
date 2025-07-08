using Trading.Domain;

namespace Trading.Application.Interfaces
{
    public interface ITradeRepository
    {
        Task<Trade> AddAsync(Trade trade);
        Task<List<Trade>> GetAllAsync();
    }
} 