using SFML.System;
using SFML_GE.System;

namespace SFML_GE.GUI
{
    /// <summary>
    /// A Dimension containing two vectors, a relative scale and an absolute offset, scale will usually be clammped to 0.0-1.0.
    /// </summary>
    public readonly struct UDim2
    {
        /// <summary>
        /// A Vector2 thats relative to some value, normally clammped to 0.0-1.0
        /// </summary>
        public Vector2 Scale { get; } // from 0-1

        /// <summary>
        /// A Vector2 thats absolute, can be any number.
        /// </summary>
        public Vector2 Offset { get; }  // any number

        /// <summary>
        /// Creates a new UDim2 from two vectors.
        /// </summary>
        public UDim2(Vector2 scale, Vector2 offset)
        {
            this.Scale = scale;
            this.Offset = offset;
        }

        /// <summary>
        /// Creates a new UDim2 from four numbers.
        /// </summary>
        public UDim2(float scaleX, float scaleY, float offsetX, float offsetY)
        {
            Scale = new Vector2(scaleX, scaleY);
            Offset = new Vector2(offsetX, offsetY);
        }

        /// <summary>
        /// A <see cref="UDim2"/> where every value is 0.
        /// </summary>
        public static readonly UDim2 zero = new UDim2(0, 0, 0, 0);

        /// <summary>
        /// Returns the Vector2 this UDim2 computes to.
        /// An example being that, if <paramref name="scaleRelative"/> is a screen size, then
        /// scale.x = 0.5 would be half of the screen width.
        /// </summary>
        public readonly Vector2 GetVector(Vector2 scaleRelative)
        {
            return Offset + scaleRelative * Scale.Clamp(0.0f, 1.0f);
        }
    }
}
