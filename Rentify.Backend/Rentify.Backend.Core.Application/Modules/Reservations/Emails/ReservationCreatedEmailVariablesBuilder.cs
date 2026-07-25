using System.Globalization;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Helpers;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Emails;

public static class ReservationCreatedEmailVariablesBuilder
{
    private const string MissingValue = "No disponible";
    private const string MissingLocation = "No especificada";

    private static readonly CultureInfo DominicanCulture =
        CultureInfo.GetCultureInfo("es-DO");

    public static IReadOnlyDictionary<string, string> Build(
        ReservationCreatedEmailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new Dictionary<string, string>
        {
            ["ReservationCode"] = data.ReservationCode,
            ["CustomerFirstName"] = data.CustomerFirstName,
            ["TenantName"] = data.TenantName,
            ["CreatedAt"] = EmailDateFormatter.FormatDate(data.CreatedAtUtc),
            ["VehicleDescription"] = BuildVehicleDescription(data),
            ["RentalType"] = TranslateRentalType(data.RentalType),
            ["RentalPeriod"] = FormatRentalPeriod(data.Quantity, data.RentalType),
            ["DeliveryDateTime"] = EmailDateFormatter.FormatDate(data.DeliveryDateTime),
            ["DeliveryLocation"] = FormatLocation(
                data.DeliveryLocationName,
                data.DeliveryAddressDetails),
            ["ExpectedReturnDateTime"] = EmailDateFormatter.FormatDate(data.ExpectedReturnDateTime),
            ["ReturnLocation"] = FormatLocation(
                data.ReturnLocationName,
                data.ReturnAddressDetails),
            ["UnitRate"] = FormatMoney(data.UnitRate),
            ["RentalAmount"] = FormatMoney(data.RentalAmount),
            ["DeliveryFee"] = FormatMoney(data.DeliveryFee),
            ["ReturnFee"] = FormatMoney(data.ReturnFee),
            ["SecurityDepositDisplay"] = data.SecurityDepositAmount > 0
                ? FormatMoney(data.SecurityDepositAmount)
                : "No aplica",
            ["DiscountAmount"] = FormatMoney(data.DiscountAmount),
            ["TotalAmount"] = FormatMoney(data.TotalAmount),
            ["TenantPhone"] = FormatContact(data.TenantPhone),
            ["TenantWhatsApp"] = FormatContact(data.TenantWhatsApp),
            ["TenantEmail"] = FormatContact(data.TenantEmail)
        };
    }

    public static string TranslateRentalType(RentalType rentalType)
    {
        return rentalType switch
        {
            RentalType.Daily => "Diaria",
            RentalType.Weekly => "Semanal",
            RentalType.Monthly => "Mensual",
            _ => throw new ArgumentOutOfRangeException(
                nameof(rentalType),
                rentalType,
                "Unsupported rental type.")
        };
    }

    public static string FormatRentalPeriod(int quantity, RentalType rentalType)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                quantity,
                "Reservation quantity must be greater than zero.");
        }

        string unit = rentalType switch
        {
            RentalType.Daily => quantity == 1 ? "día" : "días",
            RentalType.Weekly => quantity == 1 ? "semana" : "semanas",
            RentalType.Monthly => quantity == 1 ? "mes" : "meses",
            _ => throw new ArgumentOutOfRangeException(
                nameof(rentalType),
                rentalType,
                "Unsupported rental type.")
        };

        return $"{quantity} {unit}";
    }

    public static string FormatMoney(decimal amount)
    {
        return $"RD${amount.ToString("N2", DominicanCulture)}";
    }

    public static string FormatLocation(string? name, string? details)
    {
        string? normalizedName = Normalize(name);
        string? normalizedDetails = Normalize(details);

        if (normalizedName is null)
        {
            return normalizedDetails ?? MissingLocation;
        }

        return normalizedDetails is null
            ? normalizedName
            : $"{normalizedName} — {normalizedDetails}";
    }

    public static string BuildVehicleDescription(ReservationCreatedEmailData data)
    {
        string description = string.Join(
            " ",
            new[]
            {
                Normalize(data.VehicleBrandName),
                Normalize(data.VehicleModelName),
                data.VehicleYear > 0
                    ? data.VehicleYear.ToString(CultureInfo.InvariantCulture)
                    : null
            }.Where(value => value is not null));

        string? plateNumber = Normalize(data.VehiclePlateNumber);

        if (plateNumber is not null)
        {
            description = string.IsNullOrEmpty(description)
                ? $"Placa {plateNumber}"
                : $"{description} · Placa {plateNumber}";
        }

        return string.IsNullOrEmpty(description) ? MissingValue : description;
    }

    private static string FormatContact(string? value)
    {
        return Normalize(value) ?? MissingValue;
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
