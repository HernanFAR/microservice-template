using SharedKernel.Domain.Others;
using System;

namespace SharedKernel.Infrastructure.Others
{
    public class TimeProvider : ITimeProvider
    {
        public DateTimeOffset GetDateTimeOffset() => DateTimeOffset.Now;

        public DateTime GetDateTime() => DateTime.Now;
    }
}
