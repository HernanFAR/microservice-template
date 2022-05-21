using System;

namespace SharedKernel.Infrastructure.MediatR.Interfaces
{
    public interface IRequestInformation
    {
        Guid RequestId { get; }

    }
}
