using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Answers.Application.Configurations;
using Answers.Application.InternalServices;
using Answers.Domain.ETOs;
using Answers.EntityFramework;
using Answers.RazorViews.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Razor.Templating.Core;
using SharedKernel.Application.Abstractions;

namespace Answers.Application.DomainEvents
{
    public class AnswerCreatedHandler : INotificationHandler<AnswerCreated>
    {
        private readonly ApplicationDbContext _Context;
        private readonly IEmailSender _EmailSender;
        private readonly EmailConfiguration _EmailConfiguration;
        private readonly IQuestionService _QuestionService;
        private readonly IUserService _UserService;
        private readonly ILogger<AnswerCreatedHandler> _Logger;
        private const string _Subject = "Se creo una respuesta a tu pregunta Microservice.Authentications";

        public AnswerCreatedHandler(ApplicationDbContext context, IEmailSender emailSender,
            EmailConfiguration emailConfiguration, IQuestionService questionService,
            IUserService userService, ILogger<AnswerCreatedHandler> logger)
        {
            _Context = context;
            _EmailSender = emailSender;
            _EmailConfiguration = emailConfiguration;
            _QuestionService = questionService;
            _UserService = userService;
            _Logger = logger;
        }

        public async Task Handle(AnswerCreated notification, CancellationToken cancellationToken)
        {
            var answer = await _Context.Answers
                .Where(e => e.Id == notification.AnswerId)
                .Select(e => new {e.Name, e.QuestionId, e.CreatedById, e.Created})
                .SingleOrDefaultAsync(cancellationToken);

            if (answer is null)
            {
                _Logger.LogError("Al momento de manejar {0}, la respuesta de ID {1} se detecto que no existe.", 
                    nameof(AnswerCreated), notification.AnswerId);

                return;
            }

            var questionTask = _QuestionService.ReadOneAsync(answer.QuestionId, cancellationToken);
            var answerCreatorTask = _UserService.ReadAsync(answer.CreatedById, cancellationToken);

            await Task.WhenAll(questionTask, answerCreatorTask);

            var question = await questionTask;

            if (question is null)
            {
                _Logger.LogError("Al momento de manejar {0}, la pregunta de ID {1} se detecto que no existe",
                    nameof(AnswerCreated), answer.QuestionId);
            }

            var answerCreator = await answerCreatorTask;

            if (answerCreator is null)
            {
                _Logger.LogError("Al momento de manejar {0}, la creador de ID {1}, de la respuesta de ID {2}, se detecto que no existe",
                    nameof(AnswerCreated), answer.CreatedById, notification.AnswerId);
            }

            if (question is null || answerCreator is null)
            {
                return;
            }

            var questionCreator = await _UserService.ReadAsync(question.CreatedById, cancellationToken);

            if (questionCreator is null)
            {
                _Logger.LogError("Al momento de manejar {0}, el creador de ID {1}, de la pregunta de ID {2}, se detecto que no existe",
                    nameof(AnswerCreated), question.CreatedById, question.Id);

                return;
            }

            var viewModel = new AnswerNotificationViewModel(answer.Name, answer.Created, answerCreator.UserName, 
                question.Id, question.Name, question.Created, questionCreator.UserName);

            var view = await RazorTemplateEngine.RenderAsync(
                "~/Views/AnswerNotificationView.cshtml", viewModel);

            var from = new EmailAddress(_EmailConfiguration.From, _EmailConfiguration.FromName);
            var to = new EmailAddress(questionCreator.Email, questionCreator.UserName);
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
