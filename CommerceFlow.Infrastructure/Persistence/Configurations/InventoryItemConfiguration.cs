using System;
using System.Collections.Generic;
using System.Text;
using CommerceFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceFlow.Infrastructure.Persistence.Configurations
{
    public sealed class InventoryItemConfiguration
        : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(
            EntityTypeBuilder<InventoryItem> builder)
        {
            builder.ToTable("InventoryItems");

            builder.HasKey(
                inventory => inventory.ProductId);

            builder.Property(
                    inventory => inventory.AvailableQuantity)
                .IsRequired();

            builder.Property(
                    inventory => inventory.ReservedQuantity)
                .IsRequired();

            builder.Ignore(
                inventory => inventory.TotalQuantity);

            builder.HasOne<Product>()
                .WithOne()
                .HasForeignKey<InventoryItem>(
                    inventory => inventory.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
