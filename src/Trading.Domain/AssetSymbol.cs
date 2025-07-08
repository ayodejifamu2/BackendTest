namespace Trading.Domain
{
    public class AssetSymbol
    {
        public string Value { get; }

        public AssetSymbol(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Asset symbol cannot be empty.");
            Value = value.ToUpperInvariant();
        }

        public override bool Equals(object obj) => obj is AssetSymbol other && Value == other.Value;
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
    }
} 