using System;
using System.Collections.Generic;
using System.Text;
using CommerceFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceFlow.Infrastructure.Persistence.Configurations
{
    public sealed class OrderItemConfiguration
        : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(
            EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.HasKey(item => item.Id);

            builder.Property(item => item.ProductName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(item => item.Sku)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(item => item.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(item => item.Quantity)
                .IsRequired();

            builder.Ignore(item => item.LineTotal);

            builder.HasOne<Product>()
                .WithMany()
                .HasForeignKey(item => item.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}