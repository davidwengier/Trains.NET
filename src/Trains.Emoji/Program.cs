using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkiaSharp;
using Trains.NET.Engine;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;

namespace Trains.Emoji;

public class EmojiDrawer
{
    private static int s_numberOfTrainsToDraw = 6;
    private static int s_numberOfTreesToDraw = 3;

    private readonly IRenderer<Tree> _treeRenderer;
    private readonly IEnumerable<IRenderer<Track>> _trackRenderers;
    private readonly IRenderer<Train> _trainRenderer;
    private readonly IPixelMapper _pixelMapper;
    private const string BaseFolderName = "EmojiOutput";

    public static void Main(string[] args)
    {
        var imageSizes = new List<int>();

        for (int i = 0; i < args.Length; i++)
        {
            if (IsArg(args[i], "trees"))
            {
                s_numberOfTreesToDraw = Convert.ToInt32(args[++i]);
            }
            else if (IsArg(args[i], "trains"))
            {
                s_numberOfTrainsToDraw = Convert.ToInt32(args[++i]);
            }
            else
            {
                imageSizes.Add(Convert.ToInt32(args[i]));
            }
        }
        if (imageSizes.Count == 0)
        {
            imageSizes.Add(512);
        }
        DI.ServiceLocator.GetService<EmojiDrawer>().Save(imageSizes);

        static bool IsArg(string actual, string expected)
            => string.Equals(actual, "--" + expected, StringComparison.OrdinalIgnoreCase)
            || string.Equals(actual, "/" + expected, StringComparison.OrdinalIgnoreCase)
            || string.Equals(actual, "-" + expected, StringComparison.OrdinalIgnoreCase);
    }

    public EmojiDrawer(IRenderer<Tree> treeRenderer, IEnumerable<IRenderer<Track>> trackRenderers, IRenderer<Train> trainRenderer, IPixelMapper pixelMapper)
    {
        _treeRenderer = treeRenderer;
        _trackRenderers = trackRenderers;
        _trainRenderer = trainRenderer;
        _pixelMapper = pixelMapper;
    }

    public void Save(IEnumerable<int> imageSizes)
    {
        if (Directory.Exists(BaseFolderName))
        {
            Directory.Delete(BaseFolderName, true);
        }
        foreach (int imageSize in imageSizes)
        {
            string folderName = imageSizes.Count() > 1 ? Path.Combine(BaseFolderName, imageSize.ToString()) : BaseFolderName;
            IPixelMapper pixelMapper = _pixelMapper.Snapshot();
            pixelMapper.AdjustGameScale(imageSize / 40.0f);

            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }
            DrawTrees(folderName, pixelMapper);
            DrawTracks(folderName, pixelMapper);

            // Reset so all of our "Train{i}" images are the same colour, at even if different sizes
            EmojiTrainPainter.Reset();
            DrawTrains(folderName, pixelMapper);
        }
    }

    private void DrawTrains(string folderName, IPixelMapper pixelMapper)
    {
        for (int i = 0; i < s_numberOfTrainsToDraw; i++)
        {
            var train = new Train();

            DrawTrains(train, $"train{i}", folderName, pixelMapper, null);
            foreach (var renderer in _trackRenderers)
            {
                DrawTrains(train, $"trainAnd{RendererName(renderer)}{i}", folderName, pixelMapper, renderer);
            }
        }
    }

    private static string RendererName(IRenderer<Track> renderer)
    {
        return renderer.GetType().Name.Replace("Renderer", "");
    }

    private void DrawTracks(string folderName, IPixelMapper pixelMapper)
    {
        foreach (SingleTrackDirection direction in (SingleTrackDirection[])Enum.GetValues(typeof(SingleTrackDirection)))
        {
            if (direction == SingleTrackDirection.Undefined) continue;

            foreach (var trackRenderer in _trackRenderers)
            {
                DrawTracks(RendererName(trackRenderer), direction, folderName, pixelMapper, trackRenderer);
            }
        }
    }

    private void DrawTrees(string folderName, IPixelMapper pixelMapper)
    {
        for (int i = 0; i < s_numberOfTreesToDraw; i++)
        {
            Draw("tree" + i, folderName, pixelMapper, canvas => _treeRenderer.Render(canvas, new Tree() { Seed = i }));
        }
    }

    public static void Draw(string name, string folderName, IPixelMapper pixelMapper, Action<ICanvas> renderMethod)
    {
        using var bitmap = new SKBitmap(pixelMapper.CellSize, pixelMapper.CellSize, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
        using var skCanvas = new SKCanvas(bitmap);
        using (ICanvas canvas = new SKCanvasWrapper(skCanvas))
        using (canvas.Scope())
        {
            float scale = pixelMapper.CellSize / 100.0f;
            canvas.Scale(scale, scale);

            renderMethod(canvas);
        }
        using Stream s = File.OpenWrite(Path.Combine(folderName, name + ".png"));
        bitmap.Encode(s, SKEncodedImageFormat.Png, 100);
    }

    public static void DrawTracks(string prefix, SingleTrackDirection direction, string folderName, IPixelMapper pixelMapper, IRenderer<Track> trackRenderer)
    {
        // TODO: This needs to be way smarter about track types
        var track = new Bridge() { Direction = direction };
        Draw(prefix + direction, folderName, pixelMapper, canvas => RenderTrack(canvas, trackRenderer, track));
        if (track.HasMultipleStates)
        {
            // TODO: This is broken
            track.NextState();
            Draw(prefix + direction + "Alt", folderName, pixelMapper, canvas => RenderTrack(canvas, trackRenderer, track));
        }
    }

    private static void RenderTrack(ICanvas canvas, IRenderer<Track> trackRenderer, Track track)
    {
        using (canvas.Scope())
        {
            trackRenderer.Render(canvas, track);
        }
    }

    public void DrawTrains(Train train, string prefix, string folderName, IPixelMapper pixelMapper, IRenderer<Track>? trackRenderer)
    {
        DrawTrain(train, $"{prefix}Up", folderName, pixelMapper, SingleTrackDirection.Vertical, 270, trackRenderer);
        DrawTrain(train, $"{prefix}Down", folderName, pixelMapper, SingleTrackDirection.Vertical, 90, trackRenderer);
        DrawTrain(train, $"{prefix}Left", folderName, pixelMapper, SingleTrackDirection.Horizontal, 180, trackRenderer);
        DrawTrain(train, $"{prefix}Right", folderName, pixelMapper, SingleTrackDirection.Horizontal, 0, trackRenderer);
    }

    public void DrawTrain(Train train, string name, string folderName, IPixelMapper pixelMapper, SingleTrackDirection trackDirection, float angle, IRenderer<Track>? trackRenderer) =>
        Draw(name, folderName, pixelMapper, canvas =>
        {
            if (trackRenderer != null)
            {
                // TODO: This needs to be way smarter about track types
                RenderTrack(canvas, trackRenderer, new Bridge() { Direction = trackDirection });
            }
            train.SetAngle(angle);
            _trainRenderer.Render(canvas, train);
        });
}
