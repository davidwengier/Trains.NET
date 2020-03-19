using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Trains.NET.Engine
{
    public class OrderedList<T> : IEnumerable<T>
    {
        private readonly List<T> _list;

        public OrderedList(IEnumerable<object?> services)
        {
            _list = new List<T>(from svc in services
                                let order = svc.GetType().GetCustomAttribute<OrderAttribute>(true)?.Order ?? 0
                                orderby order
                                select (T)svc);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T item in _list)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
