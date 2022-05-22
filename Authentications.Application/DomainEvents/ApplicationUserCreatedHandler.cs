using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Authentications.Application.Abstractions;
using Authentications.Application.Configurations;
using Authentications.Application.ViewModels;
using Authentications.Domain.Entities;
using Authentications.Domain.Entities.Users;
using Authentications.Domain.ETOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Application.Abstractions;

namespace Authentications.Application.DomainEvents
{
    public class ApplicationUserCreatedHandler : INotificationHandler<ApplicationUserCreated>
    {
        private readonly IUserStore<ApplicationUser> _UserStore;
        private readonly IEmailSender _EmailSender;
        private readonly IRazorViewRenderService _RazorViewRenderService;
        private readonly IBackgroundTaskQueue _BackgroundTaskQueue;
        private readonly EmailConfiguration _EmailConfiguration;
        private const string _Subject = "Usuario creado en Microservice.Authentications";

        public ApplicationUserCreatedHandler(IUserStore<ApplicationUser> userStore, IEmailSender emailSender,
            IRazorViewRenderService razorViewRenderService, IBackgroundTaskQueue backgroundTaskQueue,
            EmailConfiguration emailConfiguration)
        {
            _UserStore = userStore;
            _EmailSender = emailSender;
            _RazorViewRenderService = razorViewRenderService;
            _BackgroundTaskQueue = backgroundTaskQueue;
            _EmailConfiguration = emailConfiguration;
        }

        public async Task Handle(ApplicationUserCreated notification, CancellationToken __)
        {
            await _BackgroundTaskQueue.QueueBackgroundWorkItemAsync(async cancellationToken =>
            {
                var user = await _UserStore.FindByIdAsync(notification.Id.ToString(), cancellationToken);

                var viewModel = new WelcomeUserViewModel(
                    user.UserName, user.Email, user.PhoneNumber, user.Created);

                var view = await _RazorViewRenderService.RenderViewToStringAsync(
                    "Views/WelcomeUserView.cshtml", 
                    viewModel);

                var from = new EmailAddress(_EmailConfiguration.From, _EmailConfiguration.FromName);
                var to = new EmailAddress(user.Email, user.UserName);

                var email = new EmailMessage(from, to, _Subject, view);

                await _EmailSender.SendEmailAsync(email, cancellationToken);

            });
        }
    }
}
