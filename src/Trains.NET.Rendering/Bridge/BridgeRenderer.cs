using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public class BridgeRenderer : IBridgeRenderer
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
        private static readonly PaintBrush s_waterWashPaint = new PaintBrush
        {
            Color = new Color("#D1EEEE"),
            Style = PaintStyle.Fill
        };
        private static readonly PaintBrush s_darkBrownWood = new PaintBrush
        {
            Color = new Color("#5F380F"),
            Style = PaintStyle.Fill
        };
        private static readonly PaintBrush s_plankWood = new PaintBrush
        {
            Color = new Color("#C29A69"),
            Style = PaintStyle.Fill
        };
 
        public void Render(ICanvas canvas, TrackDirection direction)
        {
            canvas.Save();
            canvas.RotateDegrees(GetDirectionRotation(direction), CanvasSize / 2, CanvasSize / 2);
            switch (direction)
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
            canvas.Restore();
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
            canvas.Save();
            canvas.RotateDegrees(90, CanvasSize / 2, CanvasSize / 2);
            DrawHorizontalRails(canvas);
            DrawHorizontalPlanks(canvas);
            canvas.Restore();
            DrawHorizontalPlanks(canvas);
        }
        private static void DrawCornerBridge(ICanvas canvas)
        {
            // Drawn from the perspective of a LeftUp corner
            DrawHorizontalSupports(canvas);
            DrawHorizontalRails(canvas, CanvasSize - RailingInset);
            canvas.Save();
            canvas.RotateDegrees(90, CanvasSize / 2, CanvasSize / 2);
            DrawHorizontalSupports(canvas);
            DrawHorizontalRails(canvas, CanvasSize - RailingInset);
            DrawHorizontalPlanks(canvas, CanvasSize - BridgeInset);
            canvas.Restore();
            DrawHorizontalPlanks(canvas, CanvasSize - BridgeInset);
        }
        private static void DrawIntersectionBridge(ICanvas canvas)
        {
            // Drawn from the perspective of a LeftUp_RightUp intersection
            DrawHorizontalSupports(canvas);
            DrawHorizontalRails(canvas);
            canvas.Save();
            canvas.RotateDegrees(90, CanvasSize / 2, CanvasSize / 2);
            DrawHorizontalSupports(canvas);
            DrawHorizontalRails(canvas, CanvasSize - RailingInset);
            DrawHorizontalPlanks(canvas, CanvasSize - BridgeInset);
            canvas.Restore();
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
        private static void DrawHorizontalRails(ICanvas canvas, float width = CanvasSize)
        {
            // Railing base 1
            canvas.DrawRect(0, RailingInset, width, RailingBaseWidth, s_darkBrownWood);

            // Railing base 2
            canvas.DrawRect(0, CanvasSize - RailingInset - RailingBaseWidth, width, RailingBaseWidth, s_darkBrownWood);
        }
        private static void DrawHorizontalPlanks(ICanvas canvas, float width = CanvasSize)
        {
            // Bridge Planks
            canvas.DrawRect(0, BridgeInset, width, BridgeWidth, s_plankWood);
        }
    }
}
