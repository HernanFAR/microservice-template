using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SharedKernel.Domain.Others;
using SharedKernel.WebAPI.Interfaces;
using SharedKernel.WebAPI.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedKernel.WebAPI.MiddleWares
{
    public class ExceptionHandlingMiddleware : IExceptionHandlingMiddleware
    {
        private readonly PathString _ExceptionHandlingPath;
        private readonly bool _ThrowInExceptionWithoutHandler;
        private readonly bool _AllowStatusCode404Response;
        protected readonly IDictionary<Type, Func<HttpContext, Exception, Task>> ExceptionHandlers;

        public ExceptionHandlingMiddleware(
            bool allowStatusCode404Response,
            bool throwInExceptionWithoutHandler,
            IDictionary<Type, Func<HttpContext, Exception, Task>>? handlers = null,
            PathString? exceptionHandlingPath = null)
        {
            handlers ??= new Dictionary<Type, Func<HttpContext, Exception, Task>>();
            handlers[typeof(ValidationException)] = ValidationExceptionHandler;
            handlers[typeof(BusinessException)] = BusinessExceptionHandler;
            ExceptionHandlers = handlers;

            _AllowStatusCode404Response = allowStatusCode404Response;
            _ExceptionHandlingPath = exceptionHandlingPath ?? PathString.Empty;
            _ThrowInExceptionWithoutHandler = throwInExceptionWithoutHandler;

        }

        public ExceptionHandlerOptions Options => new()
        {
            AllowStatusCode404Response = _AllowStatusCode404Response,
            ExceptionHandler = HandleException,
            ExceptionHandlingPath = _ExceptionHandlingPath
        };

        protected virtual async Task HandleException(HttpContext context)
        {
            var exception = context.Features
                .Get<IExceptionHandlerPathFeature>()
                .Error;

            var hasHandler = ExceptionHandlers.Keys
                .Contains(exception.GetType());

            if (!hasHandler)
            {
                if (_ThrowInExceptionWithoutHandler)
                {
                    throw exception;
                }

                await GenericExceptionHandler(context, exception);

                return;
            }

            await ExceptionHandlers[exception.GetType()](context, exception);
        }

        protected virtual async Task GenericExceptionHandler(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response
                .WriteAsJsonAsync(
                    new List<ValidationError>
                    {
                        new("Server", "Ha ocurrido un error en el servidor interno.")
                    }
                );
        }

        protected virtual async Task ValidationExceptionHandler(HttpContext context, Exception ex)
        {
            var validationException = (ValidationException)ex;

            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;

            await context.Response.WriteAsJsonAsync(validationException.Errors
                .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage))
                .ToList());
        }

        protected virtual async Task BusinessExceptionHandler(HttpContext context, Exception ex)
        {
            var businessException = (BusinessException)ex;
            context.Response.StatusCode = (int)businessException.StatusCode;

            if (businessException.Message is not null)
            {
                await context.Response
                    .WriteAsJsonAsync(
                        new List<ValidationError>
                        {
                            new("entity", ex.Message)
                        }
                    );
            }
        }
    }
}
