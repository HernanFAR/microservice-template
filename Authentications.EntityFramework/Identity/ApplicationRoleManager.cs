using Authentications.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Authentications.EntityFramework.Identity
{
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole> store, IEnumerable<IRoleValidator<ApplicationRole>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<ApplicationRole>> logger) : base(store, roleValidators, keyNormalizer, errors, logger) { }
    }
}
