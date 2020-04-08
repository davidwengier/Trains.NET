using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Trains.NET.Engine;

namespace Trains.NET.Web
{
    public static class BuilderExtensions
    {
        public static void AddFromAssembly(this IServiceCollection services, Assembly assemblyToScan)
        {
            foreach (Type t in assemblyToScan.GetTypes())
            {
                if (t.IsInterface)
                {
                    Type orderedListOfT = typeof(OrderedList<>).MakeGenericType(t);
                    services.AddSingleton(orderedListOfT, sp => Activator.CreateInstance(orderedListOfT, sp.GetServices(t)));
                }
                else
                {
                    foreach (Type inter in t.GetInterfaces())
                    {
                        if (inter.Namespace?.StartsWith("Trains.NET", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            services.AddSingleton(inter, t);
                        }
                    }
                }
            }
        }
    }
}
