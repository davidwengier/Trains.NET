namespace Trains.NET.Engine
{
    public interface ITool
    {
        ToolMode Mode { get; }

        string Name { get; }

        string Category { get; }

        void Execute(int column, int row);

        bool IsValid(int column, int row);
    }
}
