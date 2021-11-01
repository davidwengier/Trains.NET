namespace Trains.NET.Engine;

public interface ICommand
{
    string Name { get; }
    void Execute();
}
