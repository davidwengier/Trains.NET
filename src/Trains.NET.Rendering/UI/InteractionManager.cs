using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.UI
{
    public class InteractionManager : IInteractionManager
    {
        private readonly IEnumerable<IScreen> _screens;
        private readonly IGame _game;
        private readonly IPixelMapper _pixelMapper;
        private IScreen? _capturedScreen;

        public ITool? CurrentTool { get; set; }

        public InteractionManager(IEnumerable<IScreen> screens, IGame game, IPixelMapper pixelMapper)
        {
            _screens = screens;
            _game = game;
            _pixelMapper = pixelMapper;
        }

        public bool PointerClick(int x, int y)
        {
            if (HandleInteraction(x, y, MouseAction.Click))
            {
                return true;
            }


            return false;
        }

        public bool PointerMove(int x, int y)
            => HandleInteraction(x, y, MouseAction.Move);

        public bool PointerDrag(int x, int y)
            => HandleInteraction(x, y, MouseAction.Drag);

        public bool PointerRelease(int x, int y)
        {
            if (_capturedScreen != null)
            {
                _capturedScreen = null;
                return true;
            }
            return false;
        }

        private bool HandleInteraction(int x, int y, MouseAction action)
        {
            (int width, int height) = _game.GetScreenSize();

            if (_capturedScreen != null)
            {
                _capturedScreen.HandleInteraction(x, y, width, height, action);
                return true;
            }

            foreach (IScreen screen in _screens)
            {
                if (screen.HandleInteraction(x, y, width, height, action))
                {
                    _capturedScreen = screen;
                    return true;
                }
            }


            if (this.CurrentTool is not null)
            {
                (int column, int row) = _pixelMapper.ViewPortPixelsToCoords(x, y);

                if (this.CurrentTool.IsValid(column, row) && action != MouseAction.Move)
                {
                    this.CurrentTool.Execute(column, row);
                    return true;
                }
                else if (this.CurrentTool is IDraggableTool draggableTool)
                {
                    if (action == MouseAction.Click)
                    {
                        draggableTool.StartDrag(x, y);
                        return true;
                    }
                    else if (action == MouseAction.Drag)
                    {
                        draggableTool.ContinueDrag(x, y);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
