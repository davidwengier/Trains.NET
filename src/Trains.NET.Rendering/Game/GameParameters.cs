namespace Trains.NET.Rendering
{
    public class GameParameters : IGameParameters
    {
        public int CellSize { get; set; }

        public GameParameters()
        {
            this.CellSize = 40;
        }
    }
}
