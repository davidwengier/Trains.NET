using System.Collections.Generic;
using Trains.NET.Engine;
using Trains.NET.Rendering.UI;

namespace Trains.NET.Rendering
{
    [Order(200)]
    public class TrackSelectionPanel : PanelBase
    {
        private readonly ILayout<Track> _layout;
        private readonly IEnumerable<IStaticEntityFactory<Track>> _entityFactories;
        private readonly IEnumerable<IStaticEntityRenderer<Track>> _renderers;
        private MultiButton? _multiButton;

        protected override PanelPosition Position => PanelPosition.Floating;
        protected override bool CanClose => true;

        public TrackSelectionPanel(ILayout<Track> layout, IPixelMapper pixelMapper, IEnumerable<IStaticEntityFactory<Track>> entityFactories, IEnumerable<IStaticEntityRenderer<Track>> renderers)
        {
            _layout = layout;
            _entityFactories = entityFactories;
            _renderers = renderers;
            this.InnerHeight = 40;

            this.Visible = false;
            _layout.SelectionChanged += (s, e) =>
            {
                this.Visible = false;

                var track = _layout.SelectedEntity;
                if (track is not null)
                {
                    DisplayTrackSelection(pixelMapper, track);
                }

                OnChanged();
            };
        }

        private void DisplayTrackSelection(IPixelMapper pixelMapper, Track track)
        {
            var buttons = new List<ButtonBase>();
            var column = track.Column;
            var row = track.Row;
            var (x, y, _) = pixelMapper.CoordsToViewPortPixels(column, row + 1);

            foreach (var factory in _entityFactories)
            {
                foreach (Track newEntity in factory.GetAllPossibleEntities(column, row))
                {
                    buttons.Add(new TrackButton(newEntity, () => IsActive(track, newEntity), () => OnClick(column, row, newEntity), _renderers)
                    {
                        TransparentBackground = true
                    });
                }
            }

            _multiButton = new MultiButton(40, buttons.ToArray());

            this.Top = y;
            this.InnerWidth = buttons.Count * 40;
            this.Left = x - this.InnerWidth / 2;

            this.Visible = buttons.Count > 1;
        }

        private void OnClick(int column, int row, Track newEntity)
        {
            _layout.Set(column, row, newEntity);
        }

        private static bool IsActive(Track track, Track newEntity)
        {
            return newEntity.GetType() == track.GetType() &&
                newEntity.Identifier.Equals(track.Identifier);
        }

        protected override bool HandlePointerAction(int x, int y, PointerAction action)
        {
            _multiButton?.HandleMouseAction(x, y, action);
            if (action == PointerAction.Click)
            {
                this.Visible = false;
            }
            return true;
        }

        protected override void Render(ICanvas canvas)
        {
            _multiButton?.Render(canvas);
        }
    }
}
