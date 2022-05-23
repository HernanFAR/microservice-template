using SharedKernel.WebAPI.Swagger;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSecuredSwagger(this IApplicationBuilder app, string name, string url = "/swagger/v1/swagger.json")
        {
            app.UseMiddleware<SwaggerAuthenticationMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint(url, name));

            return app;
        }
    }
}
