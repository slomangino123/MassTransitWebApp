﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransitWebApp
{
    public interface IPublishIdentityIntegrationEvents
    {
        Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : class;
    }
}