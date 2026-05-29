using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Dtos.Response
{
    public sealed record TokenResponse(
    string Token,
    DateTime ExpiresAt);
}
