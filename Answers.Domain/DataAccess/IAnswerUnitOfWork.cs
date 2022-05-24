using Answers.Domain.DataAccess.Repositories;
using SharedKernel.Domain.Interfaces;

namespace Answers.Domain.DataAccess
{
    public interface IAnswerUnitOfWork : IUnitOfWork
    {
        IAnswerRepository AnswerRepository { get; }
    }
}
