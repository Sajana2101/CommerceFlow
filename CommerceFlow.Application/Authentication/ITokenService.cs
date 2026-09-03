using CommerceFlow.Domain.Entities;

namespace CommerceFlow.Application.Authentication
{
    public interface ITokenService
    {
        TokenResult CreateAccessToken(Customer customer);
    }
}