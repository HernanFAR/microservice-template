using System;
using System.Collections.Generic;
using System.Linq;
using Authentications.Application.Configurations;
using Authentications.Domain.Entities.Users;
using Authentications.Domain.ETOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Application.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Authentications.EntityFramework;
using Authentications.RazorViews.ViewModels;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;

namespace Authentications.Application.DomainEvents
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

            var view = await Razor.Templating.Core.RazorTemplateEngine.RenderAsync(
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
