using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Domain.Entities;

public class Vehicle : BaseEntity
{
    private readonly List<VehicleUnavailableDate> _unavailableDates = [];

    public Guid Id { get; private set; }
    public Guid RentCarId { get; private set; }
    public Guid ModelId { get; private set; }
    public int Year { get; private set; }
    public string PlateNumber { get; private set; } = null!;
    public string Vin { get; private set; } = null!;
    public string Color { get; private set; } = null!;
    public decimal DailyRate { get; private set; }
    public VehicleStatus Status { get; private set; }
    public string ImageUrl { get; private set; } = null!;
    public string ImagePublicId { get; private set; } = null!;

    public RentCar RentCar { get; private set; } = null!;
    public Model Model { get; private set; } = null!;
    public IReadOnlyCollection<VehicleUnavailableDate> UnavailableDates => _unavailableDates.AsReadOnly();

    private Vehicle()
    {
    }

    private Vehicle(
        Guid id,
        Guid rentCarId,
        Guid modelId,
        int year,
        string plateNumber,
        string vin,
        string color,
        decimal dailyRate,
        string imageUrl,
        string imagePublicId,
        string createdBy)
    {
        Id = id;
        RentCarId = rentCarId;
        ModelId = modelId;
        Year = year;
        PlateNumber = plateNumber;
        Vin = vin;
        Color = color;
        DailyRate = dailyRate;
        ImageUrl = imageUrl;
        ImagePublicId = imagePublicId;
        Status = VehicleStatus.Available;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
    }

    public static Vehicle Create(
        Guid rentCarId,
        Guid modelId,
        int year,
        string plateNumber,
        string vin,
        string color,
        decimal dailyRate,
        string imageUrl,
        string imagePublicId,
        string createdBy)
    {
        if (rentCarId == Guid.Empty)
            throw new ArgumentException("Rent car Id is required.");

        if (modelId == Guid.Empty)
            throw new ArgumentException("Model Id is required.");

        Validate(year, plateNumber, vin, dailyRate, imageUrl, imagePublicId);

        return new Vehicle(
            Guid.NewGuid(),
            rentCarId,
            modelId,
            year,
            NormalizePlateNumber(plateNumber),
            vin.Trim().ToUpperInvariant(),
            color?.Trim() ?? string.Empty,
            dailyRate,
            imageUrl.Trim(),
            imagePublicId.Trim(),
            createdBy);
    }

    public void ChangeStatus(VehicleStatus status, string modifiedBy)
    {
        Status = status;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void AddUnavailableDate(DateOnly startDate, DateOnly endDate, string? reason, string createdBy)
    {
        if (_unavailableDates.Any(x => !x.IsDeleted && x.Overlaps(startDate, endDate)))
            throw new ArgumentException("Vehicle already has an unavailable range that overlaps these dates.");

        _unavailableDates.Add(VehicleUnavailableDate.Create(Id, startDate, endDate, reason, createdBy));
        Status = VehicleStatus.Unavailable;
        ModifiedBy = createdBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public bool IsAvailableFor(DateOnly startDate, DateOnly endDate)
    {
        return Status == VehicleStatus.Available
               && !_unavailableDates.Any(x => !x.IsDeleted && x.Overlaps(startDate, endDate));
    }

    private static void Validate(
        int year,
        string plateNumber,
        string vin,
        decimal dailyRate,
        string imageUrl,
        string imagePublicId)
    {
        if (year < 1980 || year > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("Vehicle year is invalid.");

        if (string.IsNullOrWhiteSpace(plateNumber))
            throw new ArgumentException("Plate number is required.");

        if (string.IsNullOrWhiteSpace(vin))
            throw new ArgumentException("VIN is required.");

        if (vin.Trim().Length != 17)
            throw new ArgumentException("VIN must contain 17 characters.");

        if (dailyRate <= 0)
            throw new ArgumentException("Daily rate must be greater than zero.");

        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("Image URL is required.");

        if (string.IsNullOrWhiteSpace(imagePublicId))
            throw new ArgumentException("Image public Id is required.");
    }

    private static string NormalizePlateNumber(string plateNumber)
    {
        return plateNumber.Trim().ToUpperInvariant().Replace("-", string.Empty).Replace(" ", string.Empty);
    }
}
