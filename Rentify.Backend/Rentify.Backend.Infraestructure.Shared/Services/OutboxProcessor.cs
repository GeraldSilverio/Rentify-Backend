using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Shared.Contracts;
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
        }

        private async Task ProcessMessageAsync(
            OutboxMessage message,
            CancellationToken cancellationToken)
        {
            using IDisposable? logScope = _logger.BeginScope(new Dictionary<string, object?>
            {
                ["CorrelationId"] = message.CorrelationId,
                ["TenantId"] = message.TenantId,
                ["OutboxMessageId"] = message.Id,
                ["OutboxMessageType"] = message.Type
            });

            _logger.LogDebug(
                "Processing outbox message {OutboxMessageId} of type {OutboxMessageType} in Tenant {TenantId}",
                message.Id,
                message.Type,
                message.TenantId);

            try
            {
                if (!_handlers.TryGetValue(message.Type, out IOutboxMessageHandler? handler))
                {
                    throw new InvalidOperationException(
                        $"No outbox handler registered for message type: {message.Type}");
                }

                message.MarkAsProcessing(SystemUser);
                await _context.SaveChangesAsync(cancellationToken);

                await handler.HandleAsync(message.Payload, cancellationToken);

                message.MarkAsProcessed(SystemUser);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Processed outbox message {OutboxMessageId} of type {OutboxMessageType} in Tenant {TenantId}",
                    message.Id,
                    message.Type,
                    message.TenantId);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing outbox message {OutboxMessageId} of type {OutboxMessageType} in Tenant {TenantId}",
                    message.Id,
                    message.Type,
                    message.TenantId);

                message.MarkAsFailed(ex.Message, SystemUser);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
