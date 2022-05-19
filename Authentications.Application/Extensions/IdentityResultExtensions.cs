using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace Authentications.Application.Extensions
{
    internal static class IdentityResultExtensions
    {
        public static void ThrowValidationExceptionIfNotValid(this IdentityResult @this)
        {
            if (@this.Succeeded) return;
            
            var errors = @this.Errors
                .Select(e => new ValidationFailure(e.Code, e.Description))
                .ToList();

            throw new ValidationException(errors);
        }
    }
}
