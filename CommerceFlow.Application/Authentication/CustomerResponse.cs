using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Authentication
{
    public sealed record CustomerResponse(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string Role,
        DateTime CreatedAtUtc);
}