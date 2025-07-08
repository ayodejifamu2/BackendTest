using Trading.Api.Models;

namespace Trading.Api.Mappers
{
    public static class TradeApiValidator
    {
        public static void Validate(ExecuteTradeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
                throw new ArgumentException("UserId is required.");
            if (string.IsNullOrWhiteSpace(request.Asset))
                throw new ArgumentException("Asset is required.");
            if (request.Quantity <= 0)
                throw new ArgumentException("Quantity must be positive.");
            if (request.Price <= 0)
                throw new ArgumentException("Price must be positive.");
            if (string.IsNullOrWhiteSpace(request.TradeType))
                throw new ArgumentException("TradeType is required.");
        }
    }
} 