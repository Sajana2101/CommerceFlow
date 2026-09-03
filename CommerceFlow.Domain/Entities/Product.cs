namespace CommerceFlow.Domain.Entities
{
    public sealed class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Sku { get; private set; }
        public decimal Price { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        private Product()
        {
            Name = string.Empty;
            Description = string.Empty;
            Sku = string.Empty;
        }

        public Product(
            string name,
            string description,
            string sku,
            decimal price)
        {
            ValidateName(name);
            ValidatePrice(price);

            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException(
                    "SKU is required.",
                    nameof(sku));

            Id = Guid.NewGuid();
            Name = name.Trim();
            Description = (description ?? string.Empty).Trim();
            Sku = sku.Trim().ToUpperInvariant();
            Price = price;
            IsActive = true;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public void UpdateDetails(
            string name,
            string description,
            decimal price)
        {
            ValidateName(name);
            ValidatePrice(price);

            Name = name.Trim();
            Description = (description ?? string.Empty).Trim();
            Price = price;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "Product name is required.",
                    nameof(name));
        }

        private static void ValidatePrice(decimal price)
        {
            if (price < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(price),
                    "Price cannot be negative.");
        }
    }
}