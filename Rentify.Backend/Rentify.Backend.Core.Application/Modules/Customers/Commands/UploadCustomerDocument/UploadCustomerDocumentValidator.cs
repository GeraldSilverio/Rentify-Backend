using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.UploadCustomerDocument;

public sealed class UploadCustomerDocumentValidator : AbstractValidator<UploadCustomerDocumentCommand>
{
    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];

    public UploadCustomerDocumentValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Document).NotNull().WithMessage("Document is required.");
        RuleFor(x => x.Document.Length)
            .LessThanOrEqualTo(5 * 1024 * 1024)
            .When(x => x.Document is not null)
            .WithMessage("Document must be 5MB or less.");
        RuleFor(x => x.Document.ContentType)
            .Must(x => AllowedContentTypes.Contains(x))
            .When(x => x.Document is not null)
            .WithMessage("Only JPG, PNG and WEBP files are allowed.");
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
