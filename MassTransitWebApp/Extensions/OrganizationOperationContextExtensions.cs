using System;

namespace MassTransitWebApp
{
    public static class OrganizationOperationContextExtensions
    {
        private static readonly string CORRELATION_ID = "CorrelationId";
        private static readonly string CAUSATION_ID = "CausationId";

        public static void SetCorrelationId(this OperationContext operationContext, Guid value)
        {
            SetOrUpdateOrganizationValue(operationContext, CORRELATION_ID, value.ToString());
        }

        public static Guid GetCorrelationId(this OperationContext operationContext)
        {
            operationContext.Organization.TryGetValue(CORRELATION_ID, out string stringValue);
            Guid.TryParse(stringValue, out var returnValue);
            
            return returnValue;
        }

        public static void SetCausationId(this OperationContext operationContext, Guid value)
        {
            SetOrUpdateOrganizationValue(operationContext, CAUSATION_ID, value.ToString());
        }

        public static Guid GetCausationId(this OperationContext operationContext)
        {
            operationContext.Organization.TryGetValue(CAUSATION_ID, out string value);
            Guid.TryParse(value, out var returnValue);

            return returnValue;
        }

        private static void SetOrUpdateOrganizationValue(OperationContext operationContext, string key, string value)
        {
            operationContext.Organization[key] = value;
        }

        private static string GetOrganizationValue(this OperationContext operationContext, string key)
        {
            operationContext.Organization.TryGetValue(key, out string value);
            return value;
        }
    }
}
