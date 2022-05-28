using System;
using System.Threading;
using System.Threading.Tasks;
using Users.Domain.Entities.Users;

namespace Users.Application.Abstractions
{
    public interface ITokenGenerator
    {
        Task<(string Token, DateTime ExpireDate)> GetIdentityTokenAsync(ApplicationUser user,
            CancellationToken cancellationToken = default);
    }
}
