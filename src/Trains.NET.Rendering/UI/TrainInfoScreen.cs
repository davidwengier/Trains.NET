using System;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    [Order(20)]
    public class TrainInfoScreen : IScreen
    {
        private readonly PaintBrush _border = new()
        {
            Color = Colors.Black,
            Style = PaintStyle.Stroke,
            StrokeWidth = 3
        };
        private readonly PaintBrush _background = new()
        {
            Color = Colors.White.WithAlpha("aa"),
            Style = PaintStyle.Fill
        };
        private readonly PaintBrush _label = new PaintBrush
        {
            TextSize = 13,
            IsAntialias = true,
            Color = Colors.Black
        };

        private readonly ITrainManager _trainManager;

        public event EventHandler? Changed;

        public TrainInfoScreen(ITrainManager trainManager)
        {
            _trainManager = trainManager;
            _trainManager.Changed += (s, e) => Changed?.Invoke(s, e);
        }

        public bool HandleInteraction(int x, int y, int width, int height, MouseAction action)
        {
            return false;
        }

        public void Render(ICanvas canvas, int width, int height)
        {
            if (_trainManager.CurrentTrain == null)
            {
                return;
            }

            Train train = _trainManager.CurrentTrain;

            canvas.DrawRoundRect(width - 300, 50, 250, 20, 10, 10, _border);
            canvas.DrawRoundRect(width - 300, 50, 250, 20, 10, 10, _background);

            canvas.DrawText(train.Name, width - 280, 65, _label);

            /*
                void Delete();
                void Faster();
                void SetCurrentTrain(Train? train);
                void Slower();
                void Start();
                void Stop();
                void ToggleFollowMode();
            */
        }
    }
}
