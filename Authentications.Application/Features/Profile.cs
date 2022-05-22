using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Authentications.EntityFramework;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain.Others;

namespace Authentications.Application.Features
{
    public class Profile
    {
        public record LoginInformationDTO(string? Ip, string? Continent, string? Region, string? City, 
            long? Latitude, long? Longitude, DateTimeOffset Created);

        public record DTO(Guid Id, string UserName, string Email, string PhoneNumber, IReadOnlyList<LoginInformationDTO> Logins);

        public record Query() : IRequest<DTO>;

        public class Handler : IRequestHandler<Query, DTO>
        {
            private readonly ApplicationDbContext _Context;
            private readonly HttpContext _HttpContext;

            public Handler(ApplicationDbContext context, IHttpContextAccessor contextAccessor)
            {
                _Context = context;
                _HttpContext = contextAccessor.HttpContext!;
            }

            public async Task<DTO> Handle(Query request, CancellationToken cancellationToken)
            {
                var userId = _HttpContext.GetIdentityId();

                var existUser = await _Context.Users
                    .AnyAsync(e => e.Id == userId, cancellationToken);

                if (!existUser) throw BusinessException.NotFoundWithMessage($"El identificador {userId} no tiene una cuenta asociada");

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
