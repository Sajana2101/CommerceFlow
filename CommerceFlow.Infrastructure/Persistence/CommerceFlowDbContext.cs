using CommerceFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommerceFlow.Infrastructure.Persistence
{
    public sealed class CommerceFlowDbContext : DbContext
    {
        public CommerceFlowDbContext(
            DbContextOptions<CommerceFlowDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();

        public DbSet<Customer> Customers => Set<Customer>();

        public DbSet<Order> Orders => Set<Order>();

        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        public DbSet<InventoryItem> InventoryItems =>
                   Set<InventoryItem>();

        public DbSet<Payment> Payments =>
                     Set<Payment>();

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(CommerceFlowDbContext).Assembly);
        }
    }
}