namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Services
{
    public record ValidateInactiveSubscriptionsResponse(
        int ValidatedSubscriptions,
        int ExpiredSubscriptions,
        int ActivatedSubscriptions);
}
