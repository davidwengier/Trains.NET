using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public interface IInteractionManager
    {
        ITool? CurrentTool { get; set; }

        bool PointerClick(int x, int y);

        bool PointerMove(int x, int y);

        bool PointerDrag(int x, int y);
        bool PointerRelease(int x, int y);
    }
}
