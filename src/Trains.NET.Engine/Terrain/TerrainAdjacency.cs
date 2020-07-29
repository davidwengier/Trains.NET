namespace Trains.NET.Engine.Terrain
{
    public enum TerrainAdjacency
    {
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight
    }

    public static class AdjacencyTypes
    {
        public static readonly TerrainAdjacency[] OrthogonalAdjacents = new[] { TerrainAdjacency.Up, TerrainAdjacency.Right, TerrainAdjacency.Down, TerrainAdjacency.Left };
        public static readonly TerrainAdjacency[] CornerAdjacents = new[] { TerrainAdjacency.UpLeft, TerrainAdjacency.UpRight, TerrainAdjacency.DownRight, TerrainAdjacency.DownLeft };
    }
}
