using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CommerceFlow.Infrastructure.Persistence
{
    public sealed class CommerceFlowDbContextFactory
        : IDesignTimeDbContextFactory<CommerceFlowDbContext>
    {
        public CommerceFlowDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder =
                new DbContextOptionsBuilder<CommerceFlowDbContext>();

            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\MSSQLLocalDB;" +
                "Database=CommerceFlowDb;" +
                "Trusted_Connection=True;" +
                "TrustServerCertificate=True");

            return new CommerceFlowDbContext(
                optionsBuilder.Options);
        }
    }
}
