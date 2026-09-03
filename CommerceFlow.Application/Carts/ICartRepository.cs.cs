using System;
using System.Collections.Generic;
using System.Text;
using CommerceFlow.Domain.Entities;

namespace CommerceFlow.Application.Carts
{
    public interface ICartRepository
    {
        Task<ShoppingCart?> GetAsync(
            Guid customerId,
            CancellationToken cancellationToken = default);

        Task SaveAsync(
            ShoppingCart cart,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            Guid customerId,
            CancellationToken cancellationToken = default);
    }
}