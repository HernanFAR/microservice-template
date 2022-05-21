using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Authentications.Application.Abstractions
{
    public record EmailAddress(string Address, string Name);

    public record EmailAttachment(string Name, string MimeType, Stream Content) : IDisposable
    {
        public void Dispose()
        {
            Content.Dispose();

            GC.SuppressFinalize(this);
        }
    }

    public record EmailMessage(EmailAddress From, IReadOnlyList<EmailAddress> Tos, string Subject, string Body,
        IReadOnlyList<EmailAddress> CCs, IReadOnlyList<EmailAddress> BCCs, IReadOnlyList<EmailAttachment> Attachments)
    {
        public EmailMessage(EmailAddress from, EmailAddress to, string subject, string body,
            IReadOnlyList<EmailAttachment>? attachments = null)
            : this(from, new List<EmailAddress> { to }, subject, body,
                Array.Empty<EmailAddress>(), Array.Empty<EmailAddress>(), attachments ?? Array.Empty<EmailAttachment>())
        { }

        public EmailMessage(EmailAddress from, EmailAddress to, string subject, string body,
            IReadOnlyList<EmailAddress> ccs, IReadOnlyList<EmailAddress> bccs,
            IReadOnlyList<EmailAttachment>? attachments = null)
            : this(from, new List<EmailAddress> { to }, subject, body, ccs, bccs, attachments ?? Array.Empty<EmailAttachment>()) { }

        public EmailMessage(EmailAddress from, IReadOnlyList<EmailAddress> tos, string subject, string body,
            IReadOnlyList<EmailAttachment>? attachments = null)
            : this(from, tos, subject, body,
                Array.Empty<EmailAddress>(), Array.Empty<EmailAddress>(), attachments ?? Array.Empty<EmailAttachment>())
        { }
    }

    public interface IEmailSender
    {
        Task SendEmailAsync(EmailMessage message, CancellationToken cancellationToken);

    }
}
