using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Backend.Core.Domain.Enums
{
    public enum OutboxMessageStatus
    {
        Pending = 1,
        Processing = 2,
        Processed = 3,
        Failed = 4
    }
}
