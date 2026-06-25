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

    private const int MaxImagesPerRequest = 5;
    private const long MaxImageSizeInBytes = 5 * 1024 * 1024;

    public UploadVehicleImageValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.CreatedBy).NotEmpty();
        RuleFor(x => x.Images)
            .NotEmpty()
            .WithMessage("At least one image is required.")
            .Must(images => images.Count <= MaxImagesPerRequest)
            .WithMessage($"A maximum of {MaxImagesPerRequest} images can be uploaded per request.");

        RuleForEach(x => x.Images).ChildRules(image =>
        {
            image.RuleFor(x => x.Length)
                .GreaterThan(0)
                .WithMessage("Image is required.")
                .LessThanOrEqualTo(MaxImageSizeInBytes)
                .WithMessage("Image cannot exceed 5 MB.");

            image.RuleFor(x => x.ContentType)
                .Must(contentType => AllowedContentTypes.Contains(contentType))
                .WithMessage("Only JPEG, PNG and WebP images are allowed.");
        });
    }
}
