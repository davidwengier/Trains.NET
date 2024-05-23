namespace Trains.NET.Engine;

[AttributeUsage(AttributeTargets.Class)]
public sealed class OrderAttribute(int order) : Attribute
{
    public int Order { get; set; } = order;
}
