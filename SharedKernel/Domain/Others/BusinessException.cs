using System;
using System.Net;

namespace SharedKernel.Domain.Others
{
    public class BusinessException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public new string? Message { get; }

        public BusinessException(HttpStatusCode statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public static BusinessException Unauthorized(string? message = null)
        {
            return new BusinessException(HttpStatusCode.Unauthorized, message);
        }

        public static BusinessException Forbidden(string? message = null)
        {
            return new BusinessException(HttpStatusCode.Forbidden, message);
        }

        public static BusinessException NotFound(string? message = null)
        {
            return new BusinessException(HttpStatusCode.NotFound, message);
        }

        public static BusinessException UnprocessableEntity(string? message = null)
        {
            return new BusinessException(HttpStatusCode.UnprocessableEntity, message);
        }

    }
}
