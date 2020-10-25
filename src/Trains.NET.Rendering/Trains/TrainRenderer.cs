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

        public bool ShouldRender(Train train) => true;

        public void Render(ICanvas canvas, Train train)
        {
            SetupCanvasToDrawTrain(canvas, train);
            var shouldHighlight = _trainManager.CurrentTrain == train;
            TrainPalette? palette = _trainPainter.GetPalette(train);

            RenderTrain(canvas, palette, _trainParameters, shouldHighlight);
        }

        public static void RenderTrain(ICanvas canvas, TrainPalette palette, ITrainParameters trainParameters, bool shouldHighlight)
        {
            var outline = new PaintBrush
            {
                Color = shouldHighlight ? Colors.LightGray : palette.OutlineColor,
                Style = PaintStyle.Stroke,
                StrokeWidth = trainParameters.StrokeWidth
            };

            var smokeStack = new PaintBrush
            {
                Color = palette.OutlineColor,
                Style = PaintStyle.Fill,
                StrokeWidth = trainParameters.StrokeWidth
            };

            float startPos = -((trainParameters.HeadWidth + trainParameters.RearWidth) / 2);

            canvas.DrawGradientRect(startPos,
                            -(trainParameters.RearHeight / 2),
                            trainParameters.RearWidth,
                            trainParameters.RearHeight,

                            palette.RearSectionStartColor, palette.RearSectionEndColor);

            canvas.DrawGradientRect(startPos + trainParameters.RearWidth,
                            -(trainParameters.HeadHeight / 2),
                            trainParameters.HeadWidth,
                            trainParameters.HeadHeight,

                            palette.FrontSectionStartColor, palette.FrontSectionEndColor);

            canvas.DrawRect(startPos,
                            -(trainParameters.RearHeight / 2),
                            trainParameters.RearWidth,
                            trainParameters.RearHeight,
                            outline);

            canvas.DrawRect(startPos + trainParameters.RearWidth,
                            -(trainParameters.HeadHeight / 2),
                            trainParameters.HeadWidth,
                            trainParameters.HeadHeight,
                            outline);

            canvas.DrawCircle(startPos + trainParameters.RearWidth + trainParameters.HeadWidth - trainParameters.SmokeStackOffset, 0, trainParameters.SmokeStackRadius, smokeStack);
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
