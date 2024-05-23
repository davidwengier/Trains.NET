namespace Trains.NET.Engine;

public struct ExecuteInfo(int fromColumn, int fromRow)
{
    public readonly int FromColumn = fromColumn;
    public readonly int FromRow = fromRow;
}
