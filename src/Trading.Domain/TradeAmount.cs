namespace Trading.Domain
{
    public class TradeAmount
    {
        public decimal Quantity { get; }
        public decimal Price { get; }

        public TradeAmount(decimal quantity, decimal price)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
            if (price <= 0) throw new ArgumentException("Price must be positive.");
            Quantity = quantity;
            Price = price;
        }

        public override bool Equals(object obj) =>
            obj is TradeAmount other && Quantity == other.Quantity && Price == other.Price;
        public override int GetHashCode() => HashCode.Combine(Quantity, Price);
        public override string ToString() => $"{Quantity} @ {Price}";
    }
} 