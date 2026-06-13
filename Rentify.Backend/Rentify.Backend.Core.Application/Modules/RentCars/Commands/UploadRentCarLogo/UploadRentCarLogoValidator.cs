using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Commands.UploadRentCarLogo;

public sealed class UploadRentCarLogoValidator : AbstractValidator<UploadRentCarLogoCommand>
{
    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];

    private const long MaxLogoSizeInBytes = 2 * 1024 * 1024;

    public UploadRentCarLogoValidator()
    {
        RuleFor(x => x.RentCarId).NotEmpty().WithMessage("Rent car Id is required.");
        RuleFor(x => x.ModifiedBy).NotEmpty().WithMessage("Modified by is required.");
        RuleFor(x => x.Logo).NotNull().WithMessage("Logo is required.");

        When(x => x.Logo is not null, () =>
        {
            RuleFor(x => x.Logo.Length)
                .GreaterThan(0)
                .WithMessage("Logo is required.")
                .LessThanOrEqualTo(MaxLogoSizeInBytes)
                .WithMessage("Logo cannot exceed 2 MB.");

            RuleFor(x => x.Logo.ContentType)
                .Must(contentType => AllowedContentTypes.Contains(contentType))
                .WithMessage("Only JPEG, PNG and WebP images are allowed.");
        });
    }
}
