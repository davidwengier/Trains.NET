using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(10)]
    public class TrackTool : ITool
    {
        private readonly ILayout<Track> _entityCollection;
        private readonly IEnumerable<IStaticEntityFactory<Track>> _entityFactories;

        public ToolMode Mode => ToolMode.Build;
        public string Name => "Track";

        public TrackTool(ILayout<Track> trackLayout, IEnumerable<IStaticEntityFactory<Track>> entityFactories)
        {
            _entityCollection = trackLayout;
            _entityFactories = entityFactories;
        }

        public void Execute(int column, int row, bool isPartOfDrag)
        {
            Execute(column, row, new ExecuteInfo(isPartOfDrag: isPartOfDrag, 0, 0));
        }

        public void Execute(int column, int row, ExecuteInfo info)
        {
            if (!info.IsPartOfDrag && _entityCollection.TryGet(column, row, out Track? track))
            {
                _entityCollection.SelectedEntity = track;
            }
            else
            {
                _entityCollection.Add(column, row, _entityFactories, info.IsPartOfDrag, info.FromColumn, info.FromRow);
                _entityCollection.SelectedEntity = null;
            }
        }

        public bool IsValid(int column, int row) => _entityCollection.IsAvailable(column, row);
    }
}
