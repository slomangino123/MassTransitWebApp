using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MassTransitWebApp
{
    public static class GenericRegistrationExtensions
    {
        public static IEnumerable<Type> GetRegisteredInstancesOfGenericType(this IComponentContext context, Type type)
        {
            // Check autofac for all the registrations for everything that implements the type
            return context
                        .ComponentRegistry
                        .Registrations.Where(r => !r.Activator.LimitType.IsAbstract && r.Activator.LimitType.GetInterfaces().Any(t => t.IsGenericType && (t.GetGenericTypeDefinition() == type)))
                        .Select(r => r.Activator.LimitType);
        }

        public static IEnumerable<Type> GetAllGenericRegisteredInstancesOfType(this IComponentContext context, Type type)
        {
            if (!type.IsGenericType)
            {
                throw new ArgumentException($"Type: {type.Name} is not a generic type.");
            }

            var types = context
                        .ComponentRegistry
                        .Registrations.Where(r => !r.Activator.LimitType.IsAbstract).SelectMany(r => r.Activator.LimitType.GetInterfaces().Where(t => t.IsGenericType && (t.GetGenericTypeDefinition() == type)));

            return types;
        }

        public static IEnumerable<Type> GetGenericParametersForRegisteredGenericTypes(this IComponentContext context, Type type)
        {
            if (!type.IsGenericType)
            {
                throw new ArgumentException($"Type: {type.Name} is not a generic type.");
            }

            var genericTypes = GetAllGenericRegisteredInstancesOfType(context, type);
            return genericTypes.Select(a => a.GetGenericArguments().First());
        }

        public static IEnumerable<Type> GetRegisteredInstancesOfType(this IComponentContext context, Type type)
        {
            // Check autofac for all the registrations for everything that implements the type
            return context
                        .ComponentRegistry
                        .Registrations.Where(r => r.Activator.LimitType.GetInterfaces().Any(t => t == type) && !r.Activator.LimitType.IsAbstract)
                        .Select(r => r.Activator.LimitType);
        }
    }
}
