namespace Trains.NET.Engine
{
    internal class PointerTool : ITool
    {
        public string Name => "Pointer";

        public void Execute(int column, int row)
        {
        }

        public bool IsValid(int column, int row) => true;
    }
}
