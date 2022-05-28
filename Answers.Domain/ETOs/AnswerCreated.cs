using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Answers.Domain.ETOs
{
    public record AnswerCreated(Guid AnswerId) : INotification;
}
