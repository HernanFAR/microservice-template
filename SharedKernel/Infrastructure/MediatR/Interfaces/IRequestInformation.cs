using System;

namespace SharedKernel.Infrastructure.MediatR.Interfaces
{
    public interface IRequestInformation<TRequestInHandling>
    {
        Guid RequestId { get; }

    }
}
