﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MassTransitWebApp
{
    public interface IDifferentMessageHandler<TMessage> where TMessage : IDifferentMessage { }
}