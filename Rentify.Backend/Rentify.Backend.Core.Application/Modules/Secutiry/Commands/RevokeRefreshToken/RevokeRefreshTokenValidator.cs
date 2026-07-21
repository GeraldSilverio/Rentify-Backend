using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Commands.RevokeRefreshToken
{
    public class RevokeRefreshTokenValidator : AbstractValidator<RevokeRefreshTokenCommand>
    {
        public RevokeRefreshTokenValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("El refresh token es requerido.");
        }
    }
}
