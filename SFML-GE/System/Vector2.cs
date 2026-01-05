using SFML.Graphics;
using SFML.Graphics.Glsl;
using SFML.System;

namespace SFML_GE.System
{
    /// <summary>
    /// A Vector containing 2 floats X and Y. 
    /// Represents many things from points to directions.
    /// </summary>
    public struct Vector2
    {
        /// <summary>
        /// The first component of the vector, usually left and right where -x is left and +x is right.
        /// </summary>
        public float x;
        /// <summary>
        /// The second component of the vector, usually up and down, where -y is up and +y is down.
        /// </summary>
        public float y;

        // used to be a thing here complaining about readonly X and Y, but its no longer read only, get bent C# nerds im still on the stack!

        // for anyone wondering why there is almost two of every math operation
        /*
         *  (for all examples below, {SFMLVector2f} is a variable made of {SFML.System.Vector2f})
         * 
         *  Mostly for implicit conversions, its way easier to do
         *  { Vector2.Floor(SFMLVector2f) }
         *  instead of
         *  { (SFMLVector2f as Vector2).Floor() }
         *  or even worse if your newer/dont know about as
         *  {
         *    Vector2 floored = (Vector2)SFMLVector2f
         *    floored = floored.Floor()
         *  }
         *  and for some reason { (Vector2)(SFMLVector2f).Floor() } was not working
         * 
         * 
         */


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public Vector2(float x, float y)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Creates a new <see cref="Vector2"/> with X and Y set to <paramref name="xy"/>
        /// </summary>
        /// <param name="xy"></param>
        public Vector2(float xy)
        {
            x = xy;
            y = xy;
        }

        /// <summary>
        /// A Vector2 that equals (0, 0)
        /// </summary>
        public static readonly Vector2 zero = new Vector2(0f, 0f);

        /// <summary>
        /// A Vector2 that equals (1, 1)
        /// </summary>
        public static readonly Vector2 one = new Vector2(1f, 1f);

        /// <summary>
        /// Converts a Vector2 to a new (float, float) tuple
        /// </summary>
        public static (float, float) ToTuple(in Vector2 vec)
        {
            return (vec.x, vec.y);
        }

        /// <summary>
        /// Converts a (float, float) Tuple into a Vector2
        /// </summary>
        public static Vector2 FromTuple(in (float x, float y) tuple)
        {
            return new Vector2(tuple.x, tuple.y);
        }

        /// <summary>
        /// Rotates a point <paramref name="degrees"/> around (0,0)
        /// </summary>
        /// <param name="point"></param>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static Vector2 Rotate(in Vector2 point, float degrees)
        {
            float deg = degrees * (MathF.PI / 180); // Convert degrees to radians

            Vector2 newVec = new Vector2(
                point.x * MathF.Cos(deg) - point.y * MathF.Sin(deg),
                point.y * MathF.Cos(deg) + point.x * MathF.Sin(deg)
                );

            return newVec;
        }

        /// <summary>
        /// Rotates a point <paramref name="degrees"/> around <paramref name="origin"/>
        /// </summary>
        /// <param name="point"></param>
        /// <param name="origin"></param>
        /// <param name="degrees"></param>
        /// <returns>the rotated point</returns>
        public static Vector2 RotateAroundPoint(in Vector2 point, in Vector2 origin, float degrees)
        {
            return Rotate(point - origin, degrees) + origin;
        }

        /// <summary>
        /// Returns the absolute value of a vector.
        /// </summary>
        public static Vector2 Abs(in Vector2 a)
        {
            return new Vector2(MathF.Abs(a.x), MathF.Abs(a.y));
        }

        /// <summary>
        /// Returns the absolute value of a vector.
        /// </summary>
        public readonly Vector2 Abs()
        {
            return new Vector2(MathF.Abs(x), MathF.Abs(y));
        }

        /// <summary>
        /// Swaps x and y in the given <paramref name="vector"/> and returns a new vector with the result.
        /// </summary>
        /// <returns>The new Vector2 where X=Y and Y=X</returns>
        public static Vector2 Flip(in Vector2 vector)
        {
            return new Vector2(vector.y, vector.x);
        }

        /// <summary>
        /// Swaps x and y in the current vector and returns a new vector with the result.
        /// </summary>
        /// <returns>The new Vector2 where X=Y and Y=X</returns>
        public readonly Vector2 Flip()
        {
            return new Vector2(y, x);
        }

