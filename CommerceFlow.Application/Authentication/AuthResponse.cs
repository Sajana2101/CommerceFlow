using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Authentication
{
    public sealed record AuthResponse(
        string AccessToken,
        DateTime ExpiresAtUtc,
        CustomerResponse Customer);
}