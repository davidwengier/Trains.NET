using System;
using System.Collections.Generic;
using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    public class GameManager : IGameManager
    {
        private bool _buildMode;
        private ITool _currentTool;
        private readonly ITool _defaultTool;

        public event EventHandler? Changed;

        public GameManager(IEnumerable<ITool> tools)
        {
            _defaultTool = tools.First();
            _currentTool = _defaultTool;
        }

        public bool BuildMode
        {
            get { return _buildMode; }
            set
            {
                _buildMode = value;
                _currentTool = _defaultTool;
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