        /// <summary>
        /// Returns a new normalized Vector from <paramref name="vec"/>
        /// </summary>
        /// <param name="vec">The vector to normalize</param>
        /// <returns>the normalized Vector2</returns>
        public static Vector2 Normalize(in Vector2 vec)
        {
            float mag = MathF.Sqrt(vec.x * vec.x + vec.y * vec.y);
            if (mag == 0 || float.IsNaN(mag)) { return vec; }
            return vec / mag;
        }

        /// <summary>
        /// Returns a new normalized Vector from this vector.
        /// </summary>
        /// <returns>the normalized Vector2</returns>
        public readonly Vector2 Normalize()
        {
            float mag = MathF.Sqrt(x * x + y * y);
            if (mag == 0 || float.IsNaN(mag)) { return this; }
            return this / mag;
        }

        /// <summary>
        /// Lerps <paramref name="A"/> to <paramref name="B"/> lineraly using <paramref name="T"/>
        /// </summary>
        /// <param name="A">The vector to lerp from</param>
        /// <param name="B">The vector to lerp to</param>
        /// <param name="T">Time where 0.0f is A, and 1.0f is B</param>
        /// <returns>a new Vector Lerped between <paramref name="A"/> and <paramref name="B"/></returns>
        public static Vector2 Lerp(in Vector2 A, in Vector2 B, float T)
        {
            return A + (B - A) * T;
        }

        /// <summary>
        /// Lerps from this <see cref="Vector2"/> to <paramref name="B"/> lineraly using <paramref name="T"/>
        /// </summary>
        /// <param name="B">The vector to lerp to</param>
        /// <param name="T">Time where 0.0f is A, and 1.0f is B</param>
        /// <returns>a new Vector Lerped between this vectors value and <paramref name="B"/></returns>
        public readonly Vector2 Lerp(in Vector2 B, float T)
        {
            return this + (B - this) * T;
        }

        /// <summary>
        /// Gets the distance between <paramref name="A"/> and <paramref name="B"/>
        /// </summary>
        /// <param name="A">The starting vector</param>
        /// <param name="B">The ending vector</param>
        /// <returns>The distance between both vectors as a float</returns>
        public static float Distance(in Vector2 A, in Vector2 B)
        {
            return MathF.Sqrt(MathF.Pow(B.x - A.x, 2) + MathF.Pow(B.y - A.y, 2));
        }

        /// <summary>
        /// Gets the distance between this <see cref="Vector2"/> and <paramref name="B"/>
        /// </summary>
        /// <param name="B">the second vector to get the distance too.</param>
        /// <returns>The distance between both vectors as a float</returns>
        public readonly float Distance(in Vector2 B)
        {
            return MathF.Sqrt(MathF.Pow(B.x - x, 2) + MathF.Pow(B.y - y, 2));
        }

        /// <summary>
        /// Clamps a vector between two vectors, <paramref name="min"/> and <paramref name="max"/>
        /// </summary>
        /// <param name="min">A Vector representing the minumum of both X and Y</param>
        /// <param name="max">A Vector representing the maximum of both X and Y</param>
        /// <returns></returns>
        public readonly Vector2 Clamp(in Vector2 min, in Vector2 max)
        {
            return new Vector2(MathF.Min(MathF.Max(min.x, x), max.x), MathF.Min(MathF.Max(min.y, y), max.y));
        }

        /// <summary>
        /// Clamps both X and Y of this Vector between two floats, <paramref name="min"/> and <paramref name="max"/>
        /// </summary>
        /// <param name="min">The minimum of X and Y</param>
        /// <param name="max">The maximum of X and Y</param>
        /// <returns></returns>
        public readonly Vector2 Clamp(float min, float max)
        {
            return new Vector2(MathF.Min(MathF.Max(min, x), max), MathF.Min(MathF.Max(min, y), max));
        }

        /// <summary>
        /// Clamps a vector with four floats, clamps X between <paramref name="xMin"/> and <paramref name="xMax"/>, then clamps
        /// Y between <paramref name="yMin"/> and <paramref name="yMax"/>
        /// </summary>
        /// <param name="xMin">The minimum X can be</param>
        /// <param name="xMax">The maximum X can be</param>
        /// <param name="yMin">The minimum Y can be</param>
        /// <param name="yMax">The maximum Y can be</param>
        /// <returns>A new clamped vector.</returns>
        public readonly Vector2 Clamp(float xMin, float xMax, float yMin, float yMax)
        {
            return new Vector2(MathF.Min(MathF.Max(xMin, x), xMax), MathF.Min(MathF.Max(yMin, y), yMax));
        }

