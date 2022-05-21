using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentications.Application.ViewModels
{
    public record WelcomeUserViewModel(string Username, string Email, string PhoneNumber, DateTime Created);
}
