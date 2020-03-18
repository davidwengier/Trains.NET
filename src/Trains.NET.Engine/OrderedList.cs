using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Trains.NET.Engine
{
    public class OrderedList
    {
        protected IEnumerable<object> List { get; private set; } = null!;

        public void AddRange(IEnumerable<object> enumerable)
        {
            this.List = enumerable;
        }
    }

    public class OrderedList<T> : OrderedList, IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in base.List.Cast<T>())
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
