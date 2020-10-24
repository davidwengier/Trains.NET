using Trains.NET.Engine;

namespace Trains.NET.Rendering.Logic
{
    [Order(90)]
    public class SignalRenderer : SpecializedEntityRenderer<Signal, Track>
    {
        private const int CanvasSize = 100;
        private const int SignalWidth = 10;
        private const int SignalHeight = 16;
        private const int SignalLightHousingWidth = 5;
        private const int SignalLightHousingHeight = 8;

        private const int LightLength = 20;
        private const int LightCurveLength = LightLength + 6;
        private const int LightSpread = 20;

        private static readonly PaintBrush s_signalBodyPaint = new()
        {
            Style = PaintStyle.Fill,
            Color = Colors.VeryDarkGray
        };
        private static readonly PaintBrush s_signalLightRed = new()
        {
            Style = PaintStyle.Fill,
            Color = new Color(128, 255, 0, 0)
        };
        private static readonly PaintBrush s_signalLightAmber = new()
        {
            Style = PaintStyle.Fill,
            Color = new Color(128, 255, 191, 0)
        };
        private static readonly PaintBrush s_signalLightGreen = new()
        {
            Style = PaintStyle.Fill,
            Color = new Color(128, 0, 255, 0)
        };

        private readonly IPath _lightPath;
        private readonly TrackRenderer _trackRenderer;

        public SignalRenderer(IPathFactory pathFactory, TrackRenderer trackRenderer)
        {
            _lightPath = BuildLightPath(pathFactory);
            _trackRenderer = trackRenderer;
        }

        private static IPath BuildLightPath(IPathFactory pathFactory)
        {
            const int halfLightSpread = LightSpread / 2;

            IPath path = pathFactory.Create();

            path.LineTo(-LightLength, -halfLightSpread);
            path.ConicTo(-LightCurveLength, 0, -LightLength, halfLightSpread, 0.75f);
            path.LineTo(0, 0);

            return path;
        }

        protected override void Render(ICanvas canvas, Signal item)
        {
            using (canvas.Scope())
            {
                _trackRenderer.Render(canvas, item);
            }

            var signalState = item.SignalState;
            var direction = item.Direction;
            using (canvas.Scope())
            {
                if (direction == TrackDirection.Vertical)
                {
                    canvas.RotateDegrees(90, CanvasSize / 2, CanvasSize / 2);
                }

                canvas.Translate(CanvasSize / 2, 0);

                DrawSignal(canvas, signalState);

                canvas.Translate(0, CanvasSize);
                canvas.RotateDegrees(180);

                DrawSignal(canvas, signalState);
            }
        }

        private void DrawSignal(ICanvas canvas, SignalState state)
        {
            const int HousingY = SignalHeight / 2 - SignalLightHousingHeight / 2;
            using (canvas.Scope())
            {
                canvas.Translate(-2 * SignalLightHousingWidth, 0);

                // Light
                using (canvas.Scope())
                {
                    canvas.Translate(SignalLightHousingWidth, SignalHeight / 2);

                    var signalColor = state switch
                    {
                        SignalState.Stop => s_signalLightRed,
                        SignalState.TemporaryStop => s_signalLightAmber,
                        _ => s_signalLightGreen
                    };

                    canvas.DrawPath(_lightPath, signalColor);
                }

                // Housing
                canvas.DrawRect(0, HousingY, SignalLightHousingWidth, SignalLightHousingHeight, s_signalBodyPaint);

                // Body
                canvas.DrawRect(SignalLightHousingWidth, 0, SignalWidth, SignalHeight, s_signalBodyPaint);
            }
        }
    }
}
