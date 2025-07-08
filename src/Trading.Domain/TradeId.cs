namespace Trading.Domain
{
    public class TradeId
    {
        public Guid Value { get; }

        public TradeId(Guid value)
        {
            if (value == Guid.Empty) throw new ArgumentException("TradeId cannot be empty.");
            Value = value;
        }

        public override bool Equals(object obj) => obj is TradeId other && Value == other.Value;
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString();
    }
} 