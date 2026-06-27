using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Payments.Commands.RegisterPayment;
using Rentify.Backend.Core.Application.Modules.Payments.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Payments.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Domain.Entities;
using Rentify.Backend.Core.Domain.Entities.Payments;
using Rentify.Backend.Core.Domain.Entities.Reservations;

namespace Rentify.Backend.Core.Application.Modules.Payments.Implementations.Services;

public sealed class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RegisterPaymentResponse> RegisterAsync(RegisterPaymentCommand command, CancellationToken cancellationToken = default)
    {
        Reservation reservation = await _reservationRepository.GetByIdAsync(command.TenantId, command.ReservationId, cancellationToken)
                                  ?? throw new ApiException("Reservation not found.", StatusCodes.Status404NotFound);

        reservation.RegisterPayment(command.Amount, command.Method, command.Reference, command.CreatedBy);

        Payment payment = Payment.Create(
            command.TenantId,
            command.ReservationId,
            command.Amount,
            command.Method,
            command.Reference,
            command.CreatedBy);

        string invoiceNumber = await _paymentRepository.GenerateInvoiceNumberAsync(command.TenantId, cancellationToken);

        Invoice invoice = Invoice.Create(
            command.TenantId,
            payment.Id,
            command.ReservationId,
            invoiceNumber,
            command.Amount,
            command.CreatedBy);

        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _paymentRepository.AddInvoiceAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterPaymentResponse(payment.Id, invoice.Id, invoice.InvoiceNumber);
    }
}
