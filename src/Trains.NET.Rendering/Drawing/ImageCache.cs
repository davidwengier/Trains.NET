using System;
using System.Collections.Generic;
using System.Linq;

namespace Trains.NET.Rendering.Drawing
{
    public partial class ImageCache : IImageCache
    {
        private readonly object _cacheLock = new object();

        private readonly Dictionary<object, IImage> _disposeBuffer = new();
        private readonly Dictionary<object, IImage> _imageBuffer = new();
        private readonly Dictionary<object, bool> _dirtyState = new();
        private SuspectSetDirtyCallsTracker? _setDirtyCallsTracker;

        public void Clear()
        {
            lock (_cacheLock)
            {
                SetDirtyAll(_dirtyState.Keys.ToArray());
            }
        }

        public IImage? Get(object key)
        {
            lock (_cacheLock)
            {
                return _imageBuffer.GetValueOrDefault(key);
            }
        }

        public bool IsDirty(object key)
        {
            lock (_cacheLock)
            {
                if (!_imageBuffer.ContainsKey(key))
                {
                    return true;
                }

                if (!_dirtyState.ContainsKey(key))
                {
                    return true;
                }

                return _dirtyState[key];
            }
        }

        public void SetDirty(object key)
        {
            lock (_cacheLock)
            {
                if (_setDirtyCallsTracker is not null)
                {
                    _setDirtyCallsTracker.Add(key);
                    return;
                }
                _dirtyState[key] = true;
            }
        }

        public void SetDirtyAll(IEnumerable<object> keys)
        {
            lock (_cacheLock)
            {
                foreach (object key in keys)
                {
                    SetDirty(key);
                }
            }
        }

        public void Set(object key, IImage image)
        {
            lock (_cacheLock)
            {
                // If we have anything waiting to be disposed, dispose it
                if (_disposeBuffer.TryGetValue(key, out IImage? oldImage))
                {
                    oldImage.Dispose();
                }
                // Move the current image into the dispose buffer, this way if anyone is still holding on
                // to it we won't be disposing it out from under them
                if (_imageBuffer.TryGetValue(key, out IImage? previousImage))
                {
                    _disposeBuffer[key] = previousImage;
                }
                _imageBuffer[key] = image;
                _dirtyState[key] = false;
            }
        }

        public void Dispose()
        {
            lock (_cacheLock)
            {
                foreach (IImage image in _disposeBuffer.Values)
                {
                    image.Dispose();
                }
                foreach (IImage image in _imageBuffer.Values)
                {
                    image.Dispose();
                }
                _imageBuffer.Clear();
                _dirtyState.Clear();
                _setDirtyCallsTracker?.Dispose();
            }
        }

        public IDisposable SuspendSetDirtyCalls()
        {
            lock (_cacheLock)
            {
                if (_setDirtyCallsTracker is not null)
                {
                    throw new InvalidOperationException("Suspending dirty processing cannot be nested. Sorry!");
                }

                _setDirtyCallsTracker = new SuspectSetDirtyCallsTracker(this);
                return _setDirtyCallsTracker;
            }
        }

        private void ResumeDirtyProcessing(IEnumerable<object> keys)
        {
            lock (_cacheLock)
            {
                _setDirtyCallsTracker = null;

                SetDirtyAll(keys);
            }
        }
    }
}
