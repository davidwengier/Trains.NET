using System;
using System.Collections.Generic;

namespace Trains.NET.Rendering.Drawing
{
    public partial class ImageCache
    {
        private class SuspendSetDirtyCallsTracker : IDisposable
        {
            private readonly ImageCache _owner;
            private readonly HashSet<object> _dirtyQueue;

            public SuspendSetDirtyCallsTracker(ImageCache owner)
            {
                _owner = owner;
                _dirtyQueue = new HashSet<object>();
            }

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
}
