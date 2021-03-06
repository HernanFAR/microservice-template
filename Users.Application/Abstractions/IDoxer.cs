using System.Threading;
using System.Threading.Tasks;

namespace Users.Application.Abstractions
{
    public record DoxInfo(string Ip, string Continent, string Region, string City, long Latitude, long Longitude);

    public interface IDoxer
    {
        Task<DoxInfo?> DoxIpAsync(string ip, CancellationToken cancellationToken = default);

    }
}
