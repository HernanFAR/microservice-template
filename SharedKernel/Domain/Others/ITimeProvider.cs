using System;

namespace SharedKernel.Domain.Others
{
    public interface ITimeProvider
    {
        DateTimeOffset GetDateTimeOffset();

        DateTime GetDateTime();
    }
}
