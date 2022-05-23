using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Http
{
    public static class HttpContextExtensions
    {
        public static Guid GetIdentityId(this HttpContext @this)
        {
            var userIdClaim = @this.User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            var guid = Guid.Empty;

            if (userId is not null)
            {
                guid = new Guid(userId);
            }

            return guid;
        }

        public static bool HasRole(this HttpContext @this, string roleName)
        {
            return @this.User
                .FindAll(ClaimTypes.Role)
                .Any(e => e.Value == roleName);
        }
    }
}
