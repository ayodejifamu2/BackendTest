using Microsoft.EntityFrameworkCore;
using Trading.Application.Interfaces;
using Trading.Domain;
using Trading.Infrastructure.Data;

namespace Trading.Infrastructure.Repositories
{
    public class TradeRepository(TradingDbContext context) : ITradeRepository
    {
        public async Task<Trade> AddAsync(Trade trade)
        {
            await context.Trades.AddAsync(trade);
            await context.SaveChangesAsync();
            return trade;
        }

        public async Task<List<Trade>> GetAllAsync()
        {
            return await context.Trades.ToListAsync();
        }
    }
} 