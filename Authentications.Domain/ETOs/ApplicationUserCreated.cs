using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Authentications.Domain.ETOs
{
    public record ApplicationUserCreated(Guid Id) : INotification;
}
