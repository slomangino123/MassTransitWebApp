using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransitWebApp
{
    /// <summary>
    /// This is a generic consumer, it listens for any event that implements <see cref="IMessage"/>
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class GenericMessageConsumer<TMessage> : IConsumer<TMessage>
        where TMessage : class, IMessage
    {
        private readonly IPublishIdentityIntegrationEvents bus;
        private readonly OperationContext opContext;
        public GenericMessageConsumer(IPublishIdentityIntegrationEvents bus, OperationContext opContext)
        {
            this.bus = bus;
            this.opContext = opContext;
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            if (typeof(TMessage) == typeof(Message))
            {
                await bus.Publish(new DifferentMessage(), default(CancellationToken));
            }
        }
    }
}
