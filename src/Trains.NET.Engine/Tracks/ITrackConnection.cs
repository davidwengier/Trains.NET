namespace Trains.NET.Engine;

public interface ITrackConnection
{
    bool IsConnectedRight();
    bool IsConnectedDown();
    bool IsConnectedLeft();
    bool IsConnectedUp();

    bool CanConnectRight();
    bool CanConnectDown();
    bool CanConnectLeft();
    bool CanConnectUp();
}
