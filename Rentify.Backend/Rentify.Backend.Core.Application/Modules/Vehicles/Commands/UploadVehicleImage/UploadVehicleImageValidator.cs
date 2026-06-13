using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UploadVehicleImage;

public sealed class UploadVehicleImageValidator : AbstractValidator<UploadVehicleImageCommand>
{
    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];

    private const long MaxImageSizeInBytes = 5 * 1024 * 1024;

    public UploadVehicleImageValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty().WithMessage("Vehicle Id is required.");
        RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Created by is required.");
        RuleFor(x => x.Image).NotNull().WithMessage("Image is required.");

        When(x => x.Image is not null, () =>
        {
            RuleFor(x => x.Image.Length)
                .GreaterThan(0)
                .WithMessage("Image is required.")
                .LessThanOrEqualTo(MaxImageSizeInBytes)
                .WithMessage("Image cannot exceed 5 MB.");

            RuleFor(x => x.Image.ContentType)
                .Must(contentType => AllowedContentTypes.Contains(contentType))
                .WithMessage("Only JPEG, PNG and WebP images are allowed.");
        });
    }
}
