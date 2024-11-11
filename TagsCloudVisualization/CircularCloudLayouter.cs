using System.Drawing;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter
    {
        public readonly Point Center;
        public List<Rectangle> Tags { get; }

        private double angle;
        private double spiralStep;

        public CircularCloudLayouter(Point center)
        {
            Center = center;
            Tags = new List<Rectangle>();
            angle = 0;
            spiralStep = 0.1;
        }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            if (rectangleSize.Height <= 0 || rectangleSize.Width <= 0)
            {
                throw new ArgumentException("Width and height of the rectangle must be positive.");
            }

            Rectangle newRectangle;

            do
            {
                var location = GetSpiralPosition();
                location.Offset(-rectangleSize.Width / 2, rectangleSize.Height / 2);

                newRectangle = new Rectangle(location, rectangleSize);
                angle += spiralStep;
            }
            while (IntersectsWithAny(newRectangle));

            newRectangle = ShiftRectangleToCenter(newRectangle);
            Tags.Add(newRectangle);
            return newRectangle;
        }

        private Point GetSpiralPosition()
        {
            var x = (int)(Center.X + angle * Math.Cos(angle));
            var y = (int)(Center.Y + angle * Math.Sin(angle));
            return new Point(x, y);
        }

        private bool IntersectsWithAny(Rectangle rectangle)
        {
            foreach (var tag in Tags)
            {
                if (tag.IntersectsWith(rectangle))
                    return true;
            }
            return false;
        }

        private Rectangle ShiftRectangleToCenter(Rectangle rectangle)
        {
            var rectangleCenter = new Point(
                rectangle.Left + rectangle.Width / 2,
                rectangle.Top - rectangle.Height / 2);

            var directionToCenter = new Point(
                Center.X - rectangleCenter.X,
                Center.Y - rectangleCenter.Y
            );

            while (Math.Abs(directionToCenter.X) > 0 || Math.Abs(directionToCenter.Y) > 0)
            {
                var stepX = Math.Sign(directionToCenter.X);
                var stepY = Math.Sign(directionToCenter.Y);

                var shiftedRectangle = new Rectangle(
                    new Point(rectangle.X + stepX, rectangle.Y + stepY),
                    rectangle.Size
                );

                if (IntersectsWithAny(shiftedRectangle))
                    break;

                rectangle = shiftedRectangle;
                directionToCenter.X -= stepX;
                directionToCenter.Y -= stepY;
            }

            return rectangle;
        }

        public Size Size()
        {
            if (Tags.Count == 0) return System.Drawing.Size.Empty;

            var left = Tags.Min(rectangle => rectangle.Left);
            var right = Tags.Max(rectangle => rectangle.Right);
            var top = Tags.Min(rectangle => rectangle.Top);
            var bottom = Tags.Max(rectangle => rectangle.Bottom);

            return new Size(right - left, bottom - top);
        }
    }
}