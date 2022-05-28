using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Users.Domain.Entities.Users;

namespace Users.EntityFramework.Identity
{
    public class ApplicationSignInManager : SignInManager<ApplicationUser>
    {
        public new ApplicationUserManager UserManager { get; }

        public ApplicationSignInManager(ApplicationUserManager userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<ApplicationUser>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<ApplicationUser> confirmation) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
            UserManager = userManager;
        }

    }
}
