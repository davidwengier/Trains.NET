using Trains.NET.Engine;
using Trains.NET.Rendering.Trains;

namespace Trains.NET.Rendering
{
    public class CarriageRenderer
    {
        private const float CarriageWidth = 80f;
        private readonly ITrainParameters _trainParameters;
        private readonly ITrainPainter _trainPainter;

        public CarriageRenderer(ITrainParameters trainParameters, ITrainPainter trainPainter)
        {
            _trainParameters = trainParameters;
            _trainPainter = trainPainter;
        }

        public void Render(ICanvas canvas, Train carriage, Train owner)
        {
            TrainRenderer.SetupCanvasToDrawTrain(canvas, carriage);

            TrainPalette? palette = _trainPainter.GetPalette(owner);

            var outline = new PaintBrush
            {
                Color = palette.OutlineColor,
                Style = PaintStyle.Stroke,
                StrokeWidth = _trainParameters.StrokeWidth
            };

            float startPos = -(_trainParameters.HeadWidth / 2);

            canvas.DrawGradientRect(startPos,
                            -(_trainParameters.HeadHeight / 2),
                            CarriageWidth,
                            _trainParameters.HeadHeight,
                            palette.FrontSectionStartColor, palette.FrontSectionEndColor);

            canvas.DrawRect(startPos,
                            -(_trainParameters.HeadHeight / 2),
                            CarriageWidth,
                            _trainParameters.HeadHeight,
                            outline);
        }
    }
}
