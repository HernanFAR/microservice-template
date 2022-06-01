using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BFF.Clients;
using BFF.Configurations;
using BFF.CORSPolicies;
using BFF.Handlers;
using Microsoft.AspNetCore.Http;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

namespace BFF
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
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            services.AddCors(options => options.AddPolicy(
                DefaultPolicy.Name,
                policyBuilder => DefaultPolicy.Build(policyBuilder, Configuration)));

            services.AddSingleton<AnswerServiceConfiguration>();
            services.AddSingleton<QuestionServiceConfiguration>();

            services.AddSingleton<AnswerHttpClient>();
            services.AddSingleton<QuestionHttpClient>();

            services.AddOcelot()
                .AddDelegatingHandler<GetQuestionWithAnswersHandler>()
                .AddDelegatingHandler<GetAnswersOfQuestionHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(DefaultPolicy.Name);
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync($"BFF - Microservice.Autorization - Modo: {env.EnvironmentName}");
                });
            });

            await app.UseOcelot();
        }
    }
}
