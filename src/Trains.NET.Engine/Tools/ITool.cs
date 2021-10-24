namespace Trains.NET.Engine
{
    public interface ITool
    {
        ToolMode Mode { get; }

        string Name { get; }

        void Execute(int column, int row, bool isPartOfDrag);

        public void Execute(int column, int row, ExecuteInfo info)
        {
            Execute(column, row, info.IsPartOfDrag);
        }

        bool IsValid(int column, int row);
    }
}
