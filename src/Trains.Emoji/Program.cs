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

        private readonly IRenderer<Tree> _tree;
        private readonly IRenderer<Track> _track;
        private readonly IRenderer<Track> _bridge;
        private readonly IRenderer<Train> _train;
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

        public EmojiDrawer(IRenderer<Tree> tree, TrackRenderer track, BridgeRenderer bridge, IRenderer<Train> train, IPixelMapper pixelMapper)
        {
            _tree = tree;
            _track = track;
            _bridge = bridge;
            _train = train;
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
                for (int i = 0; i < s_numberOfTreesToDraw; i++)
                {
                    Draw("tree" + i, folderName, pixelMapper, x => _tree.Render(x, new Tree() { Seed = i }));
                }
                foreach (TrackDirection direction in (TrackDirection[])Enum.GetValues(typeof(TrackDirection)))
                {
                    if (direction == TrackDirection.Undefined) continue;

                    Draw("track" + direction, folderName, pixelMapper, x => _track.Render(x, new Track() { Direction = direction }));
                }
                foreach ((TrackDirection direction, Track track) in ((TrackDirection[])Enum.GetValues(typeof(TrackDirection)))
                                                        .Select(x => (Direction: x, Track: new Track() { Direction = x, AlternateState = true }))
                                                        .Where(x => x.Track.HasAlternateState()))
                {
                    Draw("trackAlt" + direction, folderName, pixelMapper, x => _track.Render(x, track));
                }

                // Reset so all of our "Train{i}" images are the same colour
                EmojiTrainPainter.Reset();
                for (int i = 0; i < s_numberOfTrainsToDraw; i++)
                {
                    var train = new Train();

                    DrawTrain(train, $"train{i}Up", folderName, pixelMapper, TrackDirection.Vertical, 270);
                    DrawTrain(train, $"train{i}Down", folderName, pixelMapper, TrackDirection.Vertical, 90);
                    DrawTrain(train, $"train{i}Left", folderName, pixelMapper, TrackDirection.Horizontal, 180);
                    DrawTrain(train, $"train{i}Right", folderName, pixelMapper, TrackDirection.Horizontal, 0);

                    DrawTrain(train, $"trainAndTrack{i}Up", folderName, pixelMapper, TrackDirection.Vertical, 270, _track);
                    DrawTrain(train, $"trainAndTrack{i}Down", folderName, pixelMapper, TrackDirection.Vertical, 90, _track);
                    DrawTrain(train, $"trainAndTrack{i}Left", folderName, pixelMapper, TrackDirection.Horizontal, 180, _track);
                    DrawTrain(train, $"trainAndTrack{i}Right", folderName, pixelMapper, TrackDirection.Horizontal, 0, _track);

                    DrawTrain(train, $"trainAndBridge{i}Up", folderName, pixelMapper, TrackDirection.Vertical, 270, _bridge, _track);
                    DrawTrain(train, $"trainAndBridge{i}Down", folderName, pixelMapper, TrackDirection.Vertical, 90, _bridge, _track);
                    DrawTrain(train, $"trainAndBridge{i}Left", folderName, pixelMapper, TrackDirection.Horizontal, 180, _bridge, _track);
                    DrawTrain(train, $"trainAndBridge{i}Right", folderName, pixelMapper, TrackDirection.Horizontal, 0, _bridge, _track);
                }
            }
        }

        public static void Draw(string name, string folderName, IPixelMapper pixelMapper, Action<ICanvas> x)
        {
            using var bitmap = new SKBitmap(pixelMapper.CellSize, pixelMapper.CellSize, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            using var skCanvas = new SKCanvas(bitmap);
            using (ICanvas canvas = new SKCanvasWrapper(skCanvas))
            {
                canvas.Save();

                float scale = pixelMapper.CellSize / 100.0f;
                canvas.Scale(scale, scale);

                x(canvas);

                canvas.Restore();
            }
            using Stream s = File.OpenWrite(Path.Combine(folderName, name + ".png"));
            bitmap.Encode(s, SKEncodedImageFormat.Png, 100);
        }

        public void DrawTrain(Train train, string name, string folderName, IPixelMapper pixelMapper, TrackDirection trackDirection, float angle, params IRenderer<Track>[] trackRenderers) =>
            Draw(name, folderName, pixelMapper, x =>
            {
                foreach (IRenderer<Track> renderer in trackRenderers)
                {
                    x.Save();
                    renderer.Render(x, new Track() { Direction = trackDirection });
                    x.Restore();
                }
                train.SetAngle(angle);
                _train.Render(x, train);
            });
    }
}
