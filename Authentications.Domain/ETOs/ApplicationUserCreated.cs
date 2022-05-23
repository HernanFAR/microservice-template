using MediatR;
using System;

namespace Authentications.Domain.ETOs
{
    public record ApplicationUserCreated(Guid Id) : INotification;
}
