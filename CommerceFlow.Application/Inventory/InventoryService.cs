using System;
using System.Collections.Generic;
using System.Text;
using CommerceFlow.Application.Products;
using CommerceFlow.Domain.Entities;

namespace CommerceFlow.Application.Inventory
{
    public sealed class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IProductRepository _productRepository;

        public InventoryService(
            IInventoryRepository inventoryRepository,
            IProductRepository productRepository)
        {
            _inventoryRepository = inventoryRepository;
            _productRepository = productRepository;
        }

        public async Task<InventoryResponse?> GetAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            var inventory =
                await _inventoryRepository.GetByProductIdAsync(
                    productId,
                    cancellationToken);

            return inventory is null
                ? null
                : ToResponse(inventory);
        }

        public async Task<InventoryResponse> SetAvailableQuantityAsync(
            Guid productId,
            UpdateInventoryRequest request,
            CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(
                productId,
                cancellationToken);

            if (product is null)
                throw new KeyNotFoundException(
                    "Product not found.");

            var inventory =
                await _inventoryRepository.GetByProductIdForUpdateAsync(
                    productId,
                    cancellationToken);

            if (inventory is null)
            {
                inventory = new InventoryItem(productId);

                inventory.SetAvailableQuantity(
                    request.AvailableQuantity);

                await _inventoryRepository.AddAsync(
                    inventory,
                    cancellationToken);
            }
            else
            {
                inventory.SetAvailableQuantity(
                    request.AvailableQuantity);

                await _inventoryRepository.SaveChangesAsync(
                    cancellationToken);
            }

            return ToResponse(inventory);
        }

        private static InventoryResponse ToResponse(
            InventoryItem inventory)
        {
            return new InventoryResponse(
                inventory.ProductId,
                inventory.AvailableQuantity,
                inventory.ReservedQuantity,
                inventory.TotalQuantity);
        }
    }
}