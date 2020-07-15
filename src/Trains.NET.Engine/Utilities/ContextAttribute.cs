using System;

namespace Trains.NET.Engine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ContextAttribute : Attribute
    {
        public Type Context { get; }

        public ContextAttribute(Type contextType)
        {
            this.Context = contextType;
        }
    }
}
