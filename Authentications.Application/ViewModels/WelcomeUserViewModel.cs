using System;

namespace Authentications.Application.ViewModels
{
    public record WelcomeUserViewModel(string Username, string Email, string PhoneNumber, DateTime Created);
}
