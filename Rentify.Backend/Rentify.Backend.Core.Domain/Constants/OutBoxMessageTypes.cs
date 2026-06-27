using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Backend.Core.Application.Modules.Shared.Constants
{
    public static class OutboxMessageTypes
    {
        public const string TenantRegistered = "tenant.registered";
        public const string ReservationCreated = "reservation.created";
        public const string PaymentUploaded = "payment.uploaded";
        public const string PaymentConfirmed = "payment.confirmed";
        public const string PaymentOverdue = "payment.overdue";
        public const string MaintenanceDue = "maintenance.due";
    }
}
