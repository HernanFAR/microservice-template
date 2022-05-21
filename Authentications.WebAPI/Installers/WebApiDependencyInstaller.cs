using Authentications.Application.Configurations;
using Authentications.WebAPI.CORSPolicies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SharedKernel.WebAPI.Interfaces;
using SharedKernel.WebAPI.MiddleWares;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Authentications.WebAPI.Installers
{
    public class WebApiDependencyInstaller : IDependencyInstaller
    {
        public void InstallDependencies(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddControllers();
            serviceCollection.AddSwaggerGen(c =>
                {
                    c.EnableAnnotations();

                    c.CustomSchemaIds(GetSchemaId);

                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "API de Usuarios",
                        Version = "v1",
                        Description = "API encargada de la gestión de usuarios del sistema"
                    });

                });

            serviceCollection.Configure<ApiBehaviorOptions>(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;

                });

            serviceCollection.AddSingleton<IExceptionHandlingMiddleware>(services =>
            {
                var env = services.GetRequiredService<IWebHostEnvironment>();

                return new ExceptionHandlingMiddleware(
                    true,
                    env.IsDevelopment());

            });

            serviceCollection.AddCors(options => options.AddPolicy(DefaultPolicy.Name,
                policyBuilder => DefaultPolicy.Build(policyBuilder, configuration)));

        }

        private static string GetSchemaId(Type x)
        {
            var displayName = x.GetCustomAttributes(false)
                .OfType<DisplayNameAttribute>()
                .FirstOrDefault()?.DisplayName;

            if (displayName is not null) return displayName;

            var className = x.Name.Split("`")[0];
            var genericArgs = x.GetGenericArguments();

            var genericName = genericArgs.Any()
                ? string.Concat(
                    "<", string.Join(", ", genericArgs.Select(GetSchemaId)), ">"
                )
                : string.Empty;

            return string.Concat(className, genericName);

        }
    }
}
