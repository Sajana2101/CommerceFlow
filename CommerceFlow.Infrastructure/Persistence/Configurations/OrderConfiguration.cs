using System;
using System.Collections.Generic;
using System.Text;
using CommerceFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceFlow.Infrastructure.Persistence.Configurations
{
    public sealed class OrderConfiguration
        : IEntityTypeConfiguration<Order>
    {
        public void Configure(
            EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(order => order.Id);

            builder.Property(order => order.OrderNumber)
                .HasMaxLength(40)
                .IsRequired();

            builder.HasIndex(order => order.OrderNumber)
                .IsUnique();

            builder.Property(order => order.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(order => order.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(order => order.CreatedAtUtc)
                .IsRequired();

            builder.HasOne<Customer>()
                .WithMany()
                .HasForeignKey(order => order.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(order => order.Items)
                .WithOne()
                .HasForeignKey(item => item.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(order => order.Items)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
