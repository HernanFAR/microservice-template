using System;

namespace Users.RazorViews.ViewModels
{
    public record WelcomeUserViewModel(string Username, string Email, string PhoneNumber, DateTime Created);
}
