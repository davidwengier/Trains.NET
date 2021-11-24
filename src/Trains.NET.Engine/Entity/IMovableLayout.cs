namespace Trains.NET.Engine;

public interface IMovableLayout : IEnumerable<IMovable>
{
    int Count { get; }
    int IndexOf(IMovable movable);
    IMovable this[int index] { get; }
    IMovable? GetAt(int column, int row);
    void Add(IMovable movable);
    void Remove(IMovable movable);
    // TODO: Move this out of here!
    IEnumerable<(Track, Train, float)> LastTrackLeases { get; }
}
