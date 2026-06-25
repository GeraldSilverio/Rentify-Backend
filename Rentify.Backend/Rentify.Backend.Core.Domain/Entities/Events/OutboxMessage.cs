using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Domain.Entities.Events
{
    public sealed class OutboxMessage : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid? TenantId { get; private set; }
        public string Type { get; private set; } = null!;
        public string Payload { get; private set; } = null!;
        public OutboxMessageStatus Status { get; private set; }
        public int RetryCount { get; private set; }
        public int MaxRetries { get; private set; }
        public DateTime? ProcessedDate { get; private set; }
        public DateTime? NextRetryDate { get; private set; }
        public string? Error { get; private set; }
        public string? CorrelationId { get; private set; }

        private OutboxMessage()
        {
        }

        private OutboxMessage(
            Guid id,
            Guid? tenantId,
            string type,
            string payload,
            string? correlationId,
            string createdBy)
        {
            Id = id;
            TenantId = tenantId;
            Type = type;
            Payload = payload;
            Status = OutboxMessageStatus.Pending;
            RetryCount = 0;
            MaxRetries = 5;
            CorrelationId = correlationId;
            CreatedBy = createdBy;
            ModifiedBy = createdBy;
            CreatedDate = DateTime.UtcNow;
            ModifiedDate = CreatedDate;
            IsActive = true;
            IsDeleted = false;
        }

        public static OutboxMessage Create(
            Guid? tenantId,
            string type,
            string payload,
            string? correlationId,
            string createdBy)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Outbox message type is required.");

            if (string.IsNullOrWhiteSpace(payload))
                throw new ArgumentException("Outbox message payload is required.");

            if (string.IsNullOrWhiteSpace(createdBy))
                throw new ArgumentException("CreatedBy is required.");

            return new OutboxMessage(
                Guid.NewGuid(),
                tenantId,
                type.Trim(),
                payload,
                correlationId,
                createdBy);
        }

        public void MarkAsProcessing(string modifiedBy)
        {
            Status = OutboxMessageStatus.Processing;
            ModifiedBy = modifiedBy;
            ModifiedDate = DateTime.UtcNow;
        }

        public void MarkAsProcessed(string modifiedBy)
        {
            Status = OutboxMessageStatus.Processed;
            ProcessedDate = DateTime.UtcNow;
            Error = null;
            NextRetryDate = null;
            ModifiedBy = modifiedBy;
            ModifiedDate = DateTime.UtcNow;
        }

        public void MarkAsFailed(string error, string modifiedBy)
        {
            RetryCount++;

            Error = string.IsNullOrWhiteSpace(error)
                ? "Unknown error."
                : error.Length > 4000
                    ? error[..4000]
                    : error;

            Status = RetryCount >= MaxRetries
                ? OutboxMessageStatus.Failed
                : OutboxMessageStatus.Pending;

            NextRetryDate = RetryCount >= MaxRetries
                ? null
                : DateTime.UtcNow.AddMinutes(GetRetryDelayInMinutes());

            ModifiedBy = modifiedBy;
            ModifiedDate = DateTime.UtcNow;
        }

        private int GetRetryDelayInMinutes()
        {
            return RetryCount switch
            {
                <= 1 => 1,
                2 => 5,
                3 => 15,
                4 => 30,
                _ => 60
            };
        }
    }
}
