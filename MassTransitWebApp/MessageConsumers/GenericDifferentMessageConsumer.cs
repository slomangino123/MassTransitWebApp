using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MassTransitWebApp
{
    /// <summary>
    /// This is a generic consumer, it listens for any event that implements <see cref="IDifferentMessage"/>
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class GenericDifferentMessageConsumer<TMessage> : IConsumer<TMessage>
        where TMessage : class, IDifferentMessage
    {
        private readonly OperationContext opContext;
        public GenericDifferentMessageConsumer(OperationContext opContext)
        {
            this.opContext = opContext;
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
        }
    }
}
