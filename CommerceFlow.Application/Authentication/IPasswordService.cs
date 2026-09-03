using System;
using System.Collections.Generic;
using System.Text;
using CommerceFlow.Domain.Entities;

namespace CommerceFlow.Application.Authentication
{
    public interface IPasswordService
    {
        string HashPassword(
            Customer customer,
            string password);

        bool VerifyPassword(
            Customer customer,
            string passwordHash,
            string providedPassword);
    }
}