using System;
using System.Collections.Generic;
using System.Text;

namespace HomeMate.Auth.Domain.Constants
{
    public static class Roles
    {
        public const string Customer = "Customer";
        public const string Provider = "Provider";
        public const string Admin = "Admin";

        public static readonly IReadOnlyList<string> All = [Customer, Provider, Admin];
    }
}
