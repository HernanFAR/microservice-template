using System;
using MediatR;

namespace Users.Domain.ETOs
{
    public record ApplicationUserLogged(Guid LoginInformationId) : INotification;
}
