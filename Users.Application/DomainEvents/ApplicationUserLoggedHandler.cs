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
    public class ApplicationUserLoggedHandler : INotificationHandler<ApplicationUserLogged>
    {
        private readonly ApplicationDbContext _Context;
        private readonly IEmailSender _EmailSender;
        private readonly EmailConfiguration _EmailConfiguration;
        private const string _Subject = "Se ha detectado un inicio de sesión en Microservice.Authentications";

        public ApplicationUserLoggedHandler(ApplicationDbContext context, IEmailSender emailSender,
            EmailConfiguration emailConfiguration)
        {
            _Context = context;
            _EmailSender = emailSender;
            _EmailConfiguration = emailConfiguration;
        }

        public async Task Handle(ApplicationUserLogged notification, CancellationToken cancellationToken)
        {
            var user = await _Context.Users
                .Select(e => new
                {
                    e.UserName,
                    e.Email,
                    LoginInformation = e.LoginInformations.First(li => li.Id == notification.LoginInformationId)
                })
                .AsNoTracking()
                .FirstAsync(li => li.LoginInformation.Id == notification.LoginInformationId, cancellationToken);

            var viewModel = new LoginUserViewModel(user.UserName, user.LoginInformation.Ip, user.LoginInformation.Continent, 
                user.LoginInformation.Region, user.LoginInformation.City, user.LoginInformation.Latitude, user.LoginInformation.Longitude, 
                user.LoginInformation.Date);

            var view = await RazorTemplateEngine.RenderAsync(
                "~/Views/LoginUserView.cshtml", viewModel);

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
