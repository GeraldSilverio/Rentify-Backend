using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Commands.ValidateInactiveSubscriptions;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Queries.GetCurrentSubscription;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Queries.GetSubscriptionPlans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Backend.Core.Application.Modules.Secutiry
{
    public static class SubscriptionEndpoints
    {
        public static IEndpointRouteBuilder MapSubscriptionEndpoints(this IEndpointRouteBuilder app)
        {
            var subscription = app.MapGroup("/api/v1/subscription")
                .WithTags("Subscription");

            subscription.MapGetSubscriptionPlan();
            subscription.MapGetCurrentSubscription();
            subscription.MapValidateInactiveSubscriptions();
            return app;
        }
    }
}
