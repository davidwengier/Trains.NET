using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;

namespace Trains
{
    public class WriteableBitmapFrameBuffer
    {
        public WriteableBitmap Bitmap { get; }
        public IntPtr BackBuffer { get; }
        public int BackBufferStride { get; }
        public bool Locked { get; set; }

        private readonly int _width;
        private readonly int _height;

        public WriteableBitmapFrameBuffer(int width, int height)
        {
            _width = width;
            _height = height;
            this.Bitmap = new WriteableBitmap(_width, _height, 96, 96, PixelFormats.Pbgra32, null);
            // Need to set these whilst on-thread
            this.BackBuffer = this.Bitmap.BackBuffer;
            this.BackBufferStride = this.Bitmap.BackBufferStride;
        }

        public void LockAndDirty()
        {
            if (this.Locked)
            {
                return;
            }
            this.Bitmap.Lock();
            this.Bitmap.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
            this.Locked = true;
        }

        public void Unlock()
        {
            if (!this.Locked)
            {
                return;
            }
            this.Locked = false;
            this.Bitmap.Unlock();
        }
    }
}
