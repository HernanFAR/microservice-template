using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using SharedKernel.WebAPI.Configurations;
using SharedKernel.WebAPI.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SharedKernel.WebAPI.Swagger
{
    public class ProcessAuthorizationOperationFilter : IOperationFilter
    {
        private readonly UseAPIKeyConfiguration _Configuration;

        public ProcessAuthorizationOperationFilter(UseAPIKeyConfiguration configuration)
        {
            _Configuration = configuration;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var usesApiKey = UseAPIKey(context.MethodInfo.DeclaringType) || UseAPIKey(context.MethodInfo);

            if (usesApiKey)
            {
                AddUnauthorized(operation);

                return;
            }

            var isAuthorized = IsAuthorized(context.MethodInfo.DeclaringType) || IsAuthorized(context.MethodInfo);
            var allowAnonymous = AllowAnonymous(context.MethodInfo.DeclaringType) || AllowAnonymous(context.MethodInfo);

            if (!isAuthorized  || allowAnonymous) return;

            AddUnauthorized(operation);
        }

        private void AddUnauthorized(OpenApiOperation operation)
        {
            if (!operation.Responses.ContainsKey("401"))
            {
                operation.Responses.Add("401", new OpenApiResponse
                {
                    Description = $"Se ha omitido el token JWT o no se agrego el {_Configuration.APIHeader} header"
                });
            }
            
        }
        
        private static bool UseAPIKey(ICustomAttributeProvider? attributeProvider) => 
            HasAttribute<UseAPIKeyAttribute>(attributeProvider);
        
        private static bool IsAuthorized(ICustomAttributeProvider? attributeProvider) => 
            HasAttribute<AuthorizeAttribute>(attributeProvider);
        
        private static bool AllowAnonymous(ICustomAttributeProvider? attributeProvider) => 
            HasAttribute<AllowAnonymousAttribute>(attributeProvider);
        
        private static bool HasAttribute<T>(ICustomAttributeProvider? attributeProvider) => 
            attributeProvider is not null && attributeProvider.GetCustomAttributes(true).OfType<T>().Any();
    }
}
