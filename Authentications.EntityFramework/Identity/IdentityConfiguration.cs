using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Authentications.EntityFramework.Identity
{
    public static class IdentityConfiguration
    {
        public static readonly Action<IdentityOptions> ConfigureIdentity = options =>
        {
            options.ClaimsIdentity.EmailClaimType = ClaimTypes.Email;
            options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
            options.ClaimsIdentity.SecurityStampClaimType = "AspNet.Identity.SecurityStamp";
            options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
            options.ClaimsIdentity.UserNameClaimType = ClaimTypes.Name;

            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;

            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;

            options.Stores.MaxLengthForKeys = 0;
            options.Stores.ProtectPersonalData = false;

            options.Tokens.AuthenticatorIssuer = "Microsoft.AspNetCore.Identity.UI";
            options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
            options.Tokens.ChangeEmailTokenProvider = TokenOptions.DefaultProvider;
            options.Tokens.ChangePhoneNumberTokenProvider = TokenOptions.DefaultProvider;
            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultProvider;
            options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
            options.Tokens.ProviderMap = new Dictionary<string, TokenProviderDescriptor>();

            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = false;

        };
    }
}
