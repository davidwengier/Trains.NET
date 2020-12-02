using System;
using Trains.NET.Engine;
using Trains.NET.Rendering.Trains;

namespace Trains.NET.Rendering.UI
{
    [Order(20)]
    public class TrainInfoScreen : PanelBase
    {
        private const int TrainDisplayAreaWidth = 50;
        private const int PanelWidth = 280 + TrainDisplayAreaWidth;

        private readonly ITrainManager _trainManager;
        private readonly IGameManager _gameManager;
        private readonly IGameBoard _gameBoard;
        private readonly ITrainParameters _trainParameters;
        private readonly ITrainPainter _trainPainter;
        private readonly MultiButton _controlButton;
        private readonly MultiButton _actionButton;
        private readonly MultiButton _trainSelectionButton;

        protected override PanelPosition Position => PanelPosition.Floating;
        protected override int Left => (PanelWidth + 75) * -1;
        protected override int Top => 50;
        protected override int InnerHeight => 30;
        protected override int InnerWidth => PanelWidth;
        protected override bool CanClose => true;
        protected override string? Title => "Info";

        public TrainInfoScreen(ITrainManager trainManager, IGameManager gameManager, IGameBoard gameBoard, ITrainParameters trainParameters, ITrainPainter trainPainter)
        {
            _trainManager = trainManager;
            _gameManager = gameManager;
            _gameBoard = gameBoard;
            _trainParameters = trainParameters;
            _trainPainter = trainPainter;
            _trainManager.Changed += (s, e) =>
            {
                this.Visible = _trainManager.CurrentTrain is not null;
                OnChanged();
            };
            _gameManager.Changed += (s, e) => OnChanged();
            _trainManager.CurrentTrainPropertyChanged += (s, e) => OnChanged();

            _controlButton = new MultiButton(20, new ButtonBase[]
                {
                    CreateButton(Picture.Backward, () => false, () => _trainManager.CurrentTrain?.Slower()),
                    CreateButton(Picture.Play, () => _trainManager.CurrentTrain?.Stopped != true, () => _trainManager.CurrentTrain?.Start()),
                    CreateButton(Picture.Pause, () => _trainManager.CurrentTrain?.Stopped == true, () => _trainManager.CurrentTrain?.Stop()),
                    CreateButton(Picture.Forward, () => false, () => _trainManager.CurrentTrain?.Faster()),
                });

            _actionButton = new MultiButton(20, new ButtonBase[]
                {
                    CreateButton(Picture.Plus, () => false, () =>
                    {
                        if (_trainManager.CurrentTrain is not null)
                        {
                            _gameBoard.AddCarriageToTrain(_trainManager.CurrentTrain);
                        }
                    }),
                    CreateButton(Picture.Minus, () => false, () => _trainManager.CurrentTrain?.RemoveCarriage()),
                    CreateButton(Picture.Eye, () => _trainManager.CurrentTrain?.Follow ?? false, () => _trainManager.ToggleFollow(_trainManager.CurrentTrain!)),
                    CreateButton(Picture.Trash, () => false, () =>
                    {
                        _gameBoard.RemoveMovable(_trainManager.CurrentTrain!);
                        Close();
                    }),
                });

            _trainSelectionButton = new MultiButton(20, new ButtonBase[]
                {
                    CreateButton(Picture.Left, () => false, () => _trainManager.PreviousTrain()),
                    CreateButton(Picture.Right, () => false, () => _trainManager.NextTrain())
                });

            this.Visible = _trainManager.CurrentTrain is not null;
        }

        private static ButtonBase CreateButton(Picture picture, Func<bool> isActive, Action onClick)
            => new PictureButton(picture, 16, isActive, onClick)
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

            y += 40;
            _trainSelectionButton.HandleMouseAction(x, y, action);

            return true;
        }

        protected override void Render(ICanvas canvas)
        {
            Train train = _trainManager.CurrentTrain ?? throw new NullReferenceException("Current train is null so we shouldn't be rendering");

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

            canvas.Translate(0, -40);
            _trainSelectionButton.Render(canvas);
        }
    }
}
