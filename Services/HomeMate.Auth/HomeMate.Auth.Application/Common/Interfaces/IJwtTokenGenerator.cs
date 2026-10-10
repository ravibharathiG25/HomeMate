using System;
using System.Collections.Generic;
using System.Text;

namespace HomeMate.Auth.Application.Common.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid userId, string email, string firstName, string lastName, IEnumerable<string> roles);
    }
}
