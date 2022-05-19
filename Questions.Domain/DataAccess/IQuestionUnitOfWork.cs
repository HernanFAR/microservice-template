using Questions.Domain.DataAccess.Repositories;
using SharedKernel.Domain.Interfaces;

namespace Questions.Domain.DataAccess
{
    public interface IQuestionUnitOfWork : IUnitOfWork
    {
        IQuestionRepository QuestionRepository { get; }
    }
}
