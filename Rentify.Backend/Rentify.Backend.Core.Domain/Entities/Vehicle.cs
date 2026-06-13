using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Domain.Entities;

public class Vehicle : BaseEntity
{
    private readonly List<VehicleImage> _images = [];
    private readonly List<VehicleUnavailableDate> _unavailableDates = [];

    public Guid Id { get; private set; }
    public Guid RentCarId { get; private set; }
    public string Make { get; private set; } = null!;
    public string Model { get; private set; } = null!;
    public int Year { get; private set; }
    public string PlateNumber { get; private set; } = null!;
    public string Vin { get; private set; } = null!;
    public string Color { get; private set; } = null!;
    public decimal DailyRate { get; private set; }
    public VehicleStatus Status { get; private set; }

    public RentCar RentCar { get; private set; } = null!;
    public IReadOnlyCollection<VehicleImage> Images => _images.AsReadOnly();
    public IReadOnlyCollection<VehicleUnavailableDate> UnavailableDates => _unavailableDates.AsReadOnly();

    private Vehicle()
    {
    }

    private Vehicle(
        Guid id,
        Guid rentCarId,
        string make,
        string model,
        int year,
        string plateNumber,
        string vin,
        string color,
        decimal dailyRate,
        string createdBy)
    {
        Id = id;
        RentCarId = rentCarId;
        Make = make;
        Model = model;
        Year = year;
        PlateNumber = plateNumber;
        Vin = vin;
        Color = color;
        DailyRate = dailyRate;
        Status = VehicleStatus.Available;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
    }

    public static Vehicle Create(
        Guid rentCarId,
        string make,
        string model,
        int year,
        string plateNumber,
        string vin,
        string color,
        decimal dailyRate,
        string createdBy)
    {
        if (rentCarId == Guid.Empty)
            throw new ArgumentException("Rent car Id is required.");

        Validate(make, model, year, plateNumber, vin, dailyRate);

        return new Vehicle(
            Guid.NewGuid(),
            rentCarId,
            make.Trim(),
            model.Trim(),
            year,
            NormalizePlateNumber(plateNumber),
            vin.Trim().ToUpperInvariant(),
            color?.Trim() ?? string.Empty,
            dailyRate,
            createdBy);
    }

    public void ChangeStatus(VehicleStatus status, string modifiedBy)
    {
        Status = status;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void AddImage(VehicleImage image, string modifiedBy)
    {
        if (image.VehicleId != Id)
            throw new ArgumentException("Image does not belong to this vehicle.");

        if (image.IsPrimary)
        {
            foreach (VehicleImage currentImage in _images.Where(x => !x.IsDeleted))
                currentImage.UnmarkAsPrimary(modifiedBy);
        }

        _images.Add(image);
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void DeleteImage(Guid imageId, string modifiedBy)
    {
        VehicleImage image = _images.FirstOrDefault(x => x.Id == imageId && !x.IsDeleted)
                             ?? throw new ArgumentException("Vehicle image not found.");

        image.Delete(modifiedBy);
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
        string make,
        string model,
        int year,
        string plateNumber,
        string vin,
        decimal dailyRate)
    {
        if (string.IsNullOrWhiteSpace(make))
            throw new ArgumentException("Make is required.");

        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model is required.");

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
    }

    private static string NormalizePlateNumber(string plateNumber)
    {
        return plateNumber.Trim().ToUpperInvariant().Replace("-", string.Empty).Replace(" ", string.Empty);
    }
}
