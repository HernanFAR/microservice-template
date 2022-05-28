using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Entities;
using Users.Domain.Entities.Users;

namespace Users.EntityFramework.Relations
{
    internal class IdentityUserRoleEntityConfiguration : IEntityTypeConfiguration<IdentityUserRole<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
        {
            builder.HasData(new IdentityUserRole<Guid>
            {
                UserId = ApplicationUser.Admin.Id,
                RoleId = ApplicationRole.Admin.Id
            });

        }
    }
}
