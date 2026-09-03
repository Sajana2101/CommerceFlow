using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceFlow.Application.Authentication
{
    public sealed record LoginRequest(
        string Email,
        string Password);
}