namespace Trains.NET.Engine;

[Order(140)]
public class ToggleAutosaveCommand : ICommand
{
    private readonly IGameStateManager _gameStateManager;

    public string Name => "Toggle Autosave";

    public ToggleAutosaveCommand(IGameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;
    }

    public void Execute()
    {
        _gameStateManager.AutosaveEnabled = !_gameStateManager.AutosaveEnabled;
    }
}
