using Trains.NET.Engine.Terrain;
using Trains.NET.Rendering.Terrain;

namespace Trains.NET.Rendering
{
    public class TerrainCellGradient
    {
        public TerrainCell Terrain { get; set; }
        public TerrainAdjacency Adjacency { get; set; }
        public GradientType GradientType { get; set; }
        public GradientType? GradientTypeJoin { get; set; }
    }
}
