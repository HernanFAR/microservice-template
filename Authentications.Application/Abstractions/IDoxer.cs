using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Authentications.Application.Abstractions
{
    public record DoxInfo(string Ip, string Continent, string Region, string City, long Latitude, long Longitude);

    public interface IDoxer
    {
        Task<DoxInfo?> DoxIpAsync(string ip, CancellationToken cancellationToken = default);

    }
}
