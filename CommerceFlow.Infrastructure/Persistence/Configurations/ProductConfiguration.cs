using CommerceFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceFlow.Infrastructure.Persistence.Configurations
{
    public sealed class ProductConfiguration
        : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(product => product.Id);

            builder.Property(product => product.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(product => product.Description)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(product => product.Sku)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(product => product.Price)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(product => product.IsActive)
                .IsRequired();

            builder.Property(product => product.CreatedAtUtc)
                .IsRequired();

            builder.HasIndex(product => product.Sku)
                .IsUnique();
        }
    }
}