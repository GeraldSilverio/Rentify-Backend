using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.SendTemplateEmail;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Secutiry;
using Rentify.Backend.Core.Application.Modules.Tenants.Events;
using Rentify.Backend.Core.Application.Shared.Constants;
using Rentify.Backend.Core.Application.Shared.Contracts;
using Rentify.Backend.Core.Application.Shared.Helpers;
using Rentify.Backend.Core.Domain.Entities.Events;
using Rentify.Backend.Core.Domain.Enums;
using Rentify.Backend.Infraestructure.Persistence.Context;
namespace Rentify.Backend.Infraestructure.Shared.Services
{
    public sealed class OutboxProcessor : IOutboxProcessor
    {
        private readonly RentifyContext _context;
        private readonly IReadOnlyDictionary<string, IOutboxMessageHandler> _handlers;
        private readonly ILogger<OutboxProcessor> _logger;

        private const string SystemUser = "outbox-worker";

        public OutboxProcessor(
            RentifyContext context,
            IEnumerable<IOutboxMessageHandler> handlers,
            ILogger<OutboxProcessor> logger)
        {
            _context = context;
            _handlers = handlers.ToDictionary(x => x.Type, x => x);
            _logger = logger;
        }

        public async Task ProcessPendingMessagesAsync(CancellationToken cancellationToken = default)
        {
            List<OutboxMessage> messages = await _context.OutboxMessages
                .Where(x =>
                    x.Status == OutboxMessageStatus.Pending &&
                    !x.IsDeleted &&
                    (x.NextRetryDate == null || x.NextRetryDate <= DateTime.UtcNow))
                .OrderBy(x => x.CreatedDate)
                .Take(20)
                .ToListAsync(cancellationToken);

            foreach (OutboxMessage message in messages)
            {
                await ProcessMessageAsync(message, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task ProcessMessageAsync(
            OutboxMessage message,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!_handlers.TryGetValue(message.Type, out IOutboxMessageHandler? handler))
                {
                    throw new InvalidOperationException(
                        $"No outbox handler registered for message type: {message.Type}");
                }

                message.MarkAsProcessing(SystemUser);

                await handler.HandleAsync(message.Payload, cancellationToken);

                message.MarkAsProcessed(SystemUser);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing outbox message {OutboxMessageId} of type {OutboxMessageType}",
                    message.Id,
                    message.Type);

                message.MarkAsFailed(ex.Message, SystemUser);
            }
        }
    }
}
