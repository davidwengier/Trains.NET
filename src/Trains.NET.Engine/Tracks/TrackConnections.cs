namespace Trains.NET.Engine;

public class TrackConnections(
    ITrackConnection? left,
    ITrackConnection? up,
    ITrackConnection? right,
    ITrackConnection? down)
{
    public ITrackConnection? Left { get; } = left;
    public ITrackConnection? Up { get; } = up;
    public ITrackConnection? Right { get; } = right;
    public ITrackConnection? Down { get; } = down;

    public int Count => (Up is null ? 0 : 1) +
        (Down is null ? 0 : 1) +
        (Right is null ? 0 : 1) +
        (Left is null ? 0 : 1);

    public static TrackConnections GetPotentialConnections(ILayout layout, int column, int row)
        => new(
            GetConnection(layout, column - 1, row, static connection => connection.CanConnectRight()),
            GetConnection(layout, column, row - 1, static connection => connection.CanConnectDown()),
            GetConnection(layout, column + 1, row, static connection => connection.CanConnectLeft()),
            GetConnection(layout, column, row + 1, static connection => connection.CanConnectUp()));

    public static TrackConnections GetConnectedConnections(
        ILayout layout,
        int column,
        int row,
        ITrackConnection connection)
        => new(
            connection.IsConnectedLeft()
                ? GetConnection(layout, column - 1, row, static neighbor => neighbor.IsConnectedRight())
                : null,
            connection.IsConnectedUp()
                ? GetConnection(layout, column, row - 1, static neighbor => neighbor.IsConnectedDown())
                : null,
            connection.IsConnectedRight()
                ? GetConnection(layout, column + 1, row, static neighbor => neighbor.IsConnectedLeft())
                : null,
            connection.IsConnectedDown()
                ? GetConnection(layout, column, row + 1, static neighbor => neighbor.IsConnectedUp())
                : null);

    private static ITrackConnection? GetConnection(
        ILayout layout,
        int column,
        int row,
        Func<ITrackConnection, bool> isConnected)
        => layout.TryGet(column, row, out var entity) &&
            entity is ITrackConnection connection &&
            isConnected(connection)
                ? connection
                : null;
}
