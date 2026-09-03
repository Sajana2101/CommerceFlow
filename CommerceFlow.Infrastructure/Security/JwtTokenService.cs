using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CommerceFlow.Application.Authentication;
using CommerceFlow.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CommerceFlow.Infrastructure.Security
{
    public sealed class JwtTokenService : ITokenService
    {
        private readonly JwtOptions _options;

        public JwtTokenService(
            IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public TokenResult CreateAccessToken(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(_options.Key))
                throw new InvalidOperationException(
                    "JWT signing key has not been configured.");

            var now = DateTime.UtcNow;

            var expiresAtUtc = now.AddMinutes(
                _options.ExpirationMinutes);

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    customer.Id.ToString()),

                new Claim(
                    ClaimTypes.Email,
                    customer.Email),

                new Claim(
                    ClaimTypes.Name,
                    $"{customer.FirstName} {customer.LastName}"),

                new Claim(
                    ClaimTypes.Role,
                    customer.Role.ToString()),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            };

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_options.Key));

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            var tokenValue = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return new TokenResult(
                tokenValue,
                expiresAtUtc);
        }
    }
}