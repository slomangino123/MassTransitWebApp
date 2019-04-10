using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransitWebApp
{
    public class IdentityIntegrationEventPublisher
        : IPublishIdentityIntegrationEvents
    {
        private readonly IBusControl bus;
        private readonly ILogger<IdentityIntegrationEventPublisher> logger;
        private readonly OperationContext opContext;

        public IdentityIntegrationEventPublisher(IBusControl bus, ILogger<IdentityIntegrationEventPublisher> logger, OperationContext opContext)
        {
            this.bus = bus;
            this.logger = logger;
            this.opContext = opContext;
        }

        public Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : class
        {
            try
            {
                return bus.Publish(@event,
                                   context =>
                                   {
                                       context.CorrelationId = opContext.GetCorrelationId();
                                       context.InitiatorId = opContext.GetCausationId();
                                   },
                                   cancellationToken);
            }
            catch (Exception e)
            {
                // if the publish fails, log the exception message
                logger.LogError($"Failed to publish event = {@event.GetType().Name} with exception = {e.Message}");
                return Task.FromException(e);
            }
        }
    }
}
