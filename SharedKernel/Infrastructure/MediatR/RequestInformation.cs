﻿using SharedKernel.Infrastructure.MediatR.Interfaces;
using System;

namespace SharedKernel.Infrastructure.MediatR
{
    public class RequestInformation<TRequestInHandling> : IRequestInformation<TRequestInHandling>
    {
        public RequestInformation()
        {
            RequestId = Guid.NewGuid();
        }

        public Guid RequestId { get; }
    }
}
