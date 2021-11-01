using Trains.NET.Engine;

namespace Trains.NET.Rendering;

public class TIntersectionRenderer : SpecializedEntityRenderer<TIntersection, Track>
{
    private readonly SingleTrackRenderer _trackRenderer;

    public TIntersectionRenderer(SingleTrackRenderer trackRenderer)
    {
        _trackRenderer = trackRenderer;
    }

    protected override void Render(ICanvas canvas, TIntersection track)
    {
        using (canvas.Scope())
        {
            var rotationAngle = track.Direction switch
            {
                TIntersectionDirection.LeftUp_RightUp => 0,
                TIntersectionDirection.RightUp_RightDown => 90,
                TIntersectionDirection.RightDown_LeftDown => 180,
                TIntersectionDirection.LeftDown_LeftUp => 270,
                _ => 0
            };

            canvas.RotateDegrees(rotationAngle, 50.0f, 50.0f);

            if (track.Style is TIntersectionStyle.CornerAndSecondary or TIntersectionStyle.StraightAndPrimary)
            {
                canvas.Scale(-1, 1);
                canvas.Translate(-100.0f, 0);
            }

            if (track.Style is TIntersectionStyle.StraightAndPrimary or TIntersectionStyle.StraightAndSecondary)
            {
                _trackRenderer.DrawHorizontalPlankPath(canvas);
            }
            else
            {
                _trackRenderer.DrawCornerPlankPath(canvas, singlePlank: false);
            }

            using (canvas.Scope())
            {
                if (track.Style is not TIntersectionStyle.StraightAndPrimary and not TIntersectionStyle.StraightAndSecondary)
                {
                    canvas.RotateDegrees(90, 50.0f, 50.0f);
                }

                _trackRenderer.DrawCornerPlankPath(canvas, singlePlank: true);

                if (track.Style is not TIntersectionStyle.StraightAndPrimary and not TIntersectionStyle.StraightAndSecondary)
                {
                    canvas.ClipRect(new Rectangle(0, 0, 100.0f, 50.0f), false, false);
                }

                _trackRenderer.DrawCornerTrack(canvas);
            }

            if (track.Style is TIntersectionStyle.StraightAndPrimary or TIntersectionStyle.StraightAndSecondary)
            {
                _trackRenderer.DrawHorizontalTracks(canvas);
            }
            else
            {
                _trackRenderer.DrawCornerTrack(canvas);
            }
        }
    }
}
