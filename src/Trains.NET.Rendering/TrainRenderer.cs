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
            canvas.DrawRect((_trackParameters.CellSize / 2) - _trainParameters.TrainBodyWidth, (_trackParameters.CellSize / 2) - (_trainParameters.TrainBodyWidth / 2), _trainParameters.TrainBodyWidth * 2, _trainParameters.TrainBodyWidth, _bodyPaint);
            canvas.DrawRect((_trackParameters.CellSize / 2) + _trainParameters.TrainBodyWidth, (_trackParameters.CellSize / 2) - (_trainParameters.TrainHeadWidth / 2), _trainParameters.TrainHeadWidth, _trainParameters.TrainHeadWidth, _headPaint);
        }
    }
}
