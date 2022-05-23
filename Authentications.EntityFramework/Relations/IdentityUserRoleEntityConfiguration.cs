﻿using Authentications.Domain.Entities;
using Authentications.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Authentications.EntityFramework.Relations
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
