using Autofac;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MassTransitWebApp
{
    public class OperationContextFilter<T> :
    IFilter<ConsumeContext<T>>
    where T : class
    {
        public void Probe(ProbeContext context)
        {
        }

        /// <summary>
        /// Maps CorrelationId from message context to CausationId in <see cref="OperationContext"/>.
        /// Creates a new CorrelationId in <see cref="OperationContext"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            // Get the message scope so we can map the correlation/causation to the OperationContext.
            if (context.TryGetPayload<ILifetimeScope>(out var messageScope))
            {
                var operationContext = messageScope.Resolve<OperationContext>();
                operationContext.SetCorrelationId(Guid.NewGuid());
                operationContext.SetCausationId(context.CorrelationId ?? Guid.Empty);
            }

            return next.Send(context);
        }
    }
}
