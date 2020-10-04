using System;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    [Order(20)]
    public class TrainInfoScreen : IScreen, IInteractionHandler
    {
        private readonly ITrainManager _trainManager;
        private readonly IGameManager _gameManager;
        private readonly IGameBoard _gameBoard;

        public event EventHandler? Changed;

        public TrainInfoScreen(ITrainManager trainManager, IGameManager gameManager, IGameBoard gameBoard)
        {
            _trainManager = trainManager;
            _gameManager = gameManager;
            _gameBoard = gameBoard;
            _trainManager.Changed += (s, e) => Changed?.Invoke(s, e);
            _gameManager.Changed += (s, e) => Changed?.Invoke(s, e);
            _trainManager.CurrentTrainPropertyChanged += (s, e) => Changed?.Invoke(this, EventArgs.Empty);
        }

        public bool HandlePointerAction(int x, int y, int width, int height, PointerAction action)
        {
            if (action != Rendering.PointerAction.Click)
            {
                return false;
            }

            if (_trainManager.CurrentTrain == null)
            {
                return false;
            }

            const int PanelWidth = 250;

            Train train = _trainManager.CurrentTrain;

            if (x >= width - (PanelWidth + 50) - 5 && x <= width - 50 && y >= 50 - 5 && y <= 100)
            {
                x -= width - (PanelWidth + 50);
                y -= 50;

                if (x < 5 && y < 5)
                {
                    _trainManager.CurrentTrain = null;
                }
                else if (x < 0 || y < 0)
                {
                    // filter out the thin border that isn't the X button, but isn't the panel either
                    return false;
                }

                x -= 10;

                // text is drawn from the baseline, but easier to track from the top
                y -= 20 - Brushes.Label.TextSize.GetValueOrDefault();
                if (y >= 0 && y <= 20 && x >= PanelWidth - 40 && x <= PanelWidth - 20)
                {
                    _trainManager.ToggleFollow(train);
                }

                y -= 20;
                if (y >= 0 && y <= 20)
                {
                    if (x is >= PanelWidth - 40)
                    {
                        _trainManager.CurrentTrain = null;
                        _gameBoard.RemoveMovable(train);
                    }
                    else if (_gameManager.BuildMode)
                    {
                        return true;
                    }
                    else if (x is >= 0 and < 18)
                    {
                        train.Slower();
                    }
                    else if (x is >= 20 and < 38)
                    {
                        train.Start();
                        Changed?.Invoke(this, EventArgs.Empty);
                    }
                    else if (x is >= 40 and < 58)
                    {
                        train.Stop();
                        Changed?.Invoke(this, EventArgs.Empty);
                    }
                    else if (x is >= 60 and < 78)
                    {
                        train.Faster();
                    }
                }
                return true;
            }
            return false;
        }

        public void Render(ICanvas canvas, int width, int height)
        {
            const int PanelWidth = 250;

            if (_trainManager.CurrentTrain == null)
            {
                return;
            }

            Train train = _trainManager.CurrentTrain;

            canvas.Translate(width - (PanelWidth + 50), 50);

            canvas.DrawRoundRect(0, 0, PanelWidth, 50, 10, 10, Brushes.PanelBorder);
            canvas.DrawRoundRect(0, 0, PanelWidth, 50, 10, 10, Brushes.PanelBackground);

            using (var _ = canvas.Scope())
            {
                canvas.Translate(2, 2);

                canvas.DrawCircle(0, 0, 7, Brushes.PanelBorder with { StrokeWidth = 1 });
                canvas.DrawCircle(0, 0, 7, Brushes.PanelBackground);
                canvas.DrawText("x", -4, 5, Brushes.Label);
            }

            canvas.Translate(10, 20);

            canvas.DrawText(train.Name, 0, 0, Brushes.Label);
            canvas.DrawText("{{fa-eye}}", PanelWidth - 40, 0, train.Follow ? Brushes.Active : Brushes.Label);

            canvas.Translate(0, 20);
            var brush = _gameManager.BuildMode ? Brushes.Disabled : Brushes.Label;
            canvas.DrawText("{{fa-backward}}", 0, 0, brush);
            canvas.DrawText("{{fa-forward}}", 60, 0, brush);
            canvas.DrawText($"{train.CurrentSpeed:0} km/h", 90, 0, brush);

            brush = _gameManager.BuildMode || !train.Stopped ? Brushes.Disabled : Brushes.Label;
            canvas.DrawText("{{fa-play}}", 20, 0, brush);
            brush = _gameManager.BuildMode || train.Stopped ? Brushes.Disabled : Brushes.Label;
            canvas.DrawText("{{fa-pause}}", 40, 0, brush);

            canvas.DrawText("{{fa-trash}}", PanelWidth - 40, 0, Brushes.Label);
        }
    }
}
