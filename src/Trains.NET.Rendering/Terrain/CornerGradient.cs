using System.Collections.Generic;

namespace Trains.NET.Rendering.Terrain
{
    internal class CornerGradient
    {
        public float XOffset { get; set; }
        public float YOffset { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float CircleCentreX { get; internal set; }
        public float CircleCentreY { get; internal set; }
        public float CircleRadius { get; internal set; }
        public IEnumerable<Color> Colours { get; set; }

    }
}
