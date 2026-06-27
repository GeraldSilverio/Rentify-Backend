using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Domain.Entities.Core
{
    public sealed class TenantSettings : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid TenantId { get; private set; }

        public string CurrencyCode { get; private set; } = null!;

        public string TimeZone { get; private set; } = null!;

        public bool EnableReservations { get; private set; }

        public bool EnableDriverFleet { get; private set; }

        public bool EnableMaintenance { get; private set; }

        public bool EnableLateFees { get; private set; }

        public bool EnablePublicCatalog { get; private set; }

        public Tenant Tenant { get; private set; } = null!;

        private TenantSettings()
        {
        }

        private TenantSettings(
            Guid id,
            Guid tenantId,
            string currencyCode,
            string timeZone,
            bool enableReservations,
            bool enableDriverFleet,
            bool enableMaintenance,
            bool enableLateFees,
            bool enablePublicCatalog,
            string createdBy)
        {
            Id = id;
            TenantId = tenantId;
            CurrencyCode = currencyCode;
            TimeZone = timeZone;
            EnableReservations = enableReservations;
            EnableDriverFleet = enableDriverFleet;
            EnableMaintenance = enableMaintenance;
            EnableLateFees = enableLateFees;
            EnablePublicCatalog = enablePublicCatalog;

            IsActive = true;
            IsDeleted = false;
            CreatedBy = createdBy;
            ModifiedBy = createdBy;
            CreatedDate = DateTime.UtcNow;
            ModifiedDate = CreatedDate;
        }

        public static TenantSettings CreateDefault(
            Guid tenantId,
            BusinessModel businessModel,
            string createdBy)
        {
            if (tenantId == Guid.Empty)
                throw new ArgumentException("Tenant Id is required.");

            if (!Enum.IsDefined(typeof(BusinessModel), businessModel))
                throw new ArgumentException("Invalid business model.");

            if (string.IsNullOrWhiteSpace(createdBy))
                throw new ArgumentException("CreatedBy is required.");

            bool enableReservations =
                businessModel is BusinessModel.TraditionalRentCar or BusinessModel.Mixed;

            bool enableDriverFleet =
                businessModel is BusinessModel.DriverFleetRental or BusinessModel.Mixed;

            bool enablePublicCatalog =
                businessModel is BusinessModel.TraditionalRentCar or BusinessModel.Mixed;

            bool enableLateFees =
                businessModel is BusinessModel.DriverFleetRental or BusinessModel.Mixed;

            return new TenantSettings(
                Guid.NewGuid(),
                tenantId,
                "DOP",
                "America/Santo_Domingo",
                enableReservations,
                enableDriverFleet,
                enableMaintenance: true,
                enableLateFees,
                enablePublicCatalog,
                createdBy);
        }

        public void Update(
            string currencyCode,
            string timeZone,
            bool enableReservations,
            bool enableDriverFleet,
            bool enableMaintenance,
            bool enableLateFees,
            bool enablePublicCatalog,
            string modifiedBy)
        {
            if (string.IsNullOrWhiteSpace(currencyCode))
                throw new ArgumentException("Currency code is required.");

            if (currencyCode.Trim().Length > 10)
                throw new ArgumentException("Currency code is too long.");

            if (string.IsNullOrWhiteSpace(timeZone))
                throw new ArgumentException("Time zone is required.");

            if (timeZone.Trim().Length > 100)
                throw new ArgumentException("Time zone is too long.");

            if (string.IsNullOrWhiteSpace(modifiedBy))
                throw new ArgumentException("ModifiedBy is required.");

            CurrencyCode = currencyCode.Trim().ToUpperInvariant();
            TimeZone = timeZone.Trim();
            EnableReservations = enableReservations;
            EnableDriverFleet = enableDriverFleet;
            EnableMaintenance = enableMaintenance;
            EnableLateFees = enableLateFees;
            EnablePublicCatalog = enablePublicCatalog;

            ModifiedBy = modifiedBy;
            ModifiedDate = DateTime.UtcNow;
        }
    }
}
