using Trains.NET.Engine;

namespace Trains.NET.Rendering;

[Order(10)]
public class TrackTool(ILayout<Track> trackLayout, IEnumerable<IStaticEntityFactory<Track>> entityFactories) : ITool
{
    private readonly ILayout<Track> _entityCollection = trackLayout;
    private readonly IEnumerable<IStaticEntityFactory<Track>> _entityFactories = entityFactories;

    public ToolMode Mode => ToolMode.Build;
    public string Name => "Track";

    public void Execute(int column, int row, ExecuteInfo info)
    {
        if (info.FromColumn == 0 && _entityCollection.TryGet(column, row, out Track? track))
        {
            _entityCollection.SelectedEntity = track;
        }
        else
        {
            _entityCollection.Add(column, row, _entityFactories, info.FromColumn, info.FromRow);
            _entityCollection.SelectedEntity = null;
        }
    }

    public bool IsValid(int column, int row) => _entityCollection.IsAvailable(column, row);
}
