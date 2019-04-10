using Autofac;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MassTransitWebApp
{

    /// <summary>
    ///  This consumer listens for only one type of message <see cref="DifferentMessage"/>
    /// </summary>
    public class DifferentMessageConsumer : IConsumer<DifferentMessage>
    {
        private readonly OperationContext op;
        private readonly ILifetimeScope scope;

        public DifferentMessageConsumer(OperationContext context, ILifetimeScope scope)
        {
            op = context;
            this.scope = scope;
        }

        public async Task Consume(ConsumeContext<DifferentMessage> context)
        {
        }
    }
}
