namespace CommerceFlow.Application.Authentication
{
    public sealed record RegisterRequest(
        string FirstName,
        string LastName,
        string Email,
        string Password);
}