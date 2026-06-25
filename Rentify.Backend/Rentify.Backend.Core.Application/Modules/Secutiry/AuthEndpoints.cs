using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.ForgotPassword;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.Login;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.RefreshToken;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.ResetPassword;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.RevokeRefreshToken;

namespace Rentify.Backend.Core.Application.Modules.Secutiry
{
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(
            this IEndpointRouteBuilder app)
        {
            var auth = app.MapGroup("/api/v1/authentication")
                .WithTags("Authentication");
            auth.MapLogin();
            auth.MapForgotPassword();
            auth.MapResetPassword();
            auth.MapRefreshToken();
            auth.MapRevokeRefreshToken();
            return app;
        }
    }
}
