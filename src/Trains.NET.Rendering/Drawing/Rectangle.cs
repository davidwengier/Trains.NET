namespace Trains.NET.Rendering
{
    public class Rectangle
    {
        public Rectangle(float left, float top, float right, float bottom)
        {
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;
        }

        public float Left { get; }
        public float Right { get; }
        public float Top { get; }
        public float Bottom { get; }
    }
}
