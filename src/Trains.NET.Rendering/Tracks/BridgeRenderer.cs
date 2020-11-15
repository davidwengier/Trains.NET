using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(90)]
    public class BridgeRenderer : SpecializedEntityRenderer<Bridge, Track>
    {
        private const int CanvasSize = 100;
        private const float RailingInset = SupportTopInset + SupportHeight;
        private const float RailingBaseWidth = 6;
        private const float BridgeInset = RailingInset + RailingBaseWidth;
        private const float BridgeWidth = CanvasSize - BridgeInset * 2;
        private const float SupportWidth = 22;
        private const float SupportHeight = 6;
        private const float SupportTopInset = 2;
        private const float SupportLeftPosition = (CanvasSize - SupportWidth) / 2.0f;
        private const float WaterWashWidth = 6 + SupportWidth + 2 * SupportTopInset;
        private const float WaterWashHeight = SupportHeight + SupportTopInset;
        private const float WaterWashLeftPosition = (CanvasSize - WaterWashWidth) / 2.0f;

        private readonly SingleTrackRenderer _trackRenderer;
        private readonly IPath _cornerPlankPath;
        private readonly IPath _cornerRailPath;
        private static readonly PaintBrush s_waterWashPaint = new()
        {
            Color = new Color("#D1EEEE"),
            Style = PaintStyle.Fill,
            IsAntialias = true
        };
        private static readonly PaintBrush s_darkBrownWood = new()
        {
            Color = new Color("#5F380F"),
            Style = PaintStyle.Fill,
            IsAntialias = true
        };
        private static readonly PaintBrush s_plankWood = new()
        {
            Color = new Color("#C29A69"),
            Style = PaintStyle.Fill,
            IsAntialias = true
        };

        public BridgeRenderer(IPathFactory pathFactory, SingleTrackRenderer trackRenderer)
        {
            _trackRenderer = trackRenderer;
            _cornerPlankPath = BuildCornerPlankPath(pathFactory);
            _cornerRailPath = BuildCornerRailPath(pathFactory);
        }

        protected override void Render(ICanvas canvas, Bridge track)
        {
            using (canvas.Scope())
            {
                canvas.RotateDegrees(GetDirectionRotation(track.Direction), CanvasSize / 2, CanvasSize / 2);
                switch (track.Direction)
                {
                    case SingleTrackDirection.Horizontal:
                    case SingleTrackDirection.Vertical:
                        DrawHorizontalBridge(canvas);
                        break;
                    case SingleTrackDirection.LeftDown:
                    case SingleTrackDirection.LeftUp:
                    case SingleTrackDirection.RightDown:
                    case SingleTrackDirection.RightUp:
                        DrawCornerBridge(canvas);
                        break;
                }
            }
            using (canvas.Scope())
            {
                _trackRenderer.DrawSingleTrack(canvas, track);
            }
        }

        private static void DrawHorizontalBridge(ICanvas canvas)
        {
            // Drawn from the perspective of Horizontal left-to-right
            DrawHorizontalSupports(canvas);
            DrawHorizontalRails(canvas);
            DrawHorizontalPlanks(canvas);
        }
        private void DrawCornerBridge(ICanvas canvas)
        {
            DrawCornerSupports(canvas);
            DrawCornerRails(canvas);
            DrawCornerPlanks(canvas);
        }

        private static IPath BuildCornerRailPath(IPathFactory pathFactory)
        {
            IPath path = pathFactory.Create();

            // Inner

            float innerRailA = RailingInset;
            float innerRailB = RailingInset + RailingBaseWidth;

            path.MoveTo(0, innerRailA);
            path.LineTo(0, innerRailB);
            path.ConicTo(innerRailB, innerRailB, innerRailB, 0, 0.75f);
            path.LineTo(innerRailA, 0);
            path.ConicTo(innerRailA, innerRailA, 0, innerRailA, 0.75f);

            // Outer

            float outerRailA = CanvasSize - RailingInset - RailingBaseWidth;
            float outerRailB = CanvasSize - RailingInset;

            path.MoveTo(0, outerRailA);
            path.LineTo(0, outerRailB);
            path.ConicTo(outerRailB, outerRailB, outerRailB, 0, 0.75f);
            path.LineTo(outerRailA, 0);
            path.ConicTo(outerRailA, outerRailA, 0, outerRailA, 0.75f);

            return path;
        }

        private static IPath BuildCornerPlankPath(IPathFactory pathFactory)
        {
            IPath path = pathFactory.Create();

            path.MoveTo(0, BridgeInset);
            path.LineTo(0, CanvasSize - BridgeInset);
            path.ConicTo(CanvasSize - BridgeInset, CanvasSize - BridgeInset, CanvasSize - BridgeInset, 0, 0.75f);
            path.LineTo(BridgeInset, 0);
            path.ConicTo(BridgeInset, BridgeInset, 0, BridgeInset, 0.75f);

            return path;
        }

        private static int GetDirectionRotation(SingleTrackDirection direction) => direction switch
        {
            SingleTrackDirection.Undefined => 0,
            SingleTrackDirection.Horizontal => 0,
            SingleTrackDirection.Vertical => 90,

            SingleTrackDirection.LeftUp => 0,
            SingleTrackDirection.RightUp => 90,
            SingleTrackDirection.RightDown => 180,
            SingleTrackDirection.LeftDown => 270,

            _ => 0,
        };

        private static void DrawHorizontalSupports(ICanvas canvas)
        {
            // Support 1
            canvas.DrawRect(WaterWashLeftPosition, 0, WaterWashWidth, WaterWashHeight, s_waterWashPaint);
            canvas.DrawRect(SupportLeftPosition, SupportTopInset, SupportWidth, SupportHeight, s_darkBrownWood);

            // Support 2
            canvas.DrawRect(WaterWashLeftPosition, CanvasSize - WaterWashHeight, WaterWashWidth, WaterWashHeight, s_waterWashPaint);
            canvas.DrawRect(SupportLeftPosition, CanvasSize - SupportHeight - SupportTopInset, SupportWidth, SupportHeight, s_darkBrownWood);
        }

        private static void DrawCornerSupports(ICanvas canvas)
        {
            using (canvas.Scope())
            {
                canvas.RotateDegrees(-25, 0, 0);
                canvas.DrawRect(-WaterWashWidth / 2.0f, CanvasSize - WaterWashHeight, WaterWashWidth, WaterWashHeight, s_waterWashPaint);
                canvas.DrawRect(-SupportWidth / 2.0f, CanvasSize - SupportHeight - SupportTopInset, SupportWidth, SupportHeight, s_darkBrownWood);

                canvas.RotateDegrees(-40, 0, 0);
                canvas.DrawRect(-WaterWashWidth / 2.0f, CanvasSize - WaterWashHeight, WaterWashWidth, WaterWashHeight, s_waterWashPaint);
                canvas.DrawRect(-SupportWidth / 2.0f, CanvasSize - SupportHeight - SupportTopInset, SupportWidth, SupportHeight, s_darkBrownWood);
            }
        }

        private static void DrawHorizontalRails(ICanvas canvas, float width = CanvasSize)
        {
            // Railing base 1
            canvas.DrawRect(0, RailingInset, width, RailingBaseWidth, s_darkBrownWood);

            // Railing base 2
            canvas.DrawRect(0, CanvasSize - RailingInset - RailingBaseWidth, width, RailingBaseWidth, s_darkBrownWood);
        }

        private void DrawCornerRails(ICanvas canvas)
        {
            canvas.DrawPath(_cornerRailPath, s_darkBrownWood);
        }

        private static void DrawHorizontalPlanks(ICanvas canvas, float width = CanvasSize)
        {
            // Bridge Planks
            canvas.DrawRect(0, BridgeInset, width, BridgeWidth, s_plankWood);
        }

        private void DrawCornerPlanks(ICanvas canvas)
        {
            canvas.DrawPath(_cornerPlankPath, s_plankWood);
        }
    }
}
