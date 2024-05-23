namespace Trains.NET.Engine;

[Order(140)]
public class ToggleAutosaveCommand(IGameStateManager gameStateManager) : ICommand
{
    private readonly IGameStateManager _gameStateManager = gameStateManager;

    public string Name => "Toggle Autosave";

    public void Execute()
    {
        _gameStateManager.AutosaveEnabled = !_gameStateManager.AutosaveEnabled;
    }
}
