using System;
using System.Collections.Generic;
using System.Linq;
using Trains.NET.Engine;
using Trains.NET.Rendering.UI;

namespace Trains.NET.Rendering;

[Order(200)]
public class TrackSelectionPanel : PanelBase
{
    private static readonly PaintBrush s_trackHighlightBrush = Brushes.PanelBorder with { Color = Colors.LightYellow };

    private readonly ILayout<Track> _layout;
    private readonly IPixelMapper _pixelMapper;
    private readonly IEnumerable<IStaticEntityFactory<Track>> _entityFactories;
    private readonly IEnumerable<IStaticEntityRenderer<Track>> _renderers;
    private readonly List<ButtonBase> _multiButtons = new();

    protected override bool AutoClose => true;
    protected override PanelPosition Position => PanelPosition.Floating;

    public TrackSelectionPanel(ILayout<Track> layout, IPixelMapper pixelMapper, IEnumerable<IStaticEntityFactory<Track>> entityFactories, IEnumerable<IStaticEntityRenderer<Track>> renderers)
    {
        _layout = layout;
        _pixelMapper = pixelMapper;
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
                CreateMultiButton(track);
                this.Visible = true;
            }

            OnChanged();
        };
    }

    private void CreateMultiButton(Track track)
    {
        var column = track.Column;
        var row = track.Row;
        var (x, y, _) = _pixelMapper.CoordsToViewPortPixels(column, row + 1);

        int maxButtons = 0;
        _multiButtons.Clear();
        foreach (var factory in _entityFactories.Reverse())
        {
            var buttons = new List<ButtonBase>();
            foreach (Track newEntity in factory.GetPossibleReplacements(column, row, track))
            {
                buttons.Add(new TrackButton(newEntity, () => IsActive(track, newEntity), () => OnClick(column, row, newEntity), _renderers)
                {
                    TransparentBackground = true
                });
            }
            if (buttons.Count > 0)
            {
                maxButtons = Math.Max(maxButtons, buttons.Count);
                _multiButtons.Add(new MultiButton(40, buttons.ToArray()));
            }
        }

        _multiButtons.Add(new MultiButton(40, new PictureButton(Picture.Eraser, 20, () => false, () => Erase(column, row))
        {
            TransparentBackground = true
        }));

        this.InnerWidth = maxButtons * 40;
        this.InnerHeight = _multiButtons.Count * 40;
    }

    private void Erase(int column, int row)
    {
        _layout.Remove(column, row);
    }

    private void OnClick(int column, int row, Track newEntity)
    {
        _layout.Set(column, row, newEntity);
    }

    private static bool IsActive(Track track, Track newEntity)
        => newEntity.GetType() == track.GetType() &&
            newEntity.Identifier.Equals(track.Identifier);

    protected override bool HandlePointerAction(int x, int y, PointerAction action)
    {
        foreach (var button in _multiButtons)
        {
            if (button.HandleMouseAction(x, y, action))
            {
                break;
            }
            y -= 40;
        }
        if (action == PointerAction.Click)
        {
            this.Visible = false;
        }
        return true;
    }

    protected override void PreRender(ICanvas canvas)
    {
        // Normally this method is just used to set position etc. but we are also 
        // going to cheat and draw the track highlight before the panel has a chance
        // to translate to the right position

        var track = _layout.SelectedEntity;
        if (track is not null)
        {
            var (x, y, onScreen) = _pixelMapper.CoordsToViewPortPixels(track.Column, track.Row);
            if (!onScreen)
            {
                return;
            }
            canvas.DrawRect(x, y, _pixelMapper.CellSize, _pixelMapper.CellSize, s_trackHighlightBrush);

            this.Top = y + _pixelMapper.CellSize + 5;
            this.Left = x - this.InnerWidth / 2 + _pixelMapper.CellSize / 2 - (GetPanelWidth() - this.InnerWidth) / 2;
        }
    }

    protected override void Render(ICanvas canvas)
    {
        foreach (var button in _multiButtons)
        {
            button.Render(canvas);
            canvas.Translate(0, 40);
        }
    }
}
