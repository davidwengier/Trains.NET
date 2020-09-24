using System.Windows.Media;
using SkiaSharp;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;
using Trains.NET.Instrumentation;
using System;

namespace Trains
{
    public class WriteableBitmapSwapChain : ISwapChain
    {
        private const int SwapChainCount = 3;
        private int _current;
        private readonly WriteableBitmapFrameBuffer[] _swapChain = new WriteableBitmapFrameBuffer[SwapChainCount];
        private readonly ElapsedMillisecondsTimedStat _renderTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("SwapChain-DrawNext");
        private int _width;
        private int _height;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public void DrawNext(Action<ICanvas> draw)
        {
            int next = (_current + 1) % SwapChainCount;

            WriteableBitmapFrameBuffer buffer = _swapChain[next];

            if (_width < 1 || _height < 1 || buffer == null || !buffer.Locked)
            {
                return;
            }

            _renderTime.Start();

            // Render the game
            var info = new SKImageInfo(_width, _height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            using (var surface = SKSurface.Create(info, buffer.BackBuffer, buffer.BackBufferStride))
            {
                if (surface != null)
                {
                    draw(new SKCanvasWrapper(surface.Canvas));
                }
            }

            _renderTime.Stop();

            _current = next;
        }

        public void SetSize(int width, int height)
        {
            if (width == _width && height == _height)
            {
                return;
            }

            for (int i = 0; i < SwapChainCount; i++)
            {
                if (_swapChain[i] != null && _swapChain[i].Locked)
                {
                    _swapChain[i].Bitmap.Unlock();
                }
                _swapChain[i] = new WriteableBitmapFrameBuffer(width, height);
            }

            _swapChain[_current].LockAndDirty();

            _width = width;
            _height = height;
        }

        public void PresentCurrent(Action<ImageSource> present)
        {
            var current = _swapChain[_current];

            if (_width < 1 || _height < 1 || current == null)
            {
                return;
            }

            current.Unlock();

            present(current.Bitmap);

            int next = (_current + 1) % SwapChainCount;

            _swapChain[next].LockAndDirty();
        }
    }
}
