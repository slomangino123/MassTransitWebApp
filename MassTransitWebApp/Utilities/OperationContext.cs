using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MassTransitWebApp
{
    public class OperationContext
    {
        public OperationContext()
        {
            Organization = new Dictionary<string, string>();
            Application = new Dictionary<string, string>();
            Case = new Dictionary<string, string>();
        }

        public IDictionary<string, string> Organization { get; }

        public IDictionary<string, string> Application { get; }

        public IDictionary<string, string> Case { get; }
    }
}
