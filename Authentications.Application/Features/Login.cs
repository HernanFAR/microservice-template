using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentications.Application.Features
{
    public class Login
    {
        public record DTO(string Token, DateTimeOffset ExpireDate);
    }
}
