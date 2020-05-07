using System;

namespace Trains.NET.Engine
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class OrderAttribute : Attribute
    {
        public int Order { get; set; }

        public OrderAttribute(int order)
        {
            this.Order = order;
        }
    }
}
