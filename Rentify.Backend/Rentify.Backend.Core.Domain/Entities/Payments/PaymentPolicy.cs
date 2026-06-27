using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Constants;
using Rentify.Backend.Core.Domain.Entities.Core;
using Rentify.Backend.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Backend.Core.Domain.Entities.Payments
{
    public sealed class PaymentPolicy : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid TenantId { get; private set; }

        public string Name { get; private set; } = null!;

        public PaymentFrequency PaymentFrequency { get; private set; }

        public DayOfWeek CutoffDayOfWeek { get; private set; }

        public int GraceDays { get; private set; }

        public DayOfWeek ReminderStartDayOfWeek { get; private set; }

        public bool LateFeeEnabled { get; private set; }

        public bool IsDefault { get; private set; }

        public Tenant Tenant { get; private set; } = null!;

        private PaymentPolicy()
        {
        }

        private PaymentPolicy(
            Guid id,
            Guid tenantId,
            string name,
            PaymentFrequency paymentFrequency,
            DayOfWeek cutoffDayOfWeek,
            int graceDays,
            DayOfWeek reminderStartDayOfWeek,
            bool lateFeeEnabled,
            bool isDefault,
            string createdBy)
        {
            Id = id;
            TenantId = tenantId;
            Name = name.Trim();
            PaymentFrequency = paymentFrequency;
            CutoffDayOfWeek = cutoffDayOfWeek;
            GraceDays = graceDays;
            ReminderStartDayOfWeek = reminderStartDayOfWeek;
            LateFeeEnabled = lateFeeEnabled;
            IsDefault = isDefault;

            IsActive = true;
            IsDeleted = false;
            CreatedBy = createdBy;
            ModifiedBy = createdBy;
            CreatedDate = DateTime.UtcNow;
            ModifiedDate = CreatedDate;
        }

        public static PaymentPolicy CreateDefault(
            Guid tenantId,
            BusinessModel businessModel,
            string createdBy)
        {
            if (businessModel == BusinessModel.DriverFleetRental)
                return CreateWeeklyDriverDefault(tenantId, createdBy);

            if (businessModel == BusinessModel.Mixed)
                return CreateWeeklyDriverDefault(tenantId, createdBy);

            return CreateTraditionalDefault(tenantId, createdBy);
        }

        private static PaymentPolicy CreateTraditionalDefault(
            Guid tenantId,
            string createdBy)
        {
            ValidateBase(tenantId, createdBy);

            return new PaymentPolicy(
                Guid.NewGuid(),
                tenantId,
                PaymentPolicyNames.UNIQUE_PAY,
                PaymentFrequency.OneTime,
                DayOfWeek.Sunday,
                graceDays: 0,
                reminderStartDayOfWeek: DayOfWeek.Sunday,
                lateFeeEnabled: false,
                isDefault: true,
                createdBy);
        }

        private static PaymentPolicy CreateWeeklyDriverDefault(
            Guid tenantId,
            string createdBy)
        {
            ValidateBase(tenantId, createdBy);

            return new PaymentPolicy(
                Guid.NewGuid(),
                tenantId,
                PaymentPolicyNames.SEMANAL_PAY,
                PaymentFrequency.Weekly,
                DayOfWeek.Sunday,
                graceDays: 2,
                reminderStartDayOfWeek: DayOfWeek.Wednesday,
                lateFeeEnabled: true,
                isDefault: true,
                createdBy);
        }

        public void Update(
            string name,
            PaymentFrequency paymentFrequency,
            DayOfWeek cutoffDayOfWeek,
            int graceDays,
            DayOfWeek reminderStartDayOfWeek,
            bool lateFeeEnabled,
            bool isDefault,
            string modifiedBy)
        {
            Validate(name, paymentFrequency, graceDays, modifiedBy);

            Name = name.Trim();
            PaymentFrequency = paymentFrequency;
            CutoffDayOfWeek = cutoffDayOfWeek;
            GraceDays = graceDays;
            ReminderStartDayOfWeek = reminderStartDayOfWeek;
            LateFeeEnabled = lateFeeEnabled;
            IsDefault = isDefault;

            ModifiedBy = modifiedBy;
            ModifiedDate = DateTime.UtcNow;
        }

        public DateOnly GetOverdueStartDate(DateOnly dueDate)
        {
            return dueDate.AddDays(GraceDays + 1);
        }

        public bool IsOverdue(DateOnly dueDate, DateOnly currentDate)
        {
            return currentDate >= GetOverdueStartDate(dueDate);
        }

        private static void ValidateBase(Guid tenantId, string createdBy)
        {
            if (tenantId == Guid.Empty)
                throw new ArgumentException("Tenant Id is required.");

            if (string.IsNullOrWhiteSpace(createdBy))
                throw new ArgumentException("CreatedBy is required.");
        }

        private static void Validate(
            string name,
            PaymentFrequency paymentFrequency,
            int graceDays,
            string user)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Payment policy name is required.");

            if (name.Trim().Length > 100)
                throw new ArgumentException("Payment policy name is too long.");

            if (!Enum.IsDefined(typeof(PaymentFrequency), paymentFrequency))
                throw new ArgumentException("Invalid payment frequency.");

            if (graceDays < 0)
                throw new ArgumentException("Grace days cannot be negative.");

            if (graceDays > 15)
                throw new ArgumentException("Grace days cannot be greater than 15.");

            if (string.IsNullOrWhiteSpace(user))
                throw new ArgumentException("User is required.");
        }
    }
}
