using System;
using System.Collections.Generic;
using System.Text;
using CommerceFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceFlow.Infrastructure.Persistence.Configurations
{
    public sealed class PaymentConfiguration
        : IEntityTypeConfiguration<Payment>
    {
        public void Configure(
            EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(payment => payment.Id);

            builder.Property(payment => payment.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(payment => payment.IdempotencyKey)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(payment => payment.IdempotencyKey)
                .IsUnique();

            builder.Property(payment => payment.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(payment => payment.FailureReason)
                .HasMaxLength(500);

            builder.Property(payment => payment.CreatedAtUtc)
                .IsRequired();

            builder.HasOne<Order>()
                .WithMany()
                .HasForeignKey(payment => payment.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Customer>()
                .WithMany()
                .HasForeignKey(payment => payment.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}