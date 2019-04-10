using Autofac;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransitWebApp
{
    /// <summary>
    ///  This consumer listens for only one type of message <see cref="Message"/> and on that event
    ///  publishes a new <see cref="DifferentMessage"/>.
    /// </summary>
    public class MessageConsumer : IConsumer<Message>
    {
        private readonly OperationContext op;
        private readonly IPublishIdentityIntegrationEvents bus;
        private readonly ILifetimeScope scope;
        public MessageConsumer(OperationContext context, IPublishIdentityIntegrationEvents bus, ILifetimeScope scope)
        {
            op = context;
            this.bus = bus;
            this.scope = scope;
        }

        public async Task Consume(ConsumeContext<Message> context)
        {
            await bus.Publish(new DifferentMessage(), default(CancellationToken));
        }
    }
}
