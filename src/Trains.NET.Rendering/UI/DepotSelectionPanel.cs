using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI;

[Order(200)]
public class DepotSelectionPanel : PanelBase
{
    private static readonly PaintBrush s_depotHighlightBrush = Brushes.PanelBorder with { Color = Colors.LightYellow };

    private readonly ILayout<Depot> _layout;
    private readonly IPixelMapper _pixelMapper;
    private readonly DepotFactory _depotFactory;
    private readonly IStaticEntityRenderer<Depot> _renderer;
    private MultiButton? _buttons;

    protected override bool AutoClose => true;
    protected override PanelPosition Position => PanelPosition.Floating;

    public DepotSelectionPanel(
        ILayout<Depot> layout,
        IPixelMapper pixelMapper,
        DepotFactory depotFactory,
        IStaticEntityRenderer<Depot> renderer)
    {
        _layout = layout;
        _pixelMapper = pixelMapper;
        _depotFactory = depotFactory;
        _renderer = renderer;
        InnerHeight = 40;

        Visible = false;
        _layout.SelectionChanged += (s, e) =>
        {
            Visible = false;

            if (_layout.SelectedEntity is { } depot)
            {
                CreateButtons(depot);
                Visible = true;
            }

            OnChanged();
        };
    }

    private void CreateButtons(Depot depot)
    {
        var buttons = _depotFactory
            .GetPossibleReplacements(depot.Column, depot.Row, depot)
            .Select(newDepot => new RendererButton<Depot>(
                newDepot,
                _renderer,
                () => newDepot.Direction == depot.Direction,
                () => _layout.Set(depot.Column, depot.Row, newDepot))
            {
                TransparentBackground = true
            })
            .Cast<ButtonBase>()
            .ToList();

        buttons.Add(new PictureButton(
            Picture.Eraser,
            20,
            () => false,
            () => _layout.Remove(depot.Column, depot.Row))
        {
            TransparentBackground = true
        });

        _buttons = new MultiButton(40, buttons.ToArray());
        InnerWidth = _buttons.Width;
    }

    protected override bool HandlePointerAction(int x, int y, PointerAction action)
    {
        _buttons?.HandleMouseAction(x, y, action);
        if (action == PointerAction.Click)
        {
            Visible = false;
        }

        return true;
    }

    protected override void PreRender(ICanvas canvas)
    {
        if (_layout.SelectedEntity is not { } depot)
        {
            return;
        }

        var (x, y, onScreen) = _pixelMapper.CoordsToViewPortPixels(depot.Column, depot.Row);
        if (!onScreen)
        {
            return;
        }

        canvas.DrawRect(x, y, _pixelMapper.CellSize, _pixelMapper.CellSize, s_depotHighlightBrush);

        Top = y + _pixelMapper.CellSize + 5;
        Left = x - InnerWidth / 2 + _pixelMapper.CellSize / 2 - (GetPanelWidth() - InnerWidth) / 2;
    }

    protected override void Render(ICanvas canvas)
    {
        _buttons?.Render(canvas);
    }
}
