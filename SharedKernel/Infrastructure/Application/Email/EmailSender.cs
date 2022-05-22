using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using SharedKernel.Application.Abstractions;
using SharedKernel.Infrastructure.MediatR.Interfaces;
using EmailAddress = SendGrid.Helpers.Mail.EmailAddress;

namespace SharedKernel.Infrastructure.Application.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly SendGridConfiguration _Configuration;
        private readonly ILogger<EmailSender> _Logger;
        private readonly IRequestInformation _RequestInformation;

        public EmailSender(SendGridConfiguration configuration, ILogger<EmailSender> logger, IRequestInformation requestInformation)
        {
            _Configuration = configuration;
            _Logger = logger;
            _RequestInformation = requestInformation;
        }

        public async Task SendEmailAsync(EmailMessage message, CancellationToken cancellationToken)
        {
            var from = new EmailAddress(message.From.Address, message.From.Name);
            var tos = message.Tos
                .Select(e => new EmailAddress(e.Address, e.Name))
                .ToList();

            var sendGridEmail = MailHelper.CreateSingleEmailToMultipleRecipients(
                from, 
                tos, 
                message.Subject, 
                message.Body,
                message.Body);

            var bccs = message.BCCs
                .Select(e => new EmailAddress(e.Address, e.Name))
                .ToList();
            
            sendGridEmail.AddBccs(bccs);

            var ccs = message.CCs
                .Select(e => new EmailAddress(e.Address, e.Name))
                .ToList();

            sendGridEmail.AddCcs(ccs);

            foreach (var (name, _, content) in message.Attachments)
            {
                await sendGridEmail.AddAttachmentAsync(name, content, cancellationToken: cancellationToken);
            }

            var client = new SendGridClient(_Configuration.APIKey);

            var response = await client.SendEmailAsync(sendGridEmail, cancellationToken);

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                var contentString = await response.Body.ReadAsStringAsync(cancellationToken);

                _Logger.LogError("Identificador: {0} - No se ha podido enviar la solicitud de envío de correo a SendGrid. Información del correo: {1}, " +
                                 "información de la respuesta de SendGrid {2}.",
                    _RequestInformation.RequestId, JsonConvert.SerializeObject(sendGridEmail), contentString);
            }
        }
    }
}
