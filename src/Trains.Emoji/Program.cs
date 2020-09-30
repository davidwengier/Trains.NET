using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkiaSharp;
using Trains.NET.Engine;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;

namespace Trains.Emoji
{
    public class EmojiDrawer
    {
        private static int s_numberOfTrainsToDraw = 6;
        private static int s_numberOfTreesToDraw = 3;

        private readonly IRenderer<Tree> _treeRenderer;
        private readonly IRenderer<Track> _trackRenderer;
        private readonly IRenderer<Track> _bridgeRenderer;
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

        public EmojiDrawer(IRenderer<Tree> treeRenderer, TrackRenderer trackRenderer, BridgeRenderer bridgeRenderer, IRenderer<Train> trainRenderer, IPixelMapper pixelMapper)
        {
            _treeRenderer = treeRenderer;
            _trackRenderer = trackRenderer;
            _bridgeRenderer = bridgeRenderer;
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

                DrawTrains(train, $"train{i}", folderName, pixelMapper);
                DrawTrains(train, $"trainAndTrack{i}", folderName, pixelMapper, _trackRenderer);
                DrawTrains(train, $"trainAndBridge{i}", folderName, pixelMapper, _bridgeRenderer, _trackRenderer);
            }
        }

        private void DrawTracks(string folderName, IPixelMapper pixelMapper)
        {
            foreach (TrackDirection direction in (TrackDirection[])Enum.GetValues(typeof(TrackDirection)))
            {
                if (direction == TrackDirection.Undefined) continue;

                DrawTracks("track", direction, folderName, pixelMapper, _trackRenderer);
                DrawTracks("bridge", direction, folderName, pixelMapper, _bridgeRenderer, _trackRenderer);
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

        public static void DrawTracks(string prefix, TrackDirection direction, string folderName, IPixelMapper pixelMapper, params IRenderer<Track>[] trackRenderers)
        {
            var track = new Track() { Direction = direction };
            Draw(prefix + direction, folderName, pixelMapper, canvas => RenderTrack(canvas, trackRenderers, track));
            if (track.HasAlternateState())
            {
                track.AlternateState = true;
                Draw(prefix + direction + "Alt", folderName, pixelMapper, canvas => RenderTrack(canvas, trackRenderers, track));
            }
        }

        private static void RenderTrack(ICanvas canvas, IRenderer<Track>[] trackRenderers, Track track)
        {
            foreach (IRenderer<Track> renderer in trackRenderers)
            {
                using (canvas.Scope())
                {
                    renderer.Render(canvas, track);
                }
            }
        }

        public void DrawTrains(Train train, string prefix, string folderName, IPixelMapper pixelMapper, params IRenderer<Track>[] trackRenderers)
        {
            DrawTrain(train, $"{prefix}Up", folderName, pixelMapper, TrackDirection.Vertical, 270, trackRenderers);
            DrawTrain(train, $"{prefix}Down", folderName, pixelMapper, TrackDirection.Vertical, 90, trackRenderers);
            DrawTrain(train, $"{prefix}Left", folderName, pixelMapper, TrackDirection.Horizontal, 180, trackRenderers);
            DrawTrain(train, $"{prefix}Right", folderName, pixelMapper, TrackDirection.Horizontal, 0, trackRenderers);
        }

        public void DrawTrain(Train train, string name, string folderName, IPixelMapper pixelMapper, TrackDirection trackDirection, float angle, IRenderer<Track>[] trackRenderers) =>
            Draw(name, folderName, pixelMapper, canvas =>
            {
                RenderTrack(canvas, trackRenderers, new Track() { Direction = trackDirection });
                train.SetAngle(angle);
                _trainRenderer.Render(canvas, train);
            });
    }
}
