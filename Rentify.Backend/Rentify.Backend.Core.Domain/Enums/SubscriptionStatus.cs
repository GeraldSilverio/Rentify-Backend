using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Backend.Core.Domain.Enums
{
    public enum SubscriptionStatus
    {
        Pending = 1,
        Active = 2,
        Expired = 3,
        Suspended = 4,
        Cancelled = 5
    }
}
