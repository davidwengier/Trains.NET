using System.Collections.Generic;
using System.Linq;

namespace Trains.NET.Rendering.Drawing
{
    public class ImageCache : IImageCache
    {
        private readonly object _cacheLock = new object();

        private readonly Dictionary<object, IImage> _disposeBuffer = new();
        private readonly Dictionary<object, IImage> _imageBuffer = new();
        private readonly Dictionary<object, bool> _dirtySTate = new();

        public void Clear()
        {
            foreach (object key in _dirtySTate.Keys.ToArray())
            {
                _dirtySTate[key] = true;
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
            return _dirtySTate.GetValueOrDefault(key) || !_imageBuffer.ContainsKey(key);
        }

        public void SetDirty(object key)
        {
            _dirtySTate[key] = true;
        }

        public void SetDirtyAll(IEnumerable<object> keys)
        {
            foreach (object key in keys)
            {
                SetDirty(key);
            }
        }

        public void Set(object key, IImage image)
        {
            lock (_cacheLock)
            {
                // If we have anything waiting to be disposed, dispose it
                if (_disposeBuffer.TryGetValue(key, out IImage oldImage))
                {
                    oldImage.Dispose();
                }
                // Move the current image into the dispose buffer, this way if anyone is still holding on
                // to it we won't be disposing it out from under them
                if (_imageBuffer.TryGetValue(key, out IImage previousImage))
                {
                    _disposeBuffer[key] = previousImage;
                }
                _imageBuffer[key] = image;
                _dirtySTate[key] = false;
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
                _dirtySTate.Clear();
            }
        }
    }
}
