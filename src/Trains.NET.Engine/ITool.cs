namespace Trains.NET.Engine
{
    public interface ITool
    {
        string Name { get; }

        void Execute(int column, int row);
    }
}
