using Trains.NET.Engine;
using Trains.NET.Rendering.Trains;

namespace Trains.NET.Rendering
{
    public class CarriageRenderer
    {
        private readonly float _carriageWidth;
        private readonly ITrainParameters _trainParameters;
        private readonly ITrainPainter _trainPainter;

        public CarriageRenderer(ITrainParameters trainParameters, ITrainPainter trainPainter)
        {
            _trainParameters = trainParameters;
            _trainPainter = trainPainter;

            _carriageWidth = _trainParameters.HeadWidth + _trainParameters.RearWidth;
        }

        public void Render(ICanvas canvas, Train carriage)
        {
            TrainRenderer.SetupCanvasToDrawTrain(canvas, carriage);

            TrainPalette? palette = _trainPainter.GetPalette(carriage);

            var outline = new PaintBrush
            {
                Color = palette.OutlineColor,
                Style = PaintStyle.Stroke,
                StrokeWidth = _trainParameters.StrokeWidth
            };

            float startPos = -(_trainParameters.HeadWidth / 2);

            canvas.DrawGradientRect(startPos,
                            -(_trainParameters.HeadHeight / 2),
                            _carriageWidth,
                            _trainParameters.HeadHeight,
                            palette.FrontSectionStartColor, palette.FrontSectionEndColor);

            canvas.DrawRect(startPos,
                            -(_trainParameters.HeadHeight / 2),
                            _carriageWidth,
                            _trainParameters.HeadHeight,
                            outline);
        }
    }
}
