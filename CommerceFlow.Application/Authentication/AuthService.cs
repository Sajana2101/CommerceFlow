using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CommerceFlow.Domain.Entities;

namespace CommerceFlow.Application.Authentication
{
    public sealed class AuthService : IAuthService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        public AuthService(
            ICustomerRepository customerRepository,
            IPasswordService passwordService,
            ITokenService tokenService)
        {
            _customerRepository = customerRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> RegisterAsync(
            RegisterRequest request,
            CancellationToken cancellationToken = default)
        {
            ValidateRegistration(request);

            var emailExists = await _customerRepository.EmailExistsAsync(
                request.Email,
                cancellationToken);

            if (emailExists)
                throw new InvalidOperationException(
                    "An account with this email already exists.");

            var customer = new Customer(
                request.FirstName,
                request.LastName,
                request.Email);

            var passwordHash = _passwordService.HashPassword(
                customer,
                request.Password);

            customer.SetPasswordHash(passwordHash);

            await _customerRepository.AddAsync(
                customer,
                cancellationToken);

            return CreateAuthResponse(customer);
        }

        public async Task<AuthResponse> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException(
                    "Email and password are required.");
            }

            var customer = await _customerRepository.GetByEmailAsync(
                request.Email,
                cancellationToken);

            if (customer is null ||
                !customer.IsActive)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            var passwordValid = _passwordService.VerifyPassword(
                customer,
                customer.PasswordHash,
                request.Password);

            if (!passwordValid)
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");

            return CreateAuthResponse(customer);
        }

        public async Task<CustomerResponse?> GetCustomerAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var customer = await _customerRepository.GetByIdAsync(
                customerId,
                cancellationToken);

            return customer is null
                ? null
                : ToResponse(customer);
        }

        private AuthResponse CreateAuthResponse(Customer customer)
        {
            var token = _tokenService.CreateAccessToken(customer);

            return new AuthResponse(
                token.AccessToken,
                token.ExpiresAtUtc,
                ToResponse(customer));
        }

        private static CustomerResponse ToResponse(Customer customer)
        {
            return new CustomerResponse(
                customer.Id,
                customer.FirstName,
                customer.LastName,
                customer.Email,
                customer.Role.ToString(),
                customer.CreatedAtUtc);
        }

        private static void ValidateRegistration(
            RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName))
                throw new ArgumentException(
                    "First name is required.");

            if (string.IsNullOrWhiteSpace(request.LastName))
                throw new ArgumentException(
                    "Last name is required.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException(
                    "Email is required.");

            if (!new EmailAddressAttribute().IsValid(request.Email))
                throw new ArgumentException(
                    "A valid email address is required.");

            ValidatePassword(request.Password);
        }

        private static void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException(
                    "Password is required.");

            if (password.Length < 10)
                throw new ArgumentException(
                    "Password must contain at least 10 characters.");

            if (!password.Any(char.IsUpper))
                throw new ArgumentException(
                    "Password must contain an uppercase letter.");

            if (!password.Any(char.IsLower))
                throw new ArgumentException(
                    "Password must contain a lowercase letter.");

            if (!password.Any(char.IsDigit))
                throw new ArgumentException(
                    "Password must contain a number.");

            if (!password.Any(character =>
                !char.IsLetterOrDigit(character)))
            {
                throw new ArgumentException(
                    "Password must contain a special character.");
            }
        }
    }
}
