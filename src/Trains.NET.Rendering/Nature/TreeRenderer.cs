using System;
using Trains.NET.Engine;

namespace Trains.NET.Rendering;

public class TreeRenderer : IStaticEntityRenderer<Tree>
{
    private readonly float _maxTreeSize;
    private readonly float _minTreeSize;
    private readonly float _centerOffset;
    private readonly float _baseRadius;
    private readonly PaintBrush _baseTreeBrush;
    private readonly PaintBrush _topTreeBrush;

    public TreeRenderer()
    {
        _centerOffset = 50.0f;
        _baseRadius = 25.0f;
        _minTreeSize = _baseRadius;
        _maxTreeSize = _baseRadius * 1.25f;
        _baseTreeBrush = new PaintBrush
        {
            Color = new Color("#1B633A"),
            Style = PaintStyle.Fill
        };
        _topTreeBrush = new PaintBrush
        {
            Color = new Color("#236A42"),
            Style = PaintStyle.Fill,
            IsAntialias = true
        };
    }

    public void Render(ICanvas canvas, Tree tree)
    {
        canvas.Translate(_centerOffset, _centerOffset);

        // Let's make some repeatable numbers

        BasicPRNG r = tree.GetPRNG();
        int seed = Math.Abs(tree.Seed) + 1;
        int circleCount = seed % 10 + 10;

        // Draw a base fill
        canvas.DrawCircle(0, 0, _baseRadius, _baseTreeBrush);

        // Draw a few base circles
        float angleOffset = r.NextFloat(0, (float)(Math.PI / 2.0f));
        DrawTreeLayer(canvas, r, circleCount, angleOffset, 1.0f, _baseTreeBrush);

        // Draw a few top circles
        angleOffset = r.NextFloat(0, (float)(Math.PI / 2.0f));
        DrawTreeLayer(canvas, r, (int)(circleCount * 0.7), angleOffset, 0.5f, _topTreeBrush);
    }

    private void DrawTreeLayer(ICanvas canvas, BasicPRNG r, int circleCount, float angleOffset, float scale, PaintBrush brush)
    {
        for (int i = 0; i < circleCount; i++)
        {
            float angle = angleOffset + (float)(Math.PI * 2.0 * i / circleCount);
            float offset = r.NextFloat(_minTreeSize, _maxTreeSize);
            float radius = r.NextFloat(10.0f, 15.0f);
            float x = (float)(scale * offset * Math.Cos(angle));
            float y = (float)(scale * offset * Math.Sin(angle));

            canvas.DrawCircle(x, y, radius, brush);
        }
    }
}
