using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Backend.Core.Application.Modules.Shared.Dtos.Information
{
    public sealed record ContactInformationDto(
        string Email,
        string PhoneNumber,
        string WhatsApp
    );
}
