using System;
using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    internal class TrainRenderer : ITrainRenderer //, IDisposable
    {
        private readonly ITrackParameters _trackParameters;
        private readonly ITrainParameters _trainParameters;
        private readonly OrderedList<ITrainPalette> _trainPalettes;
        private readonly Random _random = new Random();

        private readonly Dictionary<Train, ITrainPalette> _paletteMap = new Dictionary<Train, ITrainPalette>();

        private readonly PaintBrush _bodyPaint = new PaintBrush
        {
            Color = Colors.Red,
            Style = PaintStyle.Fill
        };
        private readonly PaintBrush _headPaint = new PaintBrush
        {
            Color = Colors.Blue,
            Style = PaintStyle.Fill
        };

        public TrainRenderer(ITrackParameters trackParameters, ITrainParameters trainParameters, OrderedList<ITrainPalette> trainPalettes)
        {
            _trackParameters = trackParameters;
            _trainParameters = trainParameters;
            _trainPalettes = trainPalettes;
        }

        //public void Dispose()
        //{
        //    _bodyPaint.Dispose();
        //    _headPaint.Dispose();
        //}

        public void Render(ICanvas canvas, Train train)
        {
            if (!_paletteMap.ContainsKey(train))
            {
                _paletteMap.Add(train, GetRandomPalette());
            }
            var palette = _paletteMap[train];

            SetupCanvasToDrawTrain(canvas, train, _trackParameters);

            var outline = new PaintBrush
            {
                Color = palette.OutlineColor,
                Style = PaintStyle.Stroke,
                StrokeWidth = 2
            };

            var smokeStack = new PaintBrush
            {
                Color = palette.OutlineColor,
                Style = PaintStyle.Fill,
                StrokeWidth = 2
            };

            float startPos = -((_trainParameters.HeadWidth + _trainParameters.RearWidth) / 2);

            canvas.GradientRect(startPos,
                            -(_trainParameters.RearHeight / 2),
                            _trainParameters.RearWidth,
                            _trainParameters.RearHeight,

                            palette.RearSectionStartColor, palette.RearSectionEndColor);

            canvas.GradientRect(startPos + _trainParameters.RearWidth,
                            -(_trainParameters.HeadHeight / 2),
                            _trainParameters.HeadWidth,
                            _trainParameters.HeadHeight,

                            palette.FrontSectionStartColor, palette.FrontSectionEndColor);

            canvas.DrawRect(startPos,
                            -(_trainParameters.RearHeight / 2),
                            _trainParameters.RearWidth,
                            _trainParameters.RearHeight,
                            outline);

            canvas.DrawRect(startPos + _trainParameters.RearWidth,
                            -(_trainParameters.HeadHeight / 2),
                            _trainParameters.HeadWidth,
                            _trainParameters.HeadHeight,
                            outline);

            canvas.DrawCircle(startPos + _trainParameters.RearWidth + _trainParameters.HeadWidth - 5, 0, 2, smokeStack);

        }

        private ITrainPalette GetRandomPalette()
        {
            return _trainPalettes[_random.Next(0, _trainPalettes.Count)];
        }

        public static void SetupCanvasToDrawTrain(ICanvas canvas, IMovable train, ITrackParameters trackParameters)
        {
            float x = trackParameters.CellSize * train.RelativeLeft;
            float y = trackParameters.CellSize * train.RelativeTop;
            canvas.Translate(x, y);
            canvas.RotateDegrees(train.Angle);
        }
    }
}
