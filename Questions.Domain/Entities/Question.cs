using FluentValidation;
using Questions.Domain.Validators;
using SharedKernel.Domain.Abstracts;
using System;

namespace Questions.Domain.Entities
{
    public class Question : AggregateRoot<Guid>
    {
        private readonly QuestionValidator _Validator = new();

        private Question() : base(Guid.NewGuid())
        {
            Name = string.Empty;
        }

        public Question(string name, Guid createdById, DateTime created) : this()
        {
            Name = name;

            CreatedById = createdById;
            Created = created;
        }

        public string Name { get; private set; }
        public const int NameMaxLength = 64;

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
