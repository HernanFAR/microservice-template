using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Authentications.Application.Abstractions;

namespace Authentications.Application.ViewModels
{
    public record LoginUserViewModel(string Name, DoxInfo DoxInfo, DateTime LoginDateTime);
}
