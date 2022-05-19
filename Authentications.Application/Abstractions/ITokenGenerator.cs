using Authentications.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Authentications.Application.Abstractions
{
    public interface ITokenGenerator
    {
        Task<(string Token, DateTime ExpireDate)> GetIdentityTokenAsync(ApplicationUser user,
            CancellationToken cancellationToken = default);
    }
}
