using Trains.NET.Engine;

namespace Trains.NET.Rendering;

public interface IDraggableTool : ITool
{
    void StartDrag(int x, int y);
    void ContinueDrag(int x, int y);
}
