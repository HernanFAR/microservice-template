using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Authentications.Application.Abstractions;
using Authentications.Domain.Entities;
using Authentications.Domain.ETOs;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Authentications.Application.DomainEvents
{
    public class ApplicationUserCreatedHandler : INotificationHandler<ApplicationUserCreated>
    {
        private readonly IUserStore<ApplicationUser> _UserStore;
        private readonly IEmailSender _EmailSender;

        public ApplicationUserCreatedHandler(IUserStore<ApplicationUser> userStore, IEmailSender emailSender)
        {
            _UserStore = userStore;
            _EmailSender = emailSender;
        }

        public async Task Handle(ApplicationUserCreated notification, CancellationToken cancellationToken)
        {
            var user = await _UserStore.FindByIdAsync(notification.Id.ToString(), cancellationToken);

            await _EmailSender.SendEmailAsync(null);
        }
    }
}
