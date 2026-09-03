namespace CommerceFlow.Domain.Entities
{
    public sealed class ShoppingCartItem
    {
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }

        public ShoppingCartItem(
            Guid productId,
            int quantity)
        {
            ValidateQuantity(quantity);

            ProductId = productId;
            Quantity = quantity;
        }

        public void IncreaseQuantity(int quantity)
        {
            ValidateQuantity(quantity);

            var newQuantity = Quantity + quantity;

            if (newQuantity > 50)
                throw new ArgumentException(
                    "A cart item cannot exceed 50 units.");

            Quantity = newQuantity;
        }

        public void SetQuantity(int quantity)
        {
            ValidateQuantity(quantity);

            Quantity = quantity;
        }

        private static void ValidateQuantity(int quantity)
        {
            if (quantity < 1 || quantity > 50)
                throw new ArgumentException(
                    "Quantity must be between 1 and 50.");
        }
    }
}