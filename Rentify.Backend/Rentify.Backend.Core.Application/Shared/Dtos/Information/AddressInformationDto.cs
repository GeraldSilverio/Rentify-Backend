using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Backend.Core.Application.Shared.Dtos.Information
{
    public record AddressInformationDto(
      string Street,
      string City,
      string Country = "Republica Dominicana");
}
