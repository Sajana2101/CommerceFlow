using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Authentication
{
    public sealed record TokenResult(
        string AccessToken,
        DateTime ExpiresAtUtc);
}
