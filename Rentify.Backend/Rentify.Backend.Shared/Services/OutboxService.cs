using Rentify.Backend.Core.Application.Shared.Contracts;
using Rentify.Backend.Core.Domain.Entities.Events;
using Rentify.Backend.Infraestructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rentify.Backend.Infraestructure.Shared.Services
{
    public sealed class OutboxService : IOutboxService
    {
        private readonly RentifyContext _context;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public OutboxService(RentifyContext context)
        {
            _context = context;
        }

        public async Task AddAsync<TPayload>(
            Guid? tenantId,
            string type,
            TPayload payload,
            string createdBy,
            string? correlationId = null,
            CancellationToken cancellationToken = default)
        {
            string jsonPayload = JsonSerializer.Serialize(payload, JsonOptions);

            OutboxMessage message = OutboxMessage.Create(
                tenantId,
                type,
                jsonPayload,
                correlationId,
                createdBy);

            await _context.OutboxMessages.AddAsync(message, cancellationToken);
        }
    }
}
