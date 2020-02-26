using System;
using SkiaSharp;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    internal class TrainRenderer : ITrainRenderer, IDisposable
    {
        private readonly ITrackParameters _trackParameters;
        private readonly ITrainParameters _trainParameters;
        private readonly SKPaint _bodyPaint = new SKPaint
        {
            Color = SKColors.Red,
            Style = SKPaintStyle.Fill
        };
        private readonly SKPaint _headPaint = new SKPaint
        {
            Color = SKColors.Blue,
            Style = SKPaintStyle.Fill
        };

        public TrainRenderer(ITrackParameters trackParameters, ITrainParameters trainParameters)
        {
            _trackParameters = trackParameters;
            _trainParameters = trainParameters;
        }

        public void Dispose()
        {
            _bodyPaint.Dispose();
            _headPaint.Dispose();
        }

        public void Render(SKCanvas canvas, Train train)
        {
            float x = _trackParameters.CellSize * train.RelativeLeft;
            float y = _trackParameters.CellSize * train.RelativeTop;
            canvas.Translate(x, y);
            canvas.RotateDegrees(train.Angle);
            canvas.DrawRect(-_trainParameters.TrainBodyWidth,
                            -(_trainParameters.TrainBodyWidth / 2),
                            _trainParameters.TrainBodyWidth * 2,
                            _trainParameters.TrainBodyWidth,
                            _bodyPaint);
            canvas.DrawRect(_trainParameters.TrainBodyWidth,
                            -(_trainParameters.TrainHeadWidth / 2),
                            _trainParameters.TrainHeadWidth,
                            _trainParameters.TrainHeadWidth,
                            _headPaint);
        }
    }
}
