using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Users.EntityFramework;

namespace Users.Application.Features
{
    public class Read
    {
        [DisplayName("RecordDTO")]
        public record DTO(Guid Id, string UserName, string Email, string PhoneNumber);

        [DisplayName("RecordQuery")]
        public record Query(Guid Id) : IRequest<DTO?>;

        public class Handler : IRequestHandler<Query, DTO?>
        {
            private readonly ApplicationDbContext _Context;

            public Handler(ApplicationDbContext context)
            {
                _Context = context;
            }

            public async Task<DTO?> Handle(Query request, CancellationToken cancellationToken)
            {
                var existUser = await _Context.Users
                    .AnyAsync(e => e.Id == request.Id, cancellationToken);

                if (!existUser) return null;

                var userInfo = await _Context.Users
                    .Select(e => new
                    {
                        e.Id,
                        e.UserName,
                        e.Email,
                        e.PhoneNumber
                    })
                    .SingleAsync(e => e.Id == request.Id, cancellationToken);

                return new DTO(userInfo.Id, userInfo.UserName, userInfo.Email, userInfo.PhoneNumber);
            }
        }
    }
}
