using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Carts
{
    public interface ICartService
    {
        Task<CartResponse> GetAsync(
            Guid customerId,
            CancellationToken cancellationToken = default);

        Task<CartResponse> AddItemAsync(
            Guid customerId,
            AddCartItemRequest request,
            CancellationToken cancellationToken = default);

        Task<CartResponse?> UpdateItemAsync(
            Guid customerId,
            Guid productId,
            UpdateCartItemRequest request,
            CancellationToken cancellationToken = default);

        Task<bool> RemoveItemAsync(
            Guid customerId,
            Guid productId,
            CancellationToken cancellationToken = default);

        Task ClearAsync(
            Guid customerId,
            CancellationToken cancellationToken = default);
    }
}