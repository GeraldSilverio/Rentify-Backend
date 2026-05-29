using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Dtos.Request
{
    public sealed record CreateOwnerRequest(
    string FullName,
    string Email,
    string Password,
    Guid TenantId);
}
