namespace Trains.NET.Rendering.Drawing;

public partial class ImageCache
{
    private class SuspendSetDirtyCallsTracker(ImageCache owner) : IDisposable
    {
        private readonly ImageCache _owner = owner;
        private readonly HashSet<object> _dirtyQueue = new HashSet<object>();

        internal void Add(object key)
        {
            _dirtyQueue.Add(key);
        }

        public void Dispose()
        {
            _owner.ResumeDirtyProcessing(_dirtyQueue);
        }
    }
}
