using Trains.NET.Engine;
using Trains.NET.Rendering.Trains;

namespace Trains.NET.Rendering
{
    public class TrainRenderer : IRenderer<Train>
    {
        private readonly ITrainParameters _trainParameters;
        private readonly ITrainPainter _trainPainter;
        private readonly ITrainManager _trainManager;

        public TrainRenderer(ITrainParameters trainParameters, ITrainPainter trainPainter, ITrainManager trainManager)
        {
            _trainParameters = trainParameters;
            _trainPainter = trainPainter;
            _trainManager = trainManager;
        }

        public void Render(ICanvas canvas, Train train)
        {
            TrainPalette? palette = _trainPainter.GetPalette(train);

            SetupCanvasToDrawTrain(canvas, train);

            var outline = new PaintBrush
            {
                Color = palette.OutlineColor,
                Style = PaintStyle.Stroke,
                StrokeWidth = _trainParameters.StrokeWidth
            };

            if (_trainManager.CurrentTrain == train)
            {
                outline = outline with { Color = Colors.LightGray };
            }

            var smokeStack = new PaintBrush
            {
                Color = palette.OutlineColor,
                Style = PaintStyle.Fill,
                StrokeWidth = _trainParameters.StrokeWidth
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

            canvas.DrawCircle(startPos + _trainParameters.RearWidth + _trainParameters.HeadWidth - _trainParameters.SmokeStackOffset, 0, _trainParameters.SmokeStackRadius, smokeStack);

        }

        public static void SetupCanvasToDrawTrain(ICanvas canvas, IMovable train)
        {
            float x = 100 * train.RelativeLeft;
            float y = 100 * train.RelativeTop;
            canvas.Translate(x, y);
            canvas.RotateDegrees(train.Angle);
        }
    }
}
