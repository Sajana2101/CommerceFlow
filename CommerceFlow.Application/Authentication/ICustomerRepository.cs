using System;
using System.Collections.Generic;
using System.Text;
using CommerceFlow.Domain.Entities;

namespace CommerceFlow.Application.Authentication
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default);

        Task<Customer?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<bool> EmailExistsAsync(
            string email,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Customer customer,
            CancellationToken cancellationToken = default);
    }
}