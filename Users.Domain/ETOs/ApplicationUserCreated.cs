using System;
using MediatR;

namespace Users.Domain.ETOs
{
    public record ApplicationUserCreated(Guid Id) : INotification;
}
