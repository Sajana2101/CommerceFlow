using System.Text.Json;
using CommerceFlow.Application.Carts;
using CommerceFlow.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;

namespace CommerceFlow.Infrastructure.Carts
{
    public sealed class RedisCartRepository : ICartRepository
    {
        private readonly IDistributedCache _cache;

        public RedisCartRepository(
            IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<ShoppingCart?> GetAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var json = await _cache.GetStringAsync(
                GetKey(customerId),
                cancellationToken);

            if (string.IsNullOrWhiteSpace(json))
                return null;

            var cachedCart =
                JsonSerializer.Deserialize<CartCacheModel>(json);

            if (cachedCart is null)
                return null;

            var cart = new ShoppingCart(
                cachedCart.CustomerId);

            foreach (var item in cachedCart.Items)
            {
                cart.AddItem(
                    item.ProductId,
                    item.Quantity);
            }

            return cart;
        }

        public async Task SaveAsync(
            ShoppingCart cart,
            CancellationToken cancellationToken = default)
        {
            var cachedCart = new CartCacheModel(
                cart.CustomerId,
                cart.Items
                    .Select(item =>
                        new CartCacheItemModel(
                            item.ProductId,
                            item.Quantity))
                    .ToArray());

            var json = JsonSerializer.Serialize(
                cachedCart);

            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(7)
            };

            await _cache.SetStringAsync(
                GetKey(cart.CustomerId),
                json,
                options,
                cancellationToken);
        }

        public async Task DeleteAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            await _cache.RemoveAsync(
                GetKey(customerId),
                cancellationToken);
        }

        private static string GetKey(Guid customerId)
        {
            return $"cart:{customerId}";
        }

        private sealed record CartCacheModel(
            Guid CustomerId,
            IReadOnlyCollection<CartCacheItemModel> Items);

        private sealed record CartCacheItemModel(
            Guid ProductId,
            int Quantity);
    }
}