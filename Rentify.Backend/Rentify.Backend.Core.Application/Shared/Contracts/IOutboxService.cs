using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Backend.Core.Application.Shared.Contracts
{
    public interface IOutboxService
    {
        Task AddAsync<TPayload>(
            Guid? tenantId,
            string type,
            TPayload payload,
            string createdBy,
            string? correlationId = null,
            CancellationToken cancellationToken = default);
    }
}
