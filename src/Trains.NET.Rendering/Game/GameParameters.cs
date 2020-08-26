namespace Trains.NET.Rendering
{
    public class GameParameters : IGameParameters
    {
        public float GameScale { get; set; } = 4f;
        public int CellSize => (int)(40 * this.GameScale);
    }
}
