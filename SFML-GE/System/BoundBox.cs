using SFML.Graphics;
using SFML_GE.Debugging;

namespace SFML_GE.System
{
    /// <summary>
    /// A Simple Wrapper around <see cref="FloatRect"/>. Has 5 <see cref="Vector2"/>'s, one for each corner and one for the center of the bound box.
    /// </summary>
    public readonly struct BoundBox
    {
        /// <summary>
        /// The <see cref="FloatRect"/> that makes up this BoundBox, without any rotation.
        /// </summary>
        public FloatRect Rect { get; }
        /// <summary>
        /// The Top Left Corner of this BoundBox
        /// </summary>
        public Vector2 TopLeft { get; }
        /// <summary>
        /// The Top Right Corner of this BoundBox
        /// </summary>
        public Vector2 TopRight { get; }
        /// <summary>
        /// The Bottom Left Corner of this BoundBox
        /// </summary>
        public Vector2 BottomLeft { get; }
        /// <summary>
        /// The Bottom Right Corner of this BoundBox
        /// </summary>
        public Vector2 BottomRight { get; }
        /// <summary>
        /// The Center of this BoundBox
        /// </summary>
        public Vector2 Center { get; }
        /// <summary>
        /// The Size of this BoundBox
        /// </summary>
        public Vector2 Size { get; }
        /// <summary>
        /// Equal to <see cref="TopLeft"/>, and the <see cref="Rect"/>'s <see cref="FloatRect.Position"/>
        /// </summary>
        public Vector2 Position { get; }

        readonly Vector2 minPoint = new Vector2(0, 0);
        readonly Vector2 maxPoint = new Vector2(0, 0);

        /// <summary>
        /// Creates a BoundBox from a <see cref="FloatRect"/>.
        /// </summary>
        /// <param name="rect"></param>
        public BoundBox(FloatRect rect)
        {
            Rect = rect;
            TopLeft = new Vector2(rect.Left, rect.Top);
            TopRight = new Vector2(rect.Left + rect.Width, rect.Top);
            BottomLeft = new Vector2(rect.Left, rect.Top + rect.Height);
            BottomRight = new Vector2(rect.Left + rect.Width, rect.Top + rect.Height);
            Center = new Vector2(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            Size = Rect.Size;
            Position = Rect.Position;
            minPoint = new Vector2(GetMinX(), GetMinY());
            maxPoint = new Vector2(GetMaxX(), GetMaxY());

        }

        /// <summary>
        /// Creates a boundbox where the top left rests at (left, top) and the bottom right at (width, height)
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public BoundBox(float left, float top, float width, float height)
        {
            Rect = new FloatRect(left, top, width, height);
            TopLeft = new Vector2(Rect.Left, Rect.Top);
            TopRight = new Vector2(Rect.Left + Rect.Width, Rect.Top);
            BottomLeft = new Vector2(Rect.Left, Rect.Top + Rect.Height);
            BottomRight = new Vector2(Rect.Left + Rect.Width, Rect.Top + Rect.Height);
            Center = new Vector2(Rect.Left + Rect.Width / 2, Rect.Top + Rect.Height / 2);
            Size = Rect.Size;
            Position = Rect.Position;
            minPoint = new Vector2(GetMinX(), GetMinY());
            maxPoint = new Vector2(GetMaxX(), GetMaxY());

        }

        /// <summary>
        /// Creates a BoundBox from some <see cref="Vector2"/>'s
        /// </summary>
        /// <param name="TopLeft"></param>
        /// <param name="TopRight"></param>
        /// <param name="BottomLeft"></param>
        /// <param name="BottomRight"></param>
        public BoundBox(Vector2 TopLeft, Vector2 TopRight, Vector2 BottomLeft, Vector2 BottomRight)
        {
            Rect = new FloatRect(TopLeft.x, TopLeft.y, TopRight.x - TopLeft.x, BottomLeft.y - TopLeft.y);
            this.TopLeft = TopLeft;
            this.TopRight = TopRight;
            this.BottomLeft = BottomLeft;
            this.BottomRight = BottomRight;
            Center = new Vector2(Rect.Left + Rect.Width / 2, Rect.Top + Rect.Height / 2);
            Size = Vector2.zero;
            Position = Rect.Position;
            minPoint = new Vector2(GetMinX(), GetMinY());
            maxPoint = new Vector2(GetMaxX(), GetMaxY());

            Size = new Vector2(maxPoint.x - minPoint.x, maxPoint.y - minPoint.y);
        }

        /// <summary>
        /// Creates a BoundBox from some <see cref="Vector2"/>'s
        /// </summary>
        /// <param name="TopLeftPosition">The Top left point of this bound box.</param>
        /// <param name="Size">The size of this bound box from the point <paramref name="TopLeftPosition"/></param>
        public BoundBox(Vector2 TopLeftPosition, Vector2 Size)
        {
            Vector2 TopLeft = TopLeftPosition;
            Vector2 TopRight = TopLeftPosition + new Vector2(Size.x, 0);
            Vector2 BottomRight = TopLeftPosition + Size;
            Vector2 BottomLeft = TopLeftPosition + new Vector2(0, Size.y);

            Rect = new FloatRect(TopLeft.x, TopLeft.y, TopRight.x - TopLeft.x, BottomLeft.y - TopLeft.y);
            this.TopLeft = TopLeft;
            this.TopRight = TopRight;
            this.BottomLeft = BottomLeft;
            this.BottomRight = BottomRight;
            Center = new Vector2(Rect.Left + Rect.Width / 2, Rect.Top + Rect.Height / 2);
            this.Size = Vector2.zero;
            this.Position = Rect.Position;
            minPoint = new Vector2(GetMinX(), GetMinY());
            maxPoint = new Vector2(GetMaxX(), GetMaxY());

            this.Size = new Vector2(maxPoint.x - minPoint.x, maxPoint.y - minPoint.y);
        }


        /// <summary> Gets the Minimum X position of this boundbox </summary>
        public float GetMinX()
        {
            return (new float[] { TopLeft.x, BottomLeft.x, TopRight.x, BottomRight.x }).Min();
        }

        /// <summary> Gets the Minimum Y position of this boundbox </summary>
        public float GetMinY()
        {
            return (new float[] { TopLeft.y, BottomLeft.y, TopRight.y, BottomRight.y }).Min();
        }

        /// <summary> Gets the Maximum y position of this boundbox </summary>
        public float GetMaxX()
        {
            return (new float[] { TopLeft.x, BottomLeft.x, TopRight.x, BottomRight.x }).Max();
        }

        /// <summary> Gets the Maximum Y position of this boundbox </summary>
        public float GetMaxY()
        {
            return (new float[] { TopLeft.y, BottomLeft.y, TopRight.y, BottomRight.y }).Max();
        }

        /// <summary>
        /// Checks if a point is within the AABB bounds of this boundbox.<para></para>
        /// Because its Axis Aligned, this method does not account for bounds with rotated or non-rectangular points, see <see cref="WithinBoundsAccurate(Vector2)"/>
        /// </summary>
        public readonly bool WithinBounds(Vector2 point)
        {
            bool inXBounds =
                point.x >= minPoint.x &&
                point.x <= maxPoint.x;
            bool inYBounds =
                point.y >= minPoint.y &&
                point.y <= maxPoint.y;

            return inXBounds && inYBounds;
        }

        //https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle
        // not feeling like understanding this math atm, just gonna use it!
        static bool ptInTriangle(Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            float A = 1.0f / 2.0f * (-p1.y * p2.x + p0.y * (-p1.x + p2.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y);
            float sign = A < 0.0f ? -1.0f : 1.0f;
            float s = (p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y) * sign;
            float t = (p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y) * sign;

            return s > 0.0f && t > 0.0f && s + t < 2.0f * A * sign;
        }

        /// <summary>
        /// Rotates a BoundBox around a givent point <paramref name="p"/> by <paramref name="degrees"/>,
        /// the resulting BoundBox may not be axis aligned, and should be sampled with <see cref="WithinBoundsAccurate(Vector2)"/>
        /// </summary>
        /// <param name="p">the point to rotate around</param>
        /// <param name="degrees">how many degrees to rotate around the given point <paramref name="p"/></param>
        /// <returns></returns>
        public readonly BoundBox Rotate(Vector2 p, float degrees)
        {
            return new BoundBox(
                Vector2.RotateAroundPoint(TopLeft, p, degrees),
                Vector2.RotateAroundPoint(TopRight, p, degrees),
                Vector2.RotateAroundPoint(BottomLeft, p, degrees),
                Vector2.RotateAroundPoint(BottomRight, p, degrees)
                );
        }


        /// <summary>
        /// Checks if a point is within the bounds.
        /// works by checking if the given point is within either of the two triangles that make up this BoundBox,
        /// useful if your bounds are non-rectangular, or rotated.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public readonly bool WithinBoundsAccurate(Vector2 point)
        {
            bool withinFirst = ptInTriangle(point, TopLeft, TopRight, BottomRight);
            bool withinSecond = ptInTriangle(point, BottomRight, BottomLeft, TopLeft);
            return withinFirst || withinSecond;
        }

        /// <summary>
        /// Pushes outward from the center with the given offsets.
        /// <para></para>
        /// as an example, if given a <paramref name="widthOffset"/> of 5, 
        /// then the <see cref="TopLeft"/> and <see cref="BottomLeft"/> bounds will go -5 units in the X direction<para/>
        /// and <see cref="TopRight"/> and <see cref="BottomRight"/> bounds will go +5 units in the Y direction, the same applies for the Y direction with
        /// <paramref name="heightOffset"/>
        /// </summary>
        /// <param name="widthOffset">will push all bounds away from the center of the bound box by this value on the X axis</param>
        /// <param name="heightOffset">will push all bounds away from the center of the bound box by this value on the Y axis</param>
        /// <returns></returns>
        public readonly BoundBox OffsetBoundsByCenter(float widthOffset, float heightOffset)
        {
            return new BoundBox(
                TopLeft - new Vector2(widthOffset, heightOffset),
                TopRight - new Vector2(-widthOffset, heightOffset),
                BottomLeft - new Vector2(widthOffset, -heightOffset),
                BottomRight - new Vector2(-widthOffset, -heightOffset)
            );
        }

        /// <summary>
        /// Draws a BoundBox to the given <see cref="RenderTarget"/>
        /// </summary>
        /// <param name="rt"></param>
        public readonly void Draw(RenderTarget rt)
        {
            rt.Draw(new Vertex[] { TopLeft, TopRight, BottomRight, BottomRight, BottomLeft, TopLeft }, PrimitiveType.LineStrip);
        }

        /// <summary>
        /// Draws this boundbox using the gizmo library
        /// </summary>
        public readonly void DrawGizmo()
        {
            Gizmo.DrawLine(TopLeft, TopRight);
            Gizmo.DrawLine(TopRight, BottomRight);
            Gizmo.DrawLine(BottomRight, BottomLeft);
            Gizmo.DrawLine(BottomLeft, TopLeft);
        }
    }
}
