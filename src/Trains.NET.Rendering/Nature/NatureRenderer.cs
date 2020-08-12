using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    internal class NatureRenderer : ICachableLayerRenderer
    {
        private readonly IRenderer<Tree> _treeRenderer;
        private readonly ILayout _collection;
        private readonly ITrackParameters _parameters;

        public bool IsDirty => true;

        public bool Enabled { get; set; } = true;

        public string Name => "Nature";

        public NatureRenderer(IRenderer<Tree> treeRenderer, ILayout collection, ITrackParameters parameters)
        {
            _treeRenderer = treeRenderer;
            _collection = collection;
            _parameters = parameters;
        }

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            foreach (Tree? tree in _collection.OfType<Tree>())
            {
                (int x, int y) = pixelMapper.CoordsToViewPortPixels(tree.Column, tree.Row);

                if (x < -_parameters.CellSize || y < -_parameters.CellSize || x > width || y > height) continue;

                canvas.Save();

                canvas.Translate(x, y);

                canvas.ClipRect(new Rectangle(0, 0, _parameters.CellSize, _parameters.CellSize), ClipOperation.Intersect, false);

                _treeRenderer.Render(canvas, tree);

                canvas.Restore();
            }
        }
    }
}
