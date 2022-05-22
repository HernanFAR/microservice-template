using FluentValidation;
using FluentValidation.Results;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Identity
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
