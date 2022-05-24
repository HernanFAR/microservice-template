using System;
using System.ComponentModel;
using System.Linq;
using Answers.WebAPI.CORSPolicies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SharedKernel.WebAPI.Configurations;
using SharedKernel.WebAPI.Interfaces;
using SharedKernel.WebAPI.MiddleWares;
using SharedKernel.WebAPI.Swagger;

namespace Answers.WebAPI.Installers
{
    public class WebApiDependencyInstaller : IDependencyInstaller
    {
        public void InstallDependencies(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddControllers();
            serviceCollection.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API de Preguntas",
                    Version = "v1",
                    Description = "API encargada de la gestión de preguntas",
                    Contact = new OpenApiContact
                    {
                        Name = "Hernán Álvarez",
                        Email = "h.f.alvarez.r@gmail.com",
                        Url = new Uri("https://www.linkedin.com/in/hernan-a-rubio/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT",
                    }
                });

                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Pon **_solamente_** tu Token JWT Bearer en el input inferior",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });

                setup.EnableAnnotations();
                setup.CustomSchemaIds(GetSchemaId);
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

            serviceCollection.AddTransient<SwaggerAuthenticationMiddleware>();
            serviceCollection.AddSingleton(provider => new SwaggerAuthenticationConfiguration(provider.GetRequiredService<IConfiguration>()));

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
