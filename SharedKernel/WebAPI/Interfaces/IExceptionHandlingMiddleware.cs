using Microsoft.AspNetCore.Builder;

namespace SharedKernel.WebAPI.Interfaces
{
    public interface IExceptionHandlingMiddleware
    {
        ExceptionHandlerOptions Options { get; }
    }
}
