namespace Trains.NET.Engine.Terrain
{
    public class TerrainCell
    {
        public int Column { get; set; }
        public int Row { get; set; }
        public int Height { get; set; }
        public TerrainType TerrainType { get; set; }
    }
}
