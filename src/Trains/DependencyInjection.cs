using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Trains.NET.Comet;
using Trains.NET.Engine;
using Trains.NET.Engine.Tracks;

namespace Trains
{
    public static class Services
    {
        public static ServiceProvider GetServiceProvider()
        {
            var col = new ServiceCollection();
            foreach (Assembly a in GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if (t.IsInterface)
                    {
                        WireUpOrderedList(col, t);
                        WireUpFunc(col, t);
                    }
                    else
                    {
                        foreach (Type inter in t.GetInterfaces())
                        {
                            if ((inter.Namespace?.StartsWith("Trains.NET", StringComparison.OrdinalIgnoreCase)) != true)
                            {
                                continue;
                            }
                            if (inter.IsGenericType &&
                                t.IsGenericType)
                            {
                                continue;
                            }


                            if (inter.Equals(typeof(IStaticEntity)))
                            {
                                WireUpFilteredLayout(col, t);
                            }

                            if (inter.GetCustomAttribute<TransientAttribute>(true) != null)
                            {
                                col.AddTransient(inter, t);
                            }
                            else
                            {
                                col.AddSingleton(inter, t);
                            }
                        }
                    }
                }
            }

            // TODO: Move this somewhere better
            col.AddSingleton<MainPage, MainPage>();

            return col.BuildServiceProvider();

            static IEnumerable<Assembly> GetAssemblies()
            {
                yield return typeof(NET.Engine.IGameBoard).Assembly;
                yield return typeof(NET.Rendering.IGame).Assembly;
                yield return typeof(NET.Rendering.Skia.SKCanvasWrapper).Assembly;
                yield return typeof(MainWindow).Assembly;
                yield return typeof(MainPage).Assembly;
            }

            static void WireUpOrderedList(ServiceCollection col, Type t)
            {
                Type orderedListOfT = typeof(OrderedList<>).MakeGenericType(t);
                col.AddSingleton(orderedListOfT, sp => Activator.CreateInstance(orderedListOfT, sp.GetServices(t)));
            }

            static void WireUpFunc(ServiceCollection col, Type t)
            {
                Type orderedListOfT = typeof(OrderedList<>).MakeGenericType(t);
                Type factoryType = typeof(Factory<>).MakeGenericType(t);
                col.AddSingleton(factoryType, sp => Activator.CreateInstance(factoryType, sp.GetService(orderedListOfT)));
            }

            static void WireUpFilteredLayout(ServiceCollection col, Type t)
            {
                Type filterLayoutOfT = typeof(FilteredLayout<>).MakeGenericType(t);
                Type injectedType = typeof(ILayout<>).MakeGenericType(t);
                col.AddSingleton(injectedType, filterLayoutOfT);
            }
        }
    }
}
