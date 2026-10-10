using System;
using System.Collections.Generic;
using System.Text;

namespace HomeMate.Application.Features.Authentication.DTOs
{
    public class AuthResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
    }
}
