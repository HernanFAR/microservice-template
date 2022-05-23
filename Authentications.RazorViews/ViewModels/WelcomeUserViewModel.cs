using System;

namespace Authentications.RazorViews.ViewModels
{
    public record WelcomeUserViewModel(string Username, string Email, string PhoneNumber, DateTime Created);
}
