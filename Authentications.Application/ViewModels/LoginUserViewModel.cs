using Authentications.Application.Abstractions;
using System;

namespace Authentications.Application.ViewModels
{
    public record LoginUserViewModel(string Name, DoxInfo DoxInfo, DateTime LoginDateTime);
}
