using Questions.Domain.DataAccess;
using Questions.Domain.DataAccess.Repositories;
using Questions.EntityFramework;
using SharedKernel.Infrastructure.Abstracts;

namespace Questions.Infrastructure
{
    public class QuestionUnitOfWork : EntityFrameworkUnitOfWork, IQuestionUnitOfWork
    {
        public QuestionUnitOfWork(ApplicationDbContext context,
            IQuestionRepository questionRepository) : base(context)
        {
            QuestionRepository = questionRepository;
        }

        public IQuestionRepository QuestionRepository { get; }
    }
}
