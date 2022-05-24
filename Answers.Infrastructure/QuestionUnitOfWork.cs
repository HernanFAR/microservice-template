using Answers.Domain.DataAccess;
using Answers.Domain.DataAccess.Repositories;
using Answers.EntityFramework;
using SharedKernel.Infrastructure.Abstracts;

namespace Answers.Infrastructure
{
    public class AnswerUnitOfWork : EntityFrameworkUnitOfWork, IAnswerUnitOfWork
    {
        public AnswerUnitOfWork(ApplicationDbContext context,
            IAnswerRepository questionRepository) : base(context)
        {
            AnswerRepository = questionRepository;
        }

        public IAnswerRepository AnswerRepository { get; }
    }
}
