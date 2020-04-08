using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    internal class TrainRenderer : ITrainRenderer //, IDisposable
    {
        private readonly ITrackParameters _trackParameters;
        private readonly ITrainParameters _trainParameters;
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

        public TrainRenderer(ITrackParameters trackParameters, ITrainParameters trainParameters)
        {
            _trackParameters = trackParameters;
            _trainParameters = trainParameters;
        }

        //public void Dispose()
        //{
        //    _bodyPaint.Dispose();
        //    _headPaint.Dispose();
        //}

        public void Render(ICanvas canvas, Train train)
        {
            SetupCanvasToDrawTrain(canvas, train, _trackParameters);

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

        public static void SetupCanvasToDrawTrain(ICanvas canvas, IMovable train, ITrackParameters trackParameters)
        {
            float x = trackParameters.CellSize * train.RelativeLeft;
            float y = trackParameters.CellSize * train.RelativeTop;
            canvas.Translate(x, y);
            canvas.RotateDegrees(train.Angle);
        }
    }
}
