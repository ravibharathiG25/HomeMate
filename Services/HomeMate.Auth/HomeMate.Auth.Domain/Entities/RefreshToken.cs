using System;
using System.Collections.Generic;
using System.Text;

namespace HomeMate.Auth.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresOnUtc { get; set; }
        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedOnUtc { get; set; }
        public string? ReplacedByToken { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresOnUtc;
        public bool IsRevoked => RevokedOnUtc.HasValue;
        public bool IsActive => !IsRevoked && !IsExpired;

        public void Revoke(string? replacedByToken = null)
        {
            RevokedOnUtc = DateTime.UtcNow;
            ReplacedByToken = replacedByToken;
        }
    }
}
