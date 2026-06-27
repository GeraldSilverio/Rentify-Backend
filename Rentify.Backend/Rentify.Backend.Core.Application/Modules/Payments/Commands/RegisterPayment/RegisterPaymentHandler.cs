using MediatR;
using Rentify.Backend.Core.Application.Modules.Payments.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Payments.Commands.RegisterPayment;

public sealed class RegisterPaymentHandler : IRequestHandler<RegisterPaymentCommand, ResultReponse<RegisterPaymentResponse>>
{
    private readonly IPaymentService _paymentService;

    public RegisterPaymentHandler(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<ResultReponse<RegisterPaymentResponse>> Handle(RegisterPaymentCommand request, CancellationToken cancellationToken)
    {
        RegisterPaymentResponse response = await _paymentService.RegisterAsync(request, cancellationToken);
        return ResultReponse<RegisterPaymentResponse>.Success(response);
    }
}
