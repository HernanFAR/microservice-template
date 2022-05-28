using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Razor.Templating.Core;
using SharedKernel.Application.Abstractions;
using Users.Application.Configurations;
using Users.Domain.ETOs;
using Users.EntityFramework;
using Users.RazorViews.ViewModels;

namespace Users.Application.DomainEvents
{
    public class ApplicationUserCreatedHandler : INotificationHandler<ApplicationUserCreated>
    {
        private readonly ApplicationDbContext _Context;
        private readonly IEmailSender _EmailSender;
        private readonly EmailConfiguration _EmailConfiguration;
        private const string _Subject = "Usuario creado en Microservice.Authentications";

        public ApplicationUserCreatedHandler(ApplicationDbContext context, IEmailSender emailSender, 
            EmailConfiguration emailConfiguration)
        {
            _Context = context;
            _EmailSender = emailSender;
            _EmailConfiguration = emailConfiguration;
        }

        public async Task Handle(ApplicationUserCreated notification, CancellationToken cancellationToken)
        {
            var user = await _Context.Users
                .Select(e => new
                {
                    e.Id,
                    e.UserName,
                    e.Email,
                    e.PhoneNumber,
                    e.Created
                })
                .SingleAsync(e => e.Id == notification.Id, cancellationToken);

            var viewModel = new WelcomeUserViewModel(
                user.UserName, user.Email, user.PhoneNumber, user.Created);

            var view = await RazorTemplateEngine.RenderAsync(
                "~/Views/WelcomeUserView.cshtml", viewModel);

            var from = new EmailAddress(_EmailConfiguration.From, _EmailConfiguration.FromName);
            var to = new EmailAddress(user.Email, user.UserName);
            var bccs = new List<EmailAddress>();

            if (_EmailConfiguration.BCCs is not null)
            {
                bccs = _EmailConfiguration.BCCs
                    .Select(e => new EmailAddress(e, e))
                    .ToList();
            }

            var email = new EmailMessage(from, to, _Subject, view, Array.Empty<EmailAddress>(), bccs);

            await _EmailSender.SendEmailAsync(email, cancellationToken);

        }
    }
}
