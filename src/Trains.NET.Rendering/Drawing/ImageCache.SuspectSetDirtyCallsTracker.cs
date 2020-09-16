using System;
using System.Collections.Generic;

namespace Trains.NET.Rendering.Drawing
{
    public partial class ImageCache
    {
        private class SuspectSetDirtyCallsTracker : IDisposable
        {
            private readonly ImageCache _owner;
            private readonly HashSet<object> _dirtyQueue;

            public SuspectSetDirtyCallsTracker(ImageCache owner)
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
