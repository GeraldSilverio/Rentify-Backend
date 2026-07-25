using System.Diagnostics;
using System.Net.Mail;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.SendTemplateEmail;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Reservations.Emails;
using Rentify.Backend.Core.Application.Modules.Reservations.Events;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Contracts;

namespace Rentify.Backend.Infraestructure.Shared.Services.OutBox;

public sealed class ReservationCreatedOutboxHandler : IOutboxMessageHandler
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IReservationRepository _reservationRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<ReservationCreatedOutboxHandler> _logger;

    public ReservationCreatedOutboxHandler(
        IReservationRepository reservationRepository,
        IEmailService emailService,
        ILogger<ReservationCreatedOutboxHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public string Type => OutboxMessageTypes.ReservationCreated;

    public async Task HandleAsync(
        string payload,
        CancellationToken cancellationToken = default)
    {
        ReservationCreatedEvent notification = Deserialize(payload);
        string templateCode;

        try
        {
            templateCode = ReservationCreatedEmailTemplateResolver.Resolve(
                notification.Channel);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            _logger.LogError(
                exception,
                "Unsupported reservation channel {ReservationChannel} for Reservation {ReservationId}",
                notification.Channel,
                notification.ReservationId);
            throw;
        }

        Stopwatch stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "Processing reservation created email for Reservation {ReservationId}",
            notification.ReservationId);

        try
        {
            ReservationCreatedEmailData? emailData =
                await _reservationRepository.GetCreatedEmailDataAsync(
                    notification.TenantId,
                    notification.ReservationId,
                    cancellationToken);

            if (emailData is null)
            {
                _logger.LogWarning(
                    "Reservation created email was skipped because Reservation {ReservationId} was not found in Tenant {TenantId}",
                    notification.ReservationId,
                    notification.TenantId);
                return;
            }

            if (!MailAddress.TryCreate(emailData.CustomerEmail, out _))
            {
                _logger.LogWarning(
                    "Reservation created email was skipped because Customer {CustomerId} has no valid email for Reservation {ReservationId}",
                    emailData.CustomerId,
                    emailData.ReservationId);
                return;
            }

            IReadOnlyDictionary<string, string> variables =
                ReservationCreatedEmailVariablesBuilder.Build(emailData);

            await _emailService.SendEmailAsync(
                new SendTemplateEmailCommand(
                    notification.TenantId,
                    templateCode,
                    emailData.CustomerEmail,
                    variables.ToDictionary()),
                cancellationToken);

            stopwatch.Stop();

            _logger.LogInformation(
                "Reservation created email sent for Reservation {ReservationId} using Template {EmailTemplateCode} in {ElapsedMilliseconds} ms",
                notification.ReservationId,
                templateCode,
                stopwatch.ElapsedMilliseconds);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            stopwatch.Stop();

            _logger.LogError(
                exception,
                "Failed to send reservation created email for Reservation {ReservationId} using Template {EmailTemplateCode}",
                notification.ReservationId,
                templateCode);
            throw;
        }
    }

    private static ReservationCreatedEvent Deserialize(string payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
        {
            throw new InvalidOperationException(
                "ReservationCreated payload is required.");
        }

        ReservationCreatedEvent? notification =
            JsonSerializer.Deserialize<ReservationCreatedEvent>(payload, JsonOptions);

        if (notification is null
            || notification.TenantId == Guid.Empty
            || notification.ReservationId == Guid.Empty)
        {
            throw new InvalidOperationException(
                "ReservationCreated payload contains invalid identifiers.");
        }

        return notification;
    }
}
