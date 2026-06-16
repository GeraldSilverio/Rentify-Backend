using Rentify.Backend.Core.Application.Modules.Payments.Commands.RegisterPayment;

namespace Rentify.Backend.Core.Application.Modules.Payments.Contracts.Services;

public interface IPaymentService
{
    Task<RegisterPaymentResponse> RegisterAsync(RegisterPaymentCommand command, CancellationToken cancellationToken = default);
}
