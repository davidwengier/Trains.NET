using System.Collections.Generic;
using System.Linq;

namespace Trains.NET.Rendering.Drawing
{
    public class ImageCache : IImageCache
    {
        private readonly object _cacheLock = new object();

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
                if (_imageBuffer.TryGetValue(key, out IImage existing))
                {
                    existing.Dispose();
                }
                _imageBuffer[key] = image;
                _dirtySTate[key] = false;
            }
        }

        public void Dispose()
        {
            lock (_cacheLock)
            {
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
