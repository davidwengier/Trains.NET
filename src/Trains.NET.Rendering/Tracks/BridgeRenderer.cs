using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    [Order(90)]
    public class BridgeRenderer : ICachableRenderer<Track>
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

        private readonly ITerrainMap _terrainMap;
        private readonly IPath _cornerPlankPath;
        private readonly IPath _cornerRailPath;
        private static readonly PaintBrush s_waterWashPaint = new PaintBrush
        {
            Color = new Color("#D1EEEE"),
            Style = PaintStyle.Fill,
            IsAntialias = true
        };
        private static readonly PaintBrush s_darkBrownWood = new PaintBrush
        {
            Color = new Color("#5F380F"),
            Style = PaintStyle.Fill,
            IsAntialias = true
        };
        private static readonly PaintBrush s_plankWood = new PaintBrush
        {
            Color = new Color("#C29A69"),
            Style = PaintStyle.Fill,
            IsAntialias = true
        };

        public BridgeRenderer(ITerrainMap terrainMap, IPathFactory pathFactory)
        {
            _terrainMap = terrainMap;
            _cornerPlankPath = BuildCornerPlankPath(pathFactory);
            _cornerRailPath = BuildCornerRailPath(pathFactory);
        }

        public bool ShouldRender(Track track)
            => _terrainMap.Get(track.Column, track.Row).IsWater;

        public string GetCacheKey(Track track)
            => track.Direction.ToString();

        public void Render(ICanvas canvas, Track track)
        {
            using (canvas.Scope())
            {
                canvas.RotateDegrees(GetDirectionRotation(track.Direction), CanvasSize / 2, CanvasSize / 2);
                switch (track.Direction)
                {
                    case TrackDirection.Horizontal:
                    case TrackDirection.Vertical:
                        DrawHorizontalBridge(canvas);
                        break;
                    case TrackDirection.Cross:
                        DrawCrossBridge(canvas);
                        break;
                    case TrackDirection.LeftDown:
                    case TrackDirection.LeftUp:
                    case TrackDirection.RightDown:
                    case TrackDirection.RightUp:
                        DrawCornerBridge(canvas);
                        break;
                    case TrackDirection.RightUp_RightDown:
                    case TrackDirection.RightDown_LeftDown:
                    case TrackDirection.LeftDown_LeftUp:
                    case TrackDirection.LeftUp_RightUp:
                        DrawIntersectionBridge(canvas);
                        break;
                }
            }
        }

        private static void DrawHorizontalBridge(ICanvas canvas)
        {
            // Drawn from the perspective of Horizontal left-to-right
            DrawHorizontalSupports(canvas);
            DrawHorizontalRails(canvas);
            DrawHorizontalPlanks(canvas);
        }
        private static void DrawCrossBridge(ICanvas canvas)
        {
            DrawHorizontalRails(canvas);
            using (canvas.Scope())
            {
                canvas.RotateDegrees(90, CanvasSize / 2, CanvasSize / 2);
                DrawHorizontalRails(canvas);
                DrawHorizontalPlanks(canvas);
            }
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
        private static void DrawIntersectionBridge(ICanvas canvas)
        {
            // Drawn from the perspective of a LeftUp_RightUp intersection
            DrawHorizontalSupports(canvas);
            DrawHorizontalRails(canvas);
            using (canvas.Scope())
            {
                canvas.RotateDegrees(90, CanvasSize / 2, CanvasSize / 2);
                DrawHorizontalSupports(canvas);
                DrawHorizontalRails(canvas, CanvasSize - RailingInset);
                DrawHorizontalPlanks(canvas, CanvasSize - BridgeInset);
            }
            DrawHorizontalPlanks(canvas);
        }
        private static int GetDirectionRotation(TrackDirection direction) => direction switch
        {
            TrackDirection.Undefined => 0,
            TrackDirection.Horizontal => 0,
            TrackDirection.Cross => 0,
            TrackDirection.Vertical => 90,

            TrackDirection.LeftUp => 0,
            TrackDirection.RightUp => 90,
            TrackDirection.RightDown => 180,
            TrackDirection.LeftDown => 270,

            TrackDirection.LeftUp_RightUp => 0,
            TrackDirection.RightUp_RightDown => 90,
            TrackDirection.RightDown_LeftDown => 180,
            TrackDirection.LeftDown_LeftUp => 270,

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
