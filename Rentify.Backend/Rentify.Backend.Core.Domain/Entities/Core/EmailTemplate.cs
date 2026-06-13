using Rentify.Backend.Core.Domain.Commons;

namespace Rentify.Backend.Core.Domain.Entities.Core
{
    public class EmailTemplate : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid? TenantId { get; private set; }
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string Subject { get; private set; }
        public string HtmlBody { get; private set; }
        public string? TextBody { get; private set; }
        public Tenant? Tenant { get; private set; }

        private EmailTemplate()
        {
        }

        private EmailTemplate(
            Guid? tenantId,
            string code,
            string name,
            string subject,
            string htmlBody,
            string? textBody,
            string createdBy)
        {
            Id = Guid.NewGuid();
            TenantId = tenantId;
            Code = code.Trim().ToUpperInvariant();
            Name = name.Trim();
            Subject = subject.Trim();
            HtmlBody = htmlBody;
            TextBody = textBody;
            IsActive = true;
            CreatedBy = createdBy;
            ModifiedBy = createdBy;
            CreatedDate = DateTime.UtcNow;
            ModifiedDate = CreatedDate;
        }

        public static EmailTemplate Create(
            Guid? tenantId,
            string code,
            string name,
            string subject,
            string htmlBody,
            string? textBody,
            string createdBy)
        {
            return new EmailTemplate(tenantId, code, name, subject, htmlBody, textBody, createdBy);
        }

        public void Update(
            string name,
            string subject,
            string htmlBody,
            string? textBody,
            string modifiedBy)
        {
            Name = name.Trim();
            Subject = subject.Trim();
            HtmlBody = htmlBody;
            TextBody = textBody;
            ModifiedBy = modifiedBy;
            ModifiedDate = DateTime.UtcNow;
        }
    }
}
