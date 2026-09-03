using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Authentication
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(
            RegisterRequest request,
            CancellationToken cancellationToken = default);

        Task<AuthResponse> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken = default);

        Task<CustomerResponse?> GetCustomerAsync(
            Guid customerId,
            CancellationToken cancellationToken = default);
    }
}