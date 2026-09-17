using Trains.NET.Engine;

namespace Trains.NET.Tests;

internal class NullTool : ITool
{
    public ToolMode Mode => throw new System.NotImplementedException();

    public string Name => throw new System.NotImplementedException();

    public void Execute(int column, int row, ExecuteInfo info)
    {
        throw new System.NotImplementedException();
    }

    public bool IsValid(int column, int row)
    {
        throw new System.NotImplementedException();
    }
}
