using System;
using System.Collections.Generic;
using System.Reflection;

namespace Trains.NET.Engine
{
    public class Factory<T> where T : class
    {
        private readonly Dictionary<Type, T> _services;

        public Factory(OrderedList<T> services)
        {
            _services = new Dictionary<Type, T>();
            foreach (T? service in services)
            {
                if (service == null) continue;

                Type? context = service.GetType().GetCustomAttribute<ContextAttribute>(true)?.Context;

                if (context == null) continue;

                if (!_services.ContainsKey(context))
                {
                    _services.Add(context, service);
                }
            }
        }

        public T Get(Type type)
        {
            return _services.GetValueOrDefault(type);
        }
    }
}
