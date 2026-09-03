using CommerceFlow.Application.Authentication;
using CommerceFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CommerceFlow.Infrastructure.Security
{
    public sealed class PasswordService : IPasswordService
    {
        private readonly PasswordHasher<Customer> _passwordHasher = new();

        public string HashPassword(
            Customer customer,
            string password)
        {
            return _passwordHasher.HashPassword(
                customer,
                password);
        }

        public bool VerifyPassword(
            Customer customer,
            string passwordHash,
            string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(
                customer,
                passwordHash,
                providedPassword);

            return result != PasswordVerificationResult.Failed;
        }
    }
}