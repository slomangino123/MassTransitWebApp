using Autofac;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MassTransitWebApp
{
    public class MassTransitAutofacConsumerRegistrationAdapter
    {
        public void RegisterConsumersForHandlerEventTypes(IReceiveEndpointConfigurator endpointConfigurator,
                                                          IComponentContext componentContext,
                                                          Type consumerType,
                                                          IEnumerable<Type> eventTypes)
        {
            foreach (var eventType in eventTypes)
            {
                MethodInfo registerConsumerGeneric = GetType().GetMethod("RegisterConsumer")
                                                              .MakeGenericMethod(consumerType.MakeGenericType(eventType));

                registerConsumerGeneric.Invoke(this, new object[] { endpointConfigurator, componentContext });
            }
        }

        public static void RegisterConsumer<TConsumer>(IReceiveEndpointConfigurator ec, IComponentContext c)
            where TConsumer : class, IConsumer
        {
            ec.Consumer<TConsumer>(c);
        }
    }
}
