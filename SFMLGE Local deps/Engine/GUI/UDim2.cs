using SFML.System;
using SFML_Game_Engine.System;

namespace SFML_Game_Engine.GUI
{
    /// <summary>
    /// A Dimension containing two vectors, a relative scale and an absolute offset, scale will usually be clammped to 0.0-1.0.
    /// </summary>
    public struct UDim2
    {
        /// <summary>
        /// A Vector2 thats relative to some value, normally clammped to 0.0-1.0
        /// </summary>
        public Vector2 scale { get; } // from 0-1

        /// <summary>
        /// A Vector2 thats absolute, usually anywhere from float.MinValue to float.MaxValue
        /// </summary>
        public Vector2 offset { get; }  // any number

        /// <summary>
        /// Creates a new UDim2 from two vectors.
        /// </summary>
        public UDim2(Vector2 scale, Vector2 offset)
        {
            this.scale = scale;
            this.offset = offset;
        }

        /// <summary>
        /// Creates a new UDim2 from four numbers.
        /// </summary>
        public UDim2(float scaleX, float scaleY, float offsetX, float offsetY)
        {
            this.scale = new Vector2(scaleX, scaleY);
            this.offset = new Vector2(offsetX, offsetY);
        }

        public static readonly UDim2 zero = new UDim2(0, 0, 0, 0);

        /// <summary>
        /// Returns the Vector2 this UDim2 computes to.
        /// An example being that, if <paramref name="scaleRelative"/> is a screen size, then
        /// scale.x = 0.5 would be half of the screen width.
        /// </summary>
        public readonly Vector2 GetVector(Vector2 scaleRelative)
        {
            return offset + (scaleRelative * scale.Clamp(0.0f, 1.0f));
        }
    }
}
