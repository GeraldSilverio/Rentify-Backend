using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Admin.Commands.Brands.CreateVehicleBrand
{
    public sealed class CreateVehicleBrandValidator : AbstractValidator<CreateVehicleBrandCommand>
    {
        public CreateVehicleBrandValidator()
        {
            RuleFor(x => x.BrandName).NotNull().WithMessage("BrandName is required.")
                .NotEmpty().WithMessage("BrandName cannot be empty.")
                .MaximumLength(100).WithMessage("BrandName cannot exceed 100 characters.");
        }
    }
}
