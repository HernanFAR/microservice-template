using System;
using Microsoft.AspNetCore.Identity;

namespace Users.Domain.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public static class Admin
        {
            public static readonly Guid Id = new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2);
            public const string Name = "Admin";
            public const string NormalizedName = "ADMIN";
        }
    }
}
