using CommerceFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceFlow.Infrastructure.Persistence.Configurations
{
    public sealed class CustomerConfiguration
        : IEntityTypeConfiguration<Customer>
    {
        public void Configure(
            EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(customer => customer.Id);

            builder.Property(customer => customer.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(customer => customer.LastName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(customer => customer.Email)
                .HasMaxLength(320)
                .IsRequired();

            builder.Property(customer => customer.PasswordHash)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(customer => customer.Role)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(customer => customer.IsActive)
                .IsRequired();

            builder.Property(customer => customer.CreatedAtUtc)
                .IsRequired();

            builder.HasIndex(customer => customer.Email)
                .IsUnique();
        }
    }
}