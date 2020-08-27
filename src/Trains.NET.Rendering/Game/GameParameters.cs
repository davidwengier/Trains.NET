using System;

namespace Trains.NET.Rendering
{
    public class GameParameters : IGameParameters
    {
        private float _gameScale = 1.0f;
        public float GameScale
        {
            get => _gameScale;
            set
            {
                _gameScale = value;
                GameScaleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int CellSize => (int)(40 * this.GameScale);

        public event EventHandler? GameScaleChanged;
    }
}
