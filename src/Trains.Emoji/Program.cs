using System;
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
        private readonly IRenderer<Tree> _tree;
        private readonly IRenderer<Track> _track;
        private readonly IRenderer<Train> _train;
        private readonly IPixelMapper _pixelMapper;
        private const string FolderName = "EmojiOutput";

        public static void Main()
            => DI.ServiceLocator.GetService<EmojiDrawer>().Save(512);

        public EmojiDrawer(IRenderer<Tree> tree, IRenderer<Track> track, IRenderer<Train> train, IPixelMapper pixelMapper)
        {
            _tree = tree;
            _track = track;
            _train = train;
            _pixelMapper = pixelMapper;
        }

        public void Save(int imageSize)
        {
            _pixelMapper.AdjustGameScale(imageSize / 40.0f);

            if (!Directory.Exists(FolderName))
            {
                Directory.CreateDirectory(FolderName);
            }
            for (int i = 0; i < 3; i++)
            {
                Draw("tree" + i, x => _tree.Render(x, new Tree()));
            }
            foreach (TrackDirection direction in (TrackDirection[])Enum.GetValues(typeof(TrackDirection)))
            {
                Draw("track" + direction, x => _track.Render(x, new Track() { Direction = direction }));
            }
            foreach ((TrackDirection direction, Track track) in ((TrackDirection[])Enum.GetValues(typeof(TrackDirection)))
                                                    .Select(x => (Direction: x, Track: new Track() { Direction = x, AlternateState = true }))
                                                    .Where(x => x.Track.HasAlternateState()))
            {
                Draw("trackAlt" + direction, x => _track.Render(x, track));
            }
            for (int i = 0; i < 6; i++)
            {
                DrawTrain($"train{i}Up", TrackDirection.Vertical, 270);
                DrawTrain($"train{i}Down", TrackDirection.Vertical, 90);
                DrawTrain($"train{i}Left", TrackDirection.Horizontal, 180);
                DrawTrain($"train{i}Right", TrackDirection.Horizontal, 0);
            }
        }

        public void Draw(string name, Action<ICanvas> x)
        {
            using var bitmap = new SKBitmap(_pixelMapper.CellSize, _pixelMapper.CellSize, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            using var skCanvas = new SKCanvas(bitmap);
            using (ICanvas canvas = new SKCanvasWrapper(skCanvas))
            {
                canvas.Save();

                float scale = _pixelMapper.CellSize / 100.0f;
                canvas.Scale(scale, scale);

                x(canvas);

                canvas.Restore();
            }
            using Stream s = File.OpenWrite(FolderName + "\\" + name + ".png");
            bitmap.Encode(s, SKEncodedImageFormat.Png, 100);
        }

        public void DrawTrain(string name, TrackDirection trackDirection, float angle) =>
            Draw(name, x =>
            {
                x.Save();
                _track.Render(x, new Track() { Direction = trackDirection });
                x.Restore();
                var train = new Train();
                train.SetAngle(angle);
                _train.Render(x, train);
            });
    }
}
