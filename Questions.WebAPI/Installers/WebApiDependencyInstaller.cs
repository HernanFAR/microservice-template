using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Questions.WebAPI.CORSPolicies;
using SharedKernel.WebAPI.Interfaces;
using SharedKernel.WebAPI.MiddleWares;
using System;
using System.ComponentModel;
using System.Linq;

namespace Questions.WebAPI.Installers
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
                    Title = "API de Preguntas",
                    Version = "v1",
                    Description = "API encargada de la gestión de preguntas"
                });
            });

            serviceCollection.Configure<ApiBehaviorOptions>(
                options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });

            serviceCollection.AddCors(options => options.AddPolicy(
                DefaultPolicy.Name,
                policyBuilder => DefaultPolicy.Build(policyBuilder, configuration)));

            serviceCollection.AddSingleton<IExceptionHandlingMiddleware>(services =>
            {
                var hostEnvironment = services.GetRequiredService<IWebHostEnvironment>();

                return new ExceptionHandlingMiddleware(
                    true,
                    hostEnvironment.IsDevelopment());
            });

        }

        private string GetSchemaId(Type x)
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
