namespace Trains.NET.Engine;

public interface ITogglable
{
    string Name { get; }
    bool Enabled { get; set; }
}
