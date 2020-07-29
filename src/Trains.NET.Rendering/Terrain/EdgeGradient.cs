using System.Collections.Generic;

namespace Trains.NET.Rendering.Terrain
{
    internal class EdgeGradient
    {
        public float XOffset { get; set; }
        public float YOffset { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public IEnumerable<Color> Colours { get; set; }
    }
}
