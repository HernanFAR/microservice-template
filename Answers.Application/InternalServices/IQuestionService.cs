using System;
using System.Threading;
using System.Threading.Tasks;

namespace Answers.Application.InternalServices
{
    public interface IQuestionService
    {
        Task<bool> Exist(Guid id, CancellationToken cancellationToken = default);

        Task<ReadOne.DTO?> ReadOneAsync(Guid id, CancellationToken cancellationToken = default);

    }

    public class ReadOne
    {
        public record DTO(Guid Id, string Name, DateTime Created, Guid CreatedById, DateTime? Updated, Guid? UpdatedById);
    }
}
