using System;
using Answers.Domain.ETOs;
using Answers.Domain.Validators;
using FluentValidation;
using SharedKernel.Domain.Abstracts;
using SharedKernel.Domain.Others;

namespace Answers.Domain.Entities
{
    public class Answer : AggregateRoot<Guid>
    {
        private readonly AnswerValidator _Validator = new();

        private Answer() : base(Guid.NewGuid())
        {
            Name = string.Empty;
        }

        public Answer(string name, Guid questionId, Guid createdById, DateTime created) : this()
        {
            Name = name;
            QuestionId = questionId;

            CreatedById = createdById;
            Created = created;

            AddEvent(new EventInformation(new AnswerCreated(Id), true));
        }

        public string Name { get; private set; }
        public const int NameMaxLength = 64;

        public Guid QuestionId { get; private set; }

        public Guid CreatedById { get; private set; }
        public DateTime Created { get; private set; }

        public Guid? UpdatedById { get; private set; }
        public DateTime? Updated { get; private set; }

        public void UpdateState(string name, Guid updatedBy, DateTime updated)
        {
            Name = name;

            UpdatedById = updatedBy;
            Updated = updated;
        }

        public override void Validate()
        {
            _Validator.ValidateAndThrow(this);
        }
    }
}
