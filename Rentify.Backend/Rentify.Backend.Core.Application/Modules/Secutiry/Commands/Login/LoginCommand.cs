using MediatR;
using Rentify.Backend.Core.Application.Modules.Secutiry.Dtos.Response;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Commands.Login
{
    public record LoginCommand(string UserName,string Password) : IRequest<ResultReponse<LoginResponse>>;
}
