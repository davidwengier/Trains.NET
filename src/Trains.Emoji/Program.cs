using System;
using System.IO;
using System.Linq;
using SkiaSharp;
using Trains.NET.Engine;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;
using Trains.NET.Rendering.Trains;

namespace Trains.Emoji
{
    public class EmojiDrawer
    {
        private readonly int _size;
        private readonly ITreeRenderer _tree;
        private readonly ITrackRenderer _track;
        private readonly (string color, ITrainRenderer renderer)[] _trains;
        private const string FolderName = "EmojiOutput";

        public static void Main() => new EmojiDrawer().Save();

        public EmojiDrawer()
        {
            ITrackParameters trackParameters = new TrackParameters();
            ITrainParameters trainParameters = new TrainParameters();
            IBitmapFactory bitmapFactory = new SKBitmapFactory();
            IPathFactory pathFactory = new SKPathFactory();
            ITrackPathBuilder trackPathBuilder = new TrackPathBuilder(trackParameters, pathFactory);

            _tree = new TreeRenderer(bitmapFactory, trackParameters);
            _track = new TrackRenderer(trackParameters, bitmapFactory, trackPathBuilder);

            _trains = typeof(ITrainPalette).Assembly.GetTypes()
                        .Where(x => !x.IsInterface && !x.IsAbstract && typeof(ITrainPalette).IsAssignableFrom(x) && x.GetConstructor(Type.EmptyTypes) != null)
                        .Select(x => (ITrainPalette)Activator.CreateInstance(x)!)
                        .Select(x => (x.GetType().Name, (ITrainRenderer)new TrainRenderer(trackParameters, trainParameters, new TrainPainter(new OrderedList<ITrainPalette>(new object[] { x })))))
                        .ToArray();

            _size = trackParameters.CellSize;
        }

        public void Save()
        {
            if (!Directory.Exists(FolderName))
            {
                Directory.CreateDirectory(FolderName);
            }
            for (int i = 0; i < 3; i++)
            {
                Draw("tree" + i, x => _tree.Render(x, i));
            }
            foreach (TrackDirection direction in (TrackDirection[])Enum.GetValues(typeof(TrackDirection)))
            {
                Draw("track" + direction, x => _track.Render(x, new Track(null) { Direction = direction }));
            }
            foreach (TrackDirection direction in ((TrackDirection[])Enum.GetValues(typeof(TrackDirection))).Where(x => x.IsThreeWay()))
            {
                Draw("trackAlt" + direction, x => _track.Render(x, new Track(null) { Direction = direction, AlternateState = true }));
            }
            foreach ((string color, ITrainRenderer trainRenderer) in _trains)
            {
                DrawTrain($"train{color}Up", TrackDirection.Vertical, 270, trainRenderer);
                DrawTrain($"train{color}Down", TrackDirection.Vertical, 90, trainRenderer);
                DrawTrain($"train{color}Left", TrackDirection.Horizontal, 180, trainRenderer);
                DrawTrain($"train{color}Right", TrackDirection.Horizontal, 0, trainRenderer);
            }
        }

        public void Draw(string name, Action<ICanvas> x)
        {
            using var bitmap = new SKBitmap(_size, _size, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            using var skCanvas = new SKCanvas(bitmap);
            using (ICanvas canvas = new SKCanvasWrapper(skCanvas))
            {
                canvas.Save();
                x(canvas);
                canvas.Restore();
            }
            using Stream s = File.OpenWrite(FolderName + "\\" + name + ".png");
            bitmap.Encode(s, SKEncodedImageFormat.Png, 100);
        }

        public void DrawTrain(string name, TrackDirection trackDirection, float angle, ITrainRenderer trainRenderer) =>
            Draw(name, x =>
            {
                x.Save();
                _track.Render(x, new Track(null) { Direction = trackDirection });
                x.Restore();
                var train = new Train();
                train.SetAngle(angle);
                trainRenderer.Render(x, train);
            });
    }
}
