using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Domain.Entities.Core
{
    public class TenantEmailConfiguration : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid TenantId { get; private set; }
        public EmailProviderType Provider { get; private set; }
        public string ApiKey { get; private set; }
        public string FromEmail { get; private set; }
        public string FromName { get; private set; }
        public bool IsDefault { get; private set; }
        public Tenant Tenant { get; private set; }

        private TenantEmailConfiguration()
        {
        }

        private TenantEmailConfiguration(
            Guid tenantId,
            EmailProviderType provider,
            string apiKey,
            string fromEmail,
            string fromName,
            bool isDefault,
            string createdBy)
        {
            Id = Guid.NewGuid();
            TenantId = tenantId;
            Provider = provider;
            ApiKey = apiKey;
            FromEmail = fromEmail.Trim();
            FromName = fromName.Trim();
            IsDefault = isDefault;
            IsActive = true;
            CreatedBy = createdBy;
            ModifiedBy = createdBy;
            CreatedDate = DateTime.UtcNow;
            ModifiedDate = CreatedDate;
        }

        public static TenantEmailConfiguration Create(
            Guid tenantId,
            EmailProviderType provider,
            string apiKey,
            string fromEmail,
            string fromName,
            bool isDefault,
            string createdBy)
        {
            return new TenantEmailConfiguration(tenantId, provider, apiKey, fromEmail, fromName, isDefault, createdBy);
        }

        public void Update(
            string apiKey,
            string fromEmail,
            string fromName,
            bool isDefault,
            string modifiedBy)
        {
            ApiKey = apiKey;
            FromEmail = fromEmail.Trim();
            FromName = fromName.Trim();
            IsDefault = isDefault;
            ModifiedBy = modifiedBy;
            ModifiedDate = DateTime.UtcNow;
        }
    }
}