        //https://stackoverflow.com/questions/10163083/parse-method-for-the-custom-class-c-sharp
        /// <summary>
        /// Parses a string where <c>"5,6"</c> would create a vector where X is 5 and Y is 6.
        /// Throws an exception if it cannot parse the string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>The parsed Vector2 if successful, throws <see cref="ArgumentException"/> otherwise.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Vector2 Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) throw new ArgumentException(input);

            string[] vars = input.Replace(" ", string.Empty).Split(',');
            if (vars.Length != 2) { throw new ArgumentException(input); }

            return new Vector2(float.Parse(vars[0]), float.Parse(vars[1]));
        }

        //https://stackoverflow.com/questions/10163083/parse-method-for-the-custom-class-c-sharp
        /// <summary>
        /// Tries Parses a string where <c>"5,6"</c> would create a vector where X is 5 and Y is 6, then stores it in <paramref name="output"/>.
        /// Output defaults to 0,0.
        /// </summary>
        /// <param name="input">The string to attempt to parse, should be in the format "x,y"</param>
        /// <param name="output">The parsed vector, defaults to 0,0 if the parse failed.</param>
        /// <returns>true if the parse was successful, false otherwise.</returns>
        public static bool TryParse(string input, out Vector2 output)
        {
            output = new Vector2(0, 0);
            if (string.IsNullOrWhiteSpace(input)) return false;

            string[] vars = input.Replace(" ", string.Empty).Split(',');
            if (vars.Length != 2) { return false; }

            bool gotX = float.TryParse(vars[0], out float x);
            bool gotY = float.TryParse(vars[0], out float y);
            if (!gotX && !gotY) { return false; }
            output = new Vector2(x, y);

            return true;
        }

