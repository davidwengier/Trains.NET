namespace Trains.NET.Rendering
{
    public class TrackParameters : ITrackParameters
    {
        private readonly IGameParameters _gameParameters;

        public int NumPlanks { get; } = 3;
        public int NumCornerPlanks => this.NumPlanks + 1;

        public float PlankLength => 26 * _gameParameters.GameScale;
        public float PlankWidth => 4.0f * _gameParameters.GameScale;
        public float TrackWidth => (int)(12 * _gameParameters.GameScale);
        public float RailWidth => 4f * _gameParameters.GameScale;
        public float RailTopWidth => 2.75f * _gameParameters.GameScale;

        public TrackParameters(IGameParameters gameParameters)
        {
            _gameParameters = gameParameters;
        }
    }
}
