namespace Trains.NET.Engine;

public interface ITool
{
    bool PauseGameDuringDrag { get; }

    string Name { get; }

    void Execute(int column, int row, ExecuteInfo info);

    bool IsValid(int column, int row);
}
