using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace BFF.CORSPolicies
{
    public class DefaultPolicy
    {
        public const string Name = "DefaultPolicy";

        public static readonly Action<CorsPolicyBuilder, IConfiguration> Build = (builder, configuration) =>
        {
            builder.WithOrigins(configuration.GetSection("Cors").Get<List<string>>().ToArray())
                .AllowAnyHeader()
                .AllowAnyMethod();
        };
    }
}
