using Rentify.Backend.Core.Domain.Commons;

namespace Rentify.Backend.Core.Domain.Entities.Core
{

    public class Tenant : BaseEntity
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Slug { get; private set; }

        public bool IsSuspended { get; private set; }

        public DateTime? SuspendedAt { get; private set; }

        public TenantSettings Settings { get; private set; }

        // EF
        private Tenant()
        {
        }

        private Tenant(
            Guid id,
            string name,
            string slug,
            string createdBy)
        {
            Id = id;
            Name = name;
            Slug = slug;
            IsActive = true;
            IsSuspended = false;

            CreatedDate = DateTime.UtcNow;
            CreatedBy = createdBy;
            ModifiedBy = createdBy;
            ModifiedDate = CreatedDate;
        }

        public static Tenant Create(
            string name,
            string slug,
            string createdBy)
        {
            Validate(name, slug);

            return new Tenant(
                Guid.NewGuid(),
                name.Trim(),
                slug.Trim().ToLower(),
                createdBy);
        }

        public void Suspend(string modifiedBy)
        {
            IsSuspended = true;
            SuspendedAt = DateTime.UtcNow;
            ModifiedBy = modifiedBy;
            ModifiedDate = DateTime.UtcNow;
        }

        public void Activate(string modifiedBy)
        {
            IsSuspended = false;
            SuspendedAt = null;
            ModifiedBy = modifiedBy;
            ModifiedDate = DateTime.UtcNow;
        }
        private static void Validate(string name, string slug)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tenant name is required.");

            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Tenant slug is required.");
        }
    }
}
