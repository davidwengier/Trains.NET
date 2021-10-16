using System;
using System.Collections.Generic;
using System.Linq;

namespace Trains.NET.Engine
{
    public class GameManager : IGameManager
    {
        private bool _buildMode;
        private ITool _currentTool;
        private readonly ITool _defaultTool;
        private readonly IGameBoard _gameBoard;

        public event EventHandler? Changed;

        public GameManager(IEnumerable<ITool> tools, IGameBoard gameBoard, ILayout layout)
        {
            _defaultTool = tools.First();
            _currentTool = _defaultTool;
            _gameBoard = gameBoard;

            _buildMode = !layout.Any();
            if (_buildMode)
            {
                _defaultTool = tools.Skip(1).First();
            }
        }

        public bool BuildMode
        {
            get { return _buildMode; }
            set
            {
                _buildMode = value;
                _currentTool = _defaultTool;
                _gameBoard.Enabled = !_buildMode;

                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public ITool CurrentTool
        {
            get { return _currentTool; }
            set
            {
                _currentTool = value;

                Changed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
