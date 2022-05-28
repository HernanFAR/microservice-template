using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Users.EntityFramework;

namespace Users.Application.Features
{
    public class Profile
    {
        [DisplayName("ProfileLoginInformationDTO")]
        public record LoginInformationDTO(string? Ip, string? Continent, string? Region, string? City,
            long? Latitude, long? Longitude, DateTimeOffset Created);

        [DisplayName("ProfileDTO")]
        public record DTO(Guid Id, string UserName, string Email, string PhoneNumber, IReadOnlyList<LoginInformationDTO> Logins);

        [DisplayName("ProfileQuery")]
        public record Query() : IRequest<DTO?>;

        public class Handler : IRequestHandler<Query, DTO?>
        {
            private readonly ApplicationDbContext _Context;
            private readonly HttpContext _HttpContext;

            public Handler(ApplicationDbContext context, IHttpContextAccessor contextAccessor)
            {
                _Context = context;
                _HttpContext = contextAccessor.HttpContext!;
            }

            public async Task<DTO?> Handle(Query request, CancellationToken cancellationToken)
            {
                var userId = _HttpContext.GetIdentityId();

                var existUser = await _Context.Users
                    .AnyAsync(e => e.Id == userId, cancellationToken);

                if (!existUser) return null;

                var userInfo = await _Context.Users
                    .Where(e => e.Id == userId)
                    .Select(e => new
                    {
                        e.Id,
                        e.UserName,
                        e.Email,
                        e.PhoneNumber,
                        Logins = e.LoginInformations.Select(e => new
                        {
                            e.Ip,
                            e.Continent,
                            e.Region,
                            e.City,
                            e.Longitude,
                            e.Latitude,
                            e.Date
                        })
                    })
                    .SingleAsync(cancellationToken);

                return new DTO(userInfo.Id, userInfo.UserName, userInfo.Email, userInfo.PhoneNumber,
                    userInfo.Logins
                        .Select(e => new LoginInformationDTO(e.Ip,
                            e.Continent, e.Region, e.City, e.Latitude, e.Longitude, e.Date))
                        .ToList());
            }
        }
    }
}
