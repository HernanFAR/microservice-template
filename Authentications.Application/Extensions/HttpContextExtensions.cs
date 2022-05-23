using System;
using System.Security.Claims;

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
    }
}
