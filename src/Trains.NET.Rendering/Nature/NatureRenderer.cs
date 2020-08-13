using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public class NatureRenderer : ICachableLayerRenderer
    {
        private readonly IRenderer<Tree> _treeRenderer;
        private readonly ILayout<Tree> _collection;
        private readonly IGameParameters _gameParameters;
        private bool _dirty = true;

        public bool IsDirty => _dirty;

        public bool Enabled { get; set; } = true;

        public string Name => "Nature";

        public NatureRenderer(IRenderer<Tree> treeRenderer, ILayout<Tree> collection, IGameParameters gameParameters)
        {
            _treeRenderer = treeRenderer;
            _collection = collection;
            _gameParameters = gameParameters;

            _collection.CollectionChanged += (s, e) => _dirty = true;
        }

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            foreach (Tree? tree in _collection.OfType<Tree>())
            {
                (int x, int y) = pixelMapper.CoordsToViewPortPixels(tree.Column, tree.Row);

                if (x < -_gameParameters.CellSize || y < -_gameParameters.CellSize || x > width || y > height) continue;

                canvas.Save();

                canvas.Translate(x, y);

                canvas.ClipRect(new Rectangle(0, 0, _gameParameters.CellSize, _gameParameters.CellSize), ClipOperation.Intersect, false);

                _treeRenderer.Render(canvas, tree);

                canvas.Restore();
            }
            _dirty = false;
        }
    }
}
