using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    internal class NatureRenderer : ICachableLayerRenderer
    {
        private readonly ITreeRenderer _treeRenderer;
        private readonly ILayout _collection;
        private readonly IGameParameters _gameParameters;

        public bool IsDirty => true;

        public bool Enabled { get; set; } = true;

        public string Name => "Nature";

        public NatureRenderer(ITreeRenderer treeRenderer, ILayout collection, IGameParameters gameParameters)
        {
            _treeRenderer = treeRenderer;
            _collection = collection;
            _gameParameters = gameParameters;
        }

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            foreach (var tree in _collection.OfType<Tree>())
            {
                (int x, int y) = pixelMapper.CoordsToViewPortPixels(tree.Column, tree.Row);

                if (x < -_gameParameters.CellSize || y < -_gameParameters.CellSize || x > width || y > height) continue;

                canvas.Save();

                canvas.Translate(x, y);

                canvas.ClipRect(new Rectangle(0, 0, _gameParameters.CellSize, _gameParameters.CellSize), ClipOperation.Intersect, false);

                _treeRenderer.Render(canvas, tree.Seed);

                canvas.Restore();
            }
        }
    }
}
