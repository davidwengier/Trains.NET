using System;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Drawing;
using Xunit;

namespace Trains.NET.Tests
{
    public class ImageCacheTests : IDisposable
    {
        private readonly ImageCache _imageCache = new ImageCache();

        [Fact]
        public void BasicCacheOperations()
        {
            object key = new object();
            var image = new TestImage();

            // non existent is dirty
            Assert.True(_imageCache.IsDirty(key));
            // non existent doesn't throw
            Assert.Null(_imageCache.Get(key));

            // set clears dirty
            _imageCache.Set(key, image);
            Assert.False(_imageCache.IsDirty(key));

            // Check image is cached
            Assert.Equal(image, _imageCache.Get(key));

            /// set doesn't dispose
            _imageCache.Set(key, new TestImage());
            Assert.False(image.IsDisposed);

            // Check image has changed
            Assert.NotEqual(image, _imageCache.Get(key));

            // 2nd set disposes
            _imageCache.Set(key, new TestImage());
            Assert.True(image.IsDisposed);
        }

        [Fact]
        public void DirtyTests()
        {
            object key = new object();
            var image = new TestImage();

            _imageCache.Set(key, image);
            Assert.False(_imageCache.IsDirty(key));
            Assert.Equal(image, _imageCache.Get(key));

            _imageCache.SetDirty(key);
            Assert.True(_imageCache.IsDirty(key));
            Assert.Equal(image, _imageCache.Get(key));

            _imageCache.Set(key, image);
            Assert.False(_imageCache.IsDirty(key));
            Assert.Equal(image, _imageCache.Get(key));

            _imageCache.SetDirtyAll(new object[] { key });
            Assert.True(_imageCache.IsDirty(key));
            Assert.Equal(image, _imageCache.Get(key));

            _imageCache.Set(key, image);
            Assert.False(_imageCache.IsDirty(key));
            Assert.Equal(image, _imageCache.Get(key));
        }

        public void Dispose()
        {
            _imageCache.Dispose();
        }

        private class TestImage : IImage
        {
            public bool IsDisposed { get; private set; }
            public void Dispose()
            {
                this.IsDisposed = true;
            }
        }
    }
}
