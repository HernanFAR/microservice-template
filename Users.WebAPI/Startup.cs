using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedKernel.WebAPI.Interfaces;
using Users.WebAPI.CORSPolicies;

namespace Users.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.InstallApplicationDependenciesFrom<Startup>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IExceptionHandlingMiddleware exceptionHandlingMiddleware)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSecuredSwagger("Web API Usuarios v1");

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(DefaultPolicy.Name);
            app.UseAuthorization();

            app.UseExceptionHandler(exceptionHandlingMiddleware.Options);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
