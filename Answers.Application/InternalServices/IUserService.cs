using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Answers.Application.InternalServices
{
    public interface IUserService
    {
        Task<Read.DTO?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

    }

    public class Read
    {
        public record DTO(Guid Id, string UserName, string Email, string PhoneNumber);
    }

}
