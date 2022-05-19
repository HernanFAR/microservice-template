using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Authentications.Application.Abstractions;

namespace Authentications.Infrastructure.Abstractions
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(EmailMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