        /// <summary>
        /// Gets the Magnitude of this vector<para/>
        /// Identical to <see cref="Vector2.Length()"/>
        /// </summary>
        /// <returns>The Length/Magnitude of this vector as a float</returns>
        public readonly float Magnitude()
        {
            return MathF.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// Gets the Magnitude of <paramref name="vec"/><para/>
        /// Identical to <see cref="Vector2.Length(in Vector2)"/>
        /// </summary>
        /// <param name="vec">The Vector to get the Length/Magnitude of</param>
        /// <returns>The Length/Magnitude of this vector as a float</returns>
        public static float Magnitude(in Vector2 vec)
        {
            return MathF.Sqrt(vec.x * vec.x + vec.y * vec.y);
        }

        /// <summary>
        /// Gets the Length of this vector.<para/>
        /// Identical to <see cref="Vector2.Magnitude()"/>
        /// </summary>
        /// <returns>The Length/Magnitude of this vector as a float</returns>
        public readonly float Length()
        {
            return MathF.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// Gets the Length of <paramref name="vec"/>.<para/>
        /// Identical to <see cref="Vector2.Magnitude(in Vector2)"/>
        /// </summary>
        /// <param name="vec">The Vector to get the Length/Magnitude of</param>
        /// <returns>The Length/Magnitude of this vector as a float</returns>
        public static float Length(in Vector2 vec)
        {
            return MathF.Sqrt(vec.x * vec.x + vec.y * vec.y);
        }

        /// <summary>
        /// <see cref="MathF.Floor(float)"/>'s the X and Y component of this <paramref name="vec"/>.
        /// </summary>
        /// <param name="vec">The Vector2 to floor.</param>
        public static Vector2 Floor(in Vector2 vec)
        {
            return new Vector2(MathF.Floor(vec.x), MathF.Floor(vec.y));
        }

        /// <summary>
        /// <see cref="MathF.Floor(float)"/>'s the X and Y component of this vector.
        /// </summary>
        public readonly Vector2 Floor()
        {
            return new Vector2(MathF.Floor(x), MathF.Floor(y));
        }

        /// <summary>
        /// <see cref="MathF.Ceiling(float)"/>'s the X and Y component of <paramref name="vec"/>
        /// </summary>
        public static Vector2 Ceil(in Vector2 vec)
        {
            return new Vector2(MathF.Ceiling(vec.x), MathF.Ceiling(vec.y));
        }

        /// <summary>
        /// <see cref="MathF.Ceiling(float)"/>'s the X and Y component of this vector.
        /// </summary>
        public readonly Vector2 Ceil()
        {
            return new Vector2(MathF.Ceiling(x), MathF.Ceiling(y));
        }

        /// <summary>
        /// Returns a new Rounded Vector2 from <paramref name="vec"/>
        /// </summary>
        /// <param name="vec">The Vector2 to be rounded</param>
        public static Vector2 Round(in Vector2 vec)
        {
            return new Vector2(MathF.Round(vec.x), MathF.Round(vec.y));
        }

        /// <summary>
        /// Returns a new Rounded Vector2 from <paramref name="vec"/>
        /// </summary>
        /// <param name="vec">The Vector2 to be rounded</param>
        /// <param name="digits">how many digits to round to, uses <see cref="MathF.Round(float, int)"/></param>
        /// <returns>The rounded Vector2</returns>
        public static Vector2 Round(in Vector2 vec, in int digits)
        {
            return new Vector2(MathF.Round(vec.x, digits), MathF.Round(vec.y, digits));
        }

        /// <summary>
        /// Returns a new Rounded Vector2 from the current vector
        /// <returns>The rounded Vector2</returns>
        /// </summary>
        public readonly Vector2 Round()
        {
            return new Vector2(MathF.Round(x), MathF.Round(y));
        }

        /// <summary>
        /// Returns a new Rounded Vector2 from the current vector
        /// </summary>
        /// <param name="digits">how many digits to round to, uses <see cref="MathF.Round(float, int)"/></param>
        /// <returns></returns>
        public readonly Vector2 Round(int digits)
        {
            return new Vector2(MathF.Round(x, digits), MathF.Round(y, digits));
        }

        /// <summary>
        /// Returns the dot product between this vector and another
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public readonly float Dot(in Vector2 other)
        {
            return (x * other.x) + (y *  other.y);
        }

        /// <summary>
        /// Returns the dot product between two vectors
        /// </summary>
        /// <returns></returns>
        public static float Dot(in Vector2 A, in Vector2 B)
        {
            return (A.x * B.x) + (A.y * B.y);
        }

        /// <summary>
        /// Converts this Vector2 to its string representation: 
        /// <c>
        /// "x,y"
        /// </c>
        /// </summary>
        /// <returns></returns>
        public override readonly string ToString() => x.ToString() + "," + y.ToString();

        /// <summary>
        /// Returns true if this Vector2 matches another Vector2
        /// </summary>
        /// <param name="obj">the object to check against</param>
        /// <returns>true if <paramref name="obj"/> is a Vector2, matches the xy values of this vector2, and isnt null</returns>
        public override readonly bool Equals(object? obj)
        {
            if (obj is null) { return false; }
            if (obj is Vector2 ob)
            {
                return ob.x == x && ob.y == y;
            }
            return base.Equals(obj);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static Vector2 operator +(in Vector2 a, in Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);

        public static Vector2 operator -(in Vector2 a, in Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);

        public static Vector2 operator /(in Vector2 a, in Vector2 b) => new Vector2(a.x / b.x, a.y / b.y);

        public static Vector2 operator *(in Vector2 a, in Vector2 b) => new Vector2(a.x * b.x, a.y * b.y);

        // Summary here since i dont really think a float multiplied by a vector is that common, normally is the other way around
        /// <summary> returns a new vector of (a * b.x, a * b.y) </summary>
        public static Vector2 operator *(float a, in Vector2 b) => new Vector2(a * b.x, a * b.y);

        public static Vector2 operator -(in Vector2 a, float b) => new Vector2(a.x - b, a.y - b);

        public static Vector2 operator +(in Vector2 a, float b) => new Vector2(a.x + b, a.y + b);

        public static Vector2 operator *(in Vector2 a, float b) => new Vector2(a.x * b, a.y * b);

        public static bool operator ==(in Vector2 a, in Vector2 b) => a.x == b.x && a.y == b.y;

        public static bool operator !=(in Vector2 a, in Vector2 b) => a.x != b.x || a.y != b.y;

        public static Vector2 operator /(in Vector2 a, float b) => new Vector2(a.x / b, a.y / b);

        // Below is vector conversion hell, i still dont know why we all dont use Vec<> but whatever

        public static implicit operator Vector2f(in Vector2 vec) => new Vector2f(vec.x, vec.y);

        public static implicit operator Vector2(Vector2f vec) => new Vector2(vec.X, vec.Y);

        public static implicit operator Vec2(in Vector2 vec) => new Vec2(vec.x, vec.y);

        public static explicit operator Vector2i(in Vector2 vec) => new Vector2i((int)Math.Floor(vec.x), (int)Math.Floor(vec.y));

        public static explicit operator Vector2(Vector2i vec) => new Vector2(vec.X, vec.Y);

        public static explicit operator Vector2(Vector2u v) => new Vector2(v.X, v.Y);

        public static implicit operator Vertex(in Vector2 v) => new Vertex(v);

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Returns the hash code for this instance
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override readonly int GetHashCode() // https://stackoverflow.com/questions/7813687/right-way-to-implement-gethashcode-for-this-struct
        {
            return HashCode.Combine(x, y);
        }
    }
}
