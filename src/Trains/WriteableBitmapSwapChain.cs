using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        private readonly WriteableBitmap[] _buffer = new WriteableBitmap[SwapChainCount];
        private readonly IntPtr[] _backBuffers = new IntPtr[SwapChainCount];
        private readonly int[] _backBufferStrides = new int[SwapChainCount];
        private readonly bool[] _buffersLocked = new bool[SwapChainCount];
        private readonly ElapsedMillisecondsTimedStat _renderTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("SwapChain-DrawNext");
        private int _width;
        private int _height;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public void DrawNext(Action<ICanvas> draw)
        {
            int next = (_current + 1) % SwapChainCount;

            if (_width < 1 || _height < 1 || _buffer[next] == null)
            {
                return;
            }

            if (!_buffersLocked[next])
            {
                return;
            }

            _renderTime.Start();

            // Render the game
            var info = new SKImageInfo(_width, _height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            using (var surface = SKSurface.Create(info, _backBuffers[next], _backBufferStrides[next]))
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
                if (_buffer[i] != null)
                {
                    if (_buffersLocked[i])
                    {
                        _buffer[i].Unlock();
                    }
                }
                _buffer[i] = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
                _backBuffers[i] = _buffer[i].BackBuffer;
                _backBufferStrides[i] = _buffer[i].BackBufferStride;
                _buffersLocked[i] = false;
            }
            _buffer[0].Lock();
            _buffersLocked[0] = true;

            _width = width;
            _height = height;
        }

        public void PresentCurrent(Action<ImageSource> present)
        {
            var currentImage = _buffer[_current];

            if (_width < 1 || _height < 1 || currentImage == null)
            {
                return;
            }

            if (_buffersLocked[_current])
            {
                _buffersLocked[_current] = false;
                currentImage.Unlock();
            }

            present(currentImage);

            int next = (_current + 1) % SwapChainCount;

            if (!_buffersLocked[next])
            {
                _buffer[next].Lock();
                _buffer[next].AddDirtyRect(new Int32Rect(0, 0, _width, _height));
                _buffersLocked[next] = true;
            }
        }
    }
}
