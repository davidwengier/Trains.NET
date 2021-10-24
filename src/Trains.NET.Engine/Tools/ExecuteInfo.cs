namespace Trains.NET.Engine
{
    public struct ExecuteInfo
    {
        public readonly bool IsPartOfDrag;
        public readonly int FromColumn;
        public readonly int FromRow;

        public ExecuteInfo(bool isPartOfDrag, int fromColumn, int fromRow)
        {
            IsPartOfDrag = isPartOfDrag;
            FromColumn = fromColumn;
            FromRow = fromRow;
        }
    }
}
