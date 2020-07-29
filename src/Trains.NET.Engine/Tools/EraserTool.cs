namespace Trains.NET.Engine
{
    [Order(20)]
    internal class EraserTool : ITool
    {
        private readonly IStaticEntityCollection _collection;

        public ToolMode Mode => ToolMode.Build;
        public string Name => "Eraser";

        public EraserTool(IStaticEntityCollection trackLayout)
        {
            _collection = trackLayout;
        }

        public void Execute(int column, int row)
        {
            _collection.Remove(column, row);
        }

        public bool IsValid(int column, int row) => _collection.TryGet(column, row, out _);
    }
}
