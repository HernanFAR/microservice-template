using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Configuration;

namespace Authentications.Infrastructure.Abstractions.BackgroundTaskQueue
{
    public class BackgroundTaskConfiguration
    {
        public int Capacity { get; }

        public BackgroundTaskConfiguration(IConfiguration configuration)
        {
            var section = configuration.GetSection("TareasSegundoPlano");

            if (int.TryParse(section["Capacidad"], out var result))
            {
                Capacity = result;
            }
        }

        public BackgroundTaskConfiguration(int capacity)
        {
            Capacity = capacity;
        }
    }
}
