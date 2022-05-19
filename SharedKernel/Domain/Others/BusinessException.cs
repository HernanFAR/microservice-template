using System;
using System.Net;

namespace SharedKernel.Domain.Others
{
    public class BusinessException : Exception
    {
        public HttpStatusCode? StatusCode { get; }

        public BusinessException(string message, HttpStatusCode statusCode) : this(message)
        {
            StatusCode = statusCode;
        }


        public BusinessException(string message) : base(message) { }

        public static BusinessException ForbiddenWithMessage(string message)
        {
            return new BusinessException(message, HttpStatusCode.Forbidden);
        }

        public static BusinessException NotFoundWithMessage(string message)
        {
            return new BusinessException(message, HttpStatusCode.NotFound);
        }

        public static BusinessException NotAcceptableWithMessage(string message)
        {
            return new BusinessException(message, HttpStatusCode.NotAcceptable);
        }

        public static BusinessException UnProcessableEntityWithMessage(string message)
        {
            return new BusinessException(message, HttpStatusCode.UnprocessableEntity);
        }

    }
}
