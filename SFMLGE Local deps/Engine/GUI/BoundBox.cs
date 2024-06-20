using SFML.Audio;
using SFML.Graphics;
using SFML_Game_Engine.System;

namespace SFML_Game_Engine.GUI
{
    /// <summary>
    /// A Simple Wrapper around <see cref="FloatRect"/>. Has 5 <see cref="Vector2"/>'s, one for each corner and one in the center of the bound box.
    /// </summary>
    public readonly struct BoundBox
    {
        public FloatRect Rect { get; }
        public Vector2 TopLeft { get; }
        public Vector2 TopRight { get; }
        public Vector2 BottomLeft { get; }
        public Vector2 BottomRight { get; }
        public Vector2 Center { get; }
        public Vector2 Size { get; } // simply so i dont have to keep getting stupid "Vector2f + Vector2 is ambiguiosososasasd" garbage
        public Vector2 Position { get; } // simply so i dont have to keep getting stupid "Vector2f + Vector2 is ambiguiosososasasd" garbage


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
            Center = new Vector2(rect.Left + (rect.Width / 2), rect.Top + (rect.Height / 2));
            Size = Rect.Size;
            Position = Rect.Position;
        }

        /// <summary>
        /// Creates a BoundBox from some <see cref="Vector2"/>'s, also generates a <see cref="FloatRect"/> from the given corners so <see cref="Rect"/> isnt null/all zeros
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
            Center = new Vector2(Rect.Left + (Rect.Width / 2), Rect.Top + (Rect.Height / 2));
            Size = Rect.Size;
            Position = Rect.Position;
        }
        
        /// <summary>
        /// Checks if a point is within the bounds of this boundbox.
        /// </summary>
        public readonly bool WithinBounds(Vector2 point)
        {
            bool inXBounds =
                point.x >= TopLeft.x &&
                point.x <= BottomRight.x;
            bool inYBounds =
                point.y >= TopLeft.y &&
                point.y <= BottomRight.y;

            return inXBounds && inYBounds;
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
        /// Sets a rectangle shapes position, size, fill color, outline thickness and outline color for debug viewing of bound boxes.
        /// does not actually draw the rectangle shape
        /// </summary>
        /// <param name="shape"></param>
        public readonly void SetRect(RectangleShape shape)
        {
            shape.FillColor = Color.Transparent;
            shape.OutlineColor = Color.Red; 
            shape.OutlineThickness = 1;
            shape.Position = Rect.Position;
            shape.Size = Rect.Size;
        }
    }
}
