using Rentify.Backend.Core.Domain.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Backend.Core.Domain.Entities.Core
{
    public class TenantSettings : BaseEntity
    {
        public Guid TenantId { get; private set; }

        public string Currency { get; private set; }

        public string TimeZone { get; private set; }

        public string Language { get; private set; }

        public bool AllowOnlineReservations { get; private set; }

        public bool EnableNotifications { get; private set; }

        public bool EnableSmsNotifications { get; private set; }

        public bool EnableEmailNotifications { get; private set; }

        public string LogoUrl { get; private set; }

        public string PrimaryColor { get; private set; }

        public string SecondaryColor { get; private set; }

        public Tenant Tenant { get; private set; }

        // EF
        private TenantSettings()
        {
        }

        private TenantSettings(
            Guid tenantId,
            string createdBy)
        {
            TenantId = tenantId;

            Currency = "DOP";
            TimeZone = "America/Santo_Domingo";
            Language = "es";

            AllowOnlineReservations = true;
            EnableNotifications = true;
            EnableSmsNotifications = false;
            EnableEmailNotifications = true;

            PrimaryColor = "#000000";
            SecondaryColor = "#FFFFFF";

            CreatedDate = DateTime.UtcNow;
            CreatedBy = createdBy;
            ModifiedBy = createdBy;
            ModifiedDate = CreatedDate;
        }

        public static TenantSettings Create(
            Guid tenantId,
            string createdBy)
        {
            return new TenantSettings(
                tenantId,
                createdBy);
        }

        public void UpdateBranding(
            string logoUrl,
            string primaryColor,
            string secondaryColor,
            string modifiedBy)
        {
            LogoUrl = logoUrl;
            PrimaryColor = primaryColor;
            SecondaryColor = secondaryColor;

            ModifiedBy = modifiedBy;
            ModifiedDate = DateTime.UtcNow;
        }
    }
}
