namespace CommerceFlow.Domain.Entities
{
    public sealed class ShoppingCart
    {
        private readonly List<ShoppingCartItem> _items = new();

        public Guid CustomerId { get; }

        public IReadOnlyCollection<ShoppingCartItem> Items =>
            _items.AsReadOnly();

        public ShoppingCart(Guid customerId)
        {
            CustomerId = customerId;
        }

        public void AddItem(
            Guid productId,
            int quantity)
        {
            var existingItem = _items.FirstOrDefault(
                item => item.ProductId == productId);

            if (existingItem is not null)
            {
                existingItem.IncreaseQuantity(quantity);
                return;
            }

            if (_items.Count >= 100)
                throw new InvalidOperationException(
                    "Cart cannot contain more than 100 different products.");

            _items.Add(
                new ShoppingCartItem(
                    productId,
                    quantity));
        }

        public bool UpdateItem(
            Guid productId,
            int quantity)
        {
            var item = _items.FirstOrDefault(
                item => item.ProductId == productId);

            if (item is null)
                return false;

            item.SetQuantity(quantity);

            return true;
        }

        public bool RemoveItem(Guid productId)
        {
            var item = _items.FirstOrDefault(
                item => item.ProductId == productId);

            if (item is null)
                return false;

            _items.Remove(item);

            return true;
        }
    }
}