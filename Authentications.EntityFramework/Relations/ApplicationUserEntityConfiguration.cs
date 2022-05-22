using System;
using Authentications.Domain.DataAccess;
using Authentications.Domain.Entities;
using Authentications.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authentications.EntityFramework.Relations
{
    internal class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable(nameof(ApplicationUser), DatabaseConstants.Schema);

            builder.HasData(new ApplicationUser(ApplicationUser.Admin.Id, new DateTime(2022, 05, 11))
            {
                UserName = ApplicationUser.Admin.UserName,
                NormalizedUserName = ApplicationUser.Admin.NormalizedUserName,
                Email = ApplicationUser.Admin.Email,
                NormalizedEmail = ApplicationUser.Admin.NormalizedEmail,
                ConcurrencyStamp = ApplicationUser.Admin.ConcurrencyStamp,
                EmailConfirmed = ApplicationUser.Admin.EmailConfirmed,
                LockoutEnabled = ApplicationUser.Admin.LockoutEnabled,
                SecurityStamp = ApplicationUser.Admin.SecurityStamp,
                TwoFactorEnabled = ApplicationUser.Admin.TwoFactorEnabled,
                PasswordHash = ApplicationUser.Admin.PasswordHash,
                PhoneNumber = ApplicationUser.Admin.PhoneNumber,
                PhoneNumberConfirmed = ApplicationUser.Admin.PhoneNumberConfirmed
            });

            builder.OwnsMany(e => e.LoginInformations, LoginInformationEntityConfiguration);

        }

        private void LoginInformationEntityConfiguration(OwnedNavigationBuilder<ApplicationUser, UserLoginInformation> builder)
        {
            builder.ToTable(nameof(UserLoginInformation), DatabaseConstants.Schema);

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Ip)
                .HasMaxLength(UserLoginInformation.IpMaxLength)
                .IsRequired();

            builder.Property(e => e.Continent)
                .HasMaxLength(UserLoginInformation.ContinentMaxLength)
                .IsRequired(false);

            builder.Property(e => e.Region)
                .HasMaxLength(UserLoginInformation.RegionMaxLength)
                .IsRequired(false);

            builder.Property(e => e.City)
                .HasMaxLength(UserLoginInformation.CityMaxLength)
                .IsRequired(false);
            
        }
    }
}
