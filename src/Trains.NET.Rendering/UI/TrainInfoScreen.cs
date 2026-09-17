using Trains.NET.Engine;
using Trains.NET.Rendering.Trains;

namespace Trains.NET.Rendering.UI;

[Order(20)]
public class TrainInfoScreen : PanelBase
{
    private const int TrainDisplayAreaWidth = 50;
    private const int PanelWidth = 280 + TrainDisplayAreaWidth;

    private readonly ITrainManager _trainManager;
    private readonly IGameManager _gameManager;
    private readonly IMovableLayout _movableLayout;
    private readonly ITrainParameters _trainParameters;
    private readonly ITrainPainter _trainPainter;
    private readonly MultiButton _controlButton;
    private readonly MultiButton _actionButton;
    private readonly MultiButton _trainSelectionButton;

    protected override PanelPosition Position => PanelPosition.Right;
    protected override int Left => (PanelWidth + 55) * -1;
    protected override int Top => 50;
    protected override int InnerHeight => 30;
    protected override int InnerWidth => PanelWidth;
    protected override bool CanClose => true;
    protected override string? Title => "Info";

    public TrainInfoScreen(
        ITrainManager trainManager,
        IGameManager gameManager,
        IMovableLayout movableLayout,
        ITrainParameters trainParameters,
        ITrainPainter trainPainter,
        ITooltipService tooltipService)
    {
        _trainManager = trainManager;
        _gameManager = gameManager;
        _movableLayout = movableLayout;
        _trainParameters = trainParameters;
        _trainPainter = trainPainter;
        _trainManager.Changed += (s, e) =>
        {
            Visible = _trainManager.CurrentTrain is not null;
            OnChanged();
        };
        _gameManager.Changed += (s, e) => OnChanged();
        _trainManager.CurrentTrainPropertyChanged += (s, e) => OnChanged();

        _controlButton = new MultiButton(20, new ButtonBase[]
            {
                    CreateButton(Picture.Backward, "Slower", () => false, () => _trainManager.CurrentTrain?.Slower(), tooltipService),
                    CreateButton(Picture.Play, "Start", () => _trainManager.CurrentTrain?.Stopped != true, () => _trainManager.CurrentTrain?.Start(), tooltipService),
                    CreateButton(Picture.Pause, "Stop", () => _trainManager.CurrentTrain?.Stopped == true, () => _trainManager.CurrentTrain?.Stop(), tooltipService),
                    CreateButton(Picture.Forward, "Faster", () => false, () => _trainManager.CurrentTrain?.Faster(), tooltipService),
            });

        _actionButton = new MultiButton(20, new ButtonBase[]
            {
                    CreateButton(Picture.Plus, "Add carriage", () => false, () => _trainManager.CurrentTrain?.AddCarriage(), tooltipService),
                    CreateButton(Picture.Minus, "Remove carriage", () => false, () => _trainManager.CurrentTrain?.RemoveCarriage(), tooltipService),
                    CreateButton(Picture.Eye, "Follow train", () => _trainManager.CurrentTrain?.Follow ?? false, () => _trainManager.ToggleFollow(_trainManager.CurrentTrain!), tooltipService),
                    CreateButton(Picture.Trash, "Delete train", () => false, () =>
                    {
                        _movableLayout.Remove(_trainManager.CurrentTrain!);
                        Close();
                    }, tooltipService),
            });

        _trainSelectionButton = new MultiButton(20, new ButtonBase[]
            {
                    CreateButton(Picture.Left, "Previous train", () => false, () => _trainManager.PreviousTrain(), tooltipService),
                    CreateButton(Picture.Right, "Next train", () => false, () => _trainManager.NextTrain(), tooltipService)
            });

        Visible = _trainManager.CurrentTrain is not null;
    }

    private static ButtonBase CreateButton(
        Picture picture,
        string tooltip,
        Func<bool> isActive,
        Action onClick,
        ITooltipService tooltipService)
        => new PictureButton(picture, 16, isActive, onClick, tooltipService: tooltipService, tooltip: tooltip)
        {
            TransparentBackground = true,
        };

    protected override void Close()
    {
        _trainManager.CurrentTrain = null;
    }

    protected override bool HandlePointerAction(int x, int y, PointerAction action)
    {
        y -= 30;
        if (_controlButton.HandleMouseAction(x, y, action))
        {
            return true;
        }

        x -= PanelWidth - 80;
        if (_actionButton.HandleMouseAction(x, y, action))
        {
            return true;
        }

        x -= 40;
        y += 40;
        _trainSelectionButton.HandleMouseAction(x, y, action);

        return true;
    }

    protected override void Render(ICanvas canvas)
    {
        var train = _trainManager.CurrentTrain ?? throw new NullReferenceException("Current train is null so we shouldn't be rendering");

        using (canvas.Scope())
        {
            canvas.Translate(TrainDisplayAreaWidth / 2, 5);
            canvas.Scale(0.5f, 0.5f);
            var palette = _trainPainter.GetPalette(train);
            TrainRenderer.RenderTrain(canvas, palette, _trainParameters, false);
        }

        using (canvas.Scope())
        {
            canvas.Translate(TrainDisplayAreaWidth + 10, 10);

            canvas.DrawText(train.Name, 0, 0, Brushes.Label);
        }

        canvas.Translate(0, 45);

        var brush = _gameManager.BuildMode ? Brushes.Disabled : Brushes.Label;
        canvas.DrawText($"{train.CurrentSpeed:0} km/h", 90, 0, brush);

        canvas.Translate(0, -15);

        _controlButton.Render(canvas);

        canvas.Translate(PanelWidth - 80, 0);
        _actionButton.Render(canvas);

        canvas.Translate(40, -40);
        _trainSelectionButton.Render(canvas);
    }
}
