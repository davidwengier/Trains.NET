using System.Data;

namespace Trains.NET.Engine
{
    public interface ITool
    {
        string Name { get; }

        void Execute(int column, int row);

        bool IsValid(int column, int row);
    }
}
