using CommerceFlow.Application.Authentication;
using CommerceFlow.Domain.Entities;
using CommerceFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CommerceFlow.Infrastructure.Customers
{
    public sealed class CustomerRepository : ICustomerRepository
    {
        private readonly CommerceFlowDbContext _dbContext;

        public CustomerRepository(
            CommerceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Customer?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            var normalizedEmail = email
                .Trim()
                .ToLowerInvariant();

            return await _dbContext.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    customer => customer.Email == normalizedEmail,
                    cancellationToken);
        }

        public async Task<Customer?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    customer =>
                        customer.Id == id &&
                        customer.IsActive,
                    cancellationToken);
        }

        public async Task<bool> EmailExistsAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            var normalizedEmail = email
                .Trim()
                .ToLowerInvariant();

            return await _dbContext.Customers
                .AnyAsync(
                    customer => customer.Email == normalizedEmail,
                    cancellationToken);
        }

        public async Task AddAsync(
            Customer customer,
            CancellationToken cancellationToken = default)
        {
            await _dbContext.Customers.AddAsync(
                customer,
                cancellationToken);

            await _dbContext.SaveChangesAsync(
                cancellationToken);
        }
    }
}