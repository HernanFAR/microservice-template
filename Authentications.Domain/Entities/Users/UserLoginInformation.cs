using SharedKernel.Domain.Abstracts;
using System;

namespace Authentications.Domain.Entities.Users
{
    public class UserLoginInformation : Entity<Guid>
    {
        public string Ip { get; private set; }
        public const int IpMaxLength = 32;

        public string? Continent { get; private set; }
        public const int ContinentMaxLength = 128;

        public string? Region { get; private set; }
        public const int RegionMaxLength = 128;

        public string? City { get; private set; }
        public const int CityMaxLength = 128;

        public long? Latitude { get; private set; }

        public long? Longitude { get; private set; }

        public DateTimeOffset Date { get; private set; }

        private UserLoginInformation() : base(Guid.NewGuid())
        {
            Ip = string.Empty;
            Continent = string.Empty;
            Region = string.Empty;
            City = string.Empty;
        }

        public UserLoginInformation(string ip, string? continent, string? region, string? city,
            long? latitude, long? longitude, DateTimeOffset date) : this()
        {
            Ip = ip;
            Continent = continent;
            Region = region;
            City = city;
            Latitude = latitude;
            Longitude = longitude;
            Date = date;
        }
    }
}
