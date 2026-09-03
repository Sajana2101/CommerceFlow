using System;
using System.Collections.Generic;
using System.Text;
using CommerceFlow.Application.Products;
using CommerceFlow.Domain.Entities;

namespace CommerceFlow.Application.Carts
{
    public sealed class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(
            ICartRepository cartRepository,
            IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<CartResponse> GetAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var cart = await _cartRepository.GetAsync(
                customerId,
                cancellationToken);

            cart ??= new ShoppingCart(customerId);

            return await BuildResponseAsync(
                cart,
                cancellationToken);
        }

        public async Task<CartResponse> AddItemAsync(
            Guid customerId,
            AddCartItemRequest request,
            CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(
                request.ProductId,
                cancellationToken);

            if (product is null)
                throw new KeyNotFoundException(
                    "Product not found.");

            var cart = await _cartRepository.GetAsync(
                customerId,
                cancellationToken);

            cart ??= new ShoppingCart(customerId);

            cart.AddItem(
                request.ProductId,
                request.Quantity);

            await _cartRepository.SaveAsync(
                cart,
                cancellationToken);

            return await BuildResponseAsync(
                cart,
                cancellationToken);
        }

        public async Task<CartResponse?> UpdateItemAsync(
            Guid customerId,
            Guid productId,
            UpdateCartItemRequest request,
            CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(
                productId,
                cancellationToken);

            if (product is null)
                throw new KeyNotFoundException(
                    "Product not found.");

            var cart = await _cartRepository.GetAsync(
                customerId,
                cancellationToken);

            if (cart is null)
                return null;

            var updated = cart.UpdateItem(
                productId,
                request.Quantity);

            if (!updated)
                return null;

            await _cartRepository.SaveAsync(
                cart,
                cancellationToken);

            return await BuildResponseAsync(
                cart,
                cancellationToken);
        }

        public async Task<bool> RemoveItemAsync(
            Guid customerId,
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            var cart = await _cartRepository.GetAsync(
                customerId,
                cancellationToken);

            if (cart is null)
                return false;

            var removed = cart.RemoveItem(productId);

            if (!removed)
                return false;

            if (cart.Items.Count == 0)
            {
                await _cartRepository.DeleteAsync(
                    customerId,
                    cancellationToken);
            }
            else
            {
                await _cartRepository.SaveAsync(
                    cart,
                    cancellationToken);
            }

            return true;
        }

        public async Task ClearAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            await _cartRepository.DeleteAsync(
                customerId,
                cancellationToken);
        }

        private async Task<CartResponse> BuildResponseAsync(
            ShoppingCart cart,
            CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetByIdsAsync(
                cart.Items.Select(item => item.ProductId),
                cancellationToken);

            var productMap = products.ToDictionary(
                product => product.Id);

            var items = cart.Items
                .Where(item =>
                    productMap.ContainsKey(item.ProductId))
                .Select(item =>
                {
                    var product = productMap[item.ProductId];

                    return new CartItemResponse(
                        product.Id,
                        product.Name,
                        product.Sku,
                        product.Price,
                        item.Quantity,
                        product.Price * item.Quantity);
                })
                .ToArray();

            return new CartResponse(
                cart.CustomerId,
                items,
                items.Sum(item => item.Quantity),
                items.Sum(item => item.LineTotal));
        }
    }
}