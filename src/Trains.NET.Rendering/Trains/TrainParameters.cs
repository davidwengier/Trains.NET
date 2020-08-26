namespace Trains.NET.Rendering
{
    public class TrainParameters : ITrainParameters
    {
        private readonly IGameParameters _gameParameters;

        public float RearHeight => 22 * _gameParameters.GameScale;

        public float RearWidth => 10 * _gameParameters.GameScale;

        public float HeadWidth => 25 * _gameParameters.GameScale;

        public float HeadHeight => 16 * _gameParameters.GameScale;

        public float StrokeWidth => 2 * _gameParameters.GameScale;

        public float SmokeStackRadius => 2 * _gameParameters.GameScale;

        public float SmokeStackOffset => 5 * _gameParameters.GameScale;

        public TrainParameters(IGameParameters gameParameters)
        {
            _gameParameters = gameParameters;
        }
    }
}
