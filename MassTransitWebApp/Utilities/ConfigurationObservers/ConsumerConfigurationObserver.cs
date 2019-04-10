using Autofac;
using GreenPipes;
using MassTransit.ConsumeConfigurators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MassTransitWebApp
{
    class ConsumerConfigurationObserver :
            IConsumerConfigurationObserver
    {
        readonly HashSet<Type> _consumerTypes;
        readonly ILifetimeScope _lifetimeScope;
        readonly HashSet<Tuple<Type, Type>> _messageTypes;

        public ConsumerConfigurationObserver(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
            _consumerTypes = new HashSet<Type>();
            _messageTypes = new HashSet<Tuple<Type, Type>>();
        }

        public HashSet<Type> ConsumerTypes => _consumerTypes;

        public HashSet<Tuple<Type, Type>> MessageTypes => _messageTypes;

        void IConsumerConfigurationObserver.ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
        {
            _consumerTypes.Add(typeof(TConsumer));
        }

        /// <summary>
        /// This is where a Filter is added for each Consumer/Message pair that is registered to an endpoint.
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="configurator"></param>
        void IConsumerConfigurationObserver.ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
        {
            _messageTypes.Add(Tuple.Create(typeof(TConsumer), typeof(TMessage)));

            if (_lifetimeScope.IsRegistered<OperationContextFilter<TMessage>>())
            {
                OperationContextFilter<TMessage> filter;
                if (_lifetimeScope.TryResolve(out filter))
                {
                    configurator.Message(m => m.UseFilter(filter));
                }
            }
        }
    }
}
