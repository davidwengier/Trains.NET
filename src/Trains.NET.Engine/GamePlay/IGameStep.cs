namespace Trains.NET.Engine;

public interface IGameStep
{
    void Update(long timeSinceLastTick);
}
