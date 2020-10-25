using System;

namespace Trains.NET.Rendering.UI
{
    public class PictureButton : ButtonBase
    {
        private readonly Picture _picture;
        private readonly float _pictureSize;

        public PictureButton(Picture picture, float pictureSize, Func<bool> isActive, Action onClick)
            : base(isActive, onClick)
        {
            _picture = picture;
            _pictureSize = pictureSize;
        }

        public override int GetMinimumWidth(ICanvas canvas)
            => (int)_pictureSize;

        protected override void RenderButtonLabel(ICanvas canvas)
        {
            canvas.DrawPicture(_picture, (this.Width - _pictureSize) / 2, (this.Height - _pictureSize) / 2, _pictureSize);
        }
    }
}
