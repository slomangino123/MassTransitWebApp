using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace MassTransitWebApp
{
    public class AspnetCoreOperationContextMiddleware
    {
        private const string CORRELATION_ID_HEADER = "X-Correlation-Id";

        private readonly RequestDelegate next;

        public AspnetCoreOperationContextMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Resolve our instance of the OperationContext for this request
            var opCtx = (OperationContext)context.RequestServices.GetService(typeof(OperationContext));

            // Add the correlationId to the request header if it is not already there
            var items = context.Request.Headers;
            if (!items.TryGetValue(CORRELATION_ID_HEADER, out var correlationObject) ||
                !Guid.TryParse(correlationObject[0], out Guid correlationId))
            {
                correlationId = Guid.NewGuid();
                items.Add(CORRELATION_ID_HEADER, new StringValues(correlationId.ToString()));
            }

            // Set the OperationContext's correlationId
            opCtx.SetCorrelationId(correlationId);

            await next.Invoke(context);
        }
    }
}
