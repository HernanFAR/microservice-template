using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Authentications.Application.Abstractions;
using Authentications.Application.ViewModels;
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
        private readonly IRazorViewRenderService _RazorViewRenderService;
        private readonly IBackgroundTaskQueue _BackgroundTaskQueue;
        private const string _Subject = "Usuario creado en Microservice.Authentications";

        public ApplicationUserCreatedHandler(IUserStore<ApplicationUser> userStore, IEmailSender emailSender,
            IRazorViewRenderService razorViewRenderService, IBackgroundTaskQueue backgroundTaskQueue)
        {
            _UserStore = userStore;
            _EmailSender = emailSender;
            _RazorViewRenderService = razorViewRenderService;
            _BackgroundTaskQueue = backgroundTaskQueue;
        }

        public async Task Handle(ApplicationUserCreated notification, CancellationToken __)
        {
            await _BackgroundTaskQueue.QueueBackgroundWorkItemAsync(async cancellationToken =>
            {
                var user = await _UserStore.FindByIdAsync(notification.Id.ToString(), cancellationToken);

                var view = await _RazorViewRenderService.RenderViewToStringAsync("Views/WelcomeUserView.cshtml",
                    new WelcomeUserViewModel(user.UserName, user.Email, user.PhoneNumber, user.Created));

                var email = new EmailMessage(
                    null!,
                    new EmailAddress(user.Email, user.UserName),
                    _Subject,
                    view);

                await _EmailSender.SendEmailAsync(email, cancellationToken);

            });
        }
    }
}
