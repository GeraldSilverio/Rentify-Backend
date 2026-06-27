using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Backend.Core.Application.Modules.Shared.Contracts
{
    public interface IOutboxMessageHandler
    {
        string Type { get; }

        Task HandleAsync(
            string payload,
            CancellationToken cancellationToken = default);
    }
}
