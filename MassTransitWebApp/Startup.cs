using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Autofac.Extensions.DependencyInjection;
using MassTransit;
using MassTransit.AutofacIntegration;
using MassTransit.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MassTransitWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IContainer Container { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var builder = new ContainerBuilder();

            // Register MessageHandlers, this is needed so we can find all event types that we care about
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(x => x.Name.EndsWith("Handler"))
                .AsImplementedInterfaces();

            // Register our message consumers
            builder.RegisterGeneric(typeof(GenericMessageConsumer<>))
                .AsSelf();
            builder.RegisterGeneric(typeof(GenericDifferentMessageConsumer<>))
                .AsSelf();

            // Register our consumers
            builder.RegisterType<MessageConsumer>();
            builder.RegisterType<DifferentMessageConsumer>();

            // Register the ConsumerConfigurationObserver
            builder.RegisterType<ConsumerConfigurationObserver>()
               .SingleInstance();

            builder.RegisterType<MassTransitAutofacConsumerRegistrationAdapter>()
                .AsSelf()
                .SingleInstance();
            builder.RegisterType<OperationContext>().InstancePerLifetimeScope();
            builder.RegisterType<IdentityIntegrationEventPublisher>().AsImplementedInterfaces();

            // Register our OperationContextFilter, this is the guy that does the work
            builder.RegisterGeneric(typeof(OperationContextFilter<>));

            builder.AddMassTransit(containerBuilderConfigurator =>
            {
                containerBuilderConfigurator.AddBus(componentContext =>
                {
                    var bc = Bus.Factory.CreateUsingRabbitMq(rabbitBusConfigurator =>
                    {
                        var host = rabbitBusConfigurator.Host("localhost", "/", hostConfigurator =>
                        {
                            hostConfigurator.Username("guest");
                            hostConfigurator.Password("guest");
                        });

                        rabbitBusConfigurator.ReceiveEndpoint("customer_update_queue", endpointConfigurator =>
                        {

                            var MessageTypes = componentContext.GetGenericParametersForRegisteredGenericTypes(typeof(IMessageHandler<>));
                            var differentMessageTypes = componentContext.GetGenericParametersForRegisteredGenericTypes(typeof(IDifferentMessageHandler<>));

                            // Use an adapter in order to register consumers for all event types
                            var registrationAdapter = componentContext.Resolve<MassTransitAutofacConsumerRegistrationAdapter>();
                            registrationAdapter.RegisterConsumersForHandlerEventTypes(endpointConfigurator, componentContext, typeof(GenericMessageConsumer<>), MessageTypes);
                            registrationAdapter.RegisterConsumersForHandlerEventTypes(endpointConfigurator, componentContext, typeof(GenericDifferentMessageConsumer<>), differentMessageTypes);

                            // To register consumers individually (simple way)
                            // ec.Consumer<MessageConsumer>(c);
                            // ec.Consumer<DifferentMessageConsumer>(c);

                        });

                        // Wire up the ConsumerConfigurationObserver. This will tell the MessageConfigurators
                        // to wire up Filters for each Consumer/Message pair
                        // The Github issue & MassTransit test that finally connected all the dots.
                        // https://github.com/MassTransit/MassTransit/issues/737
                        // https://github.com/MassTransit/MassTransit/blob/develop/src/Containers/MassTransit.Containers.Tests/Autofac_FilterSpecs.cs
                        rabbitBusConfigurator.ConnectConsumerConfigurationObserver(componentContext.Resolve<ConsumerConfigurationObserver>());

                    });

                    return bc;
                });
            });

            builder.Populate(services);
            Container = builder.Build();

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(Container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            app.UseMiddleware<AspnetCoreOperationContextMiddleware>();

            // Start our RabbitMQ bus
            var bus = Container.Resolve<IBusControl>();
            var busHandle = TaskUtil.Await(() => bus.StartAsync());

            lifetime.ApplicationStopping.Register(() => busHandle.Stop());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
