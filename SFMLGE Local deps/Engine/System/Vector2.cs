using SFML.Graphics;
using SFML.Graphics.Glsl;
using SFML.System;

namespace SFML_Game_Engine.System
{
    public struct Vector2
    {
        public float x;
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

        public Vector2(float x, float y)
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
            this.x = xy;
            this.y = xy;
        }

        public static readonly Vector2 zero = new Vector2(0f, 0f);

        /// <summary>
        /// Converts a Vector2 to a new (float, float) tuple
        /// </summary>
        public static (float, float) ToTuple(Vector2 vec)
        {
            return (vec.x, vec.y);
        }

        /// <summary>
        /// Converts a (float, float) Tuple into a Vector2
        /// </summary>
        public static Vector2 FromTuple((float x, float y) tuple)
        {
            return new Vector2(tuple.x, tuple.y);
        }

        /// <summary>
        /// Rotates a point <paramref name="degrees"/> around (0,0)
        /// </summary>
        /// <param name="point"></param>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static Vector2 Rotate(Vector2 point, float degrees)
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
        public static Vector2 RotateAroundPoint(Vector2 point, Vector2 origin, float degrees)
        {
            return Rotate(point - origin, degrees) + origin;
        }

        /// <summary>
        /// Returns the absolute value of a vector.
        /// </summary>
        public static Vector2 Abs(Vector2 a)
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
        public static Vector2 Flip(Vector2 vector)
        {
            return new Vector2(vector.y, vector.x);
        }

        /// <summary>
        /// Swaps x and y in the current vector and returns a new vector with the result.
        /// </summary>
        public readonly Vector2 Flip()
        {
            return new Vector2(y, x);
        }

        /// <summary>
        /// Returns a new normalized Vector from <paramref name="vec"/>
        /// </summary>
        /// <param name="vec">The vector to normalize</param>
        /// <returns></returns>
        public static Vector2 Normalize(Vector2 vec)
        {
            float mag = MathF.Sqrt(vec.x * vec.x + vec.y * vec.y);
            if (mag == 0 || mag == float.NaN) { return vec; }
            return vec / mag;
        }

        /// <summary>
        /// Returns a new normalized Vector from this vector.
        /// </summary>
        /// <returns></returns>
        public readonly Vector2 Normalize()
        {
            float mag = MathF.Sqrt(x * x + y * y);
            if (mag == 0 || mag == float.NaN) { return this; }
            return this / mag;
        }

        /// <summary>
        /// Lerps <paramref name="A"/> to <paramref name="B"/> lineraly using <paramref name="T"/>
        /// </summary>
        /// <param name="A">The vector to lerp from</param>
        /// <param name="B">The vector to lerp to</param>
        /// <param name="T">Time where 0.0f is A, and 1.0f is B</param>
        /// <returns></returns>
        public static Vector2 Lerp(Vector2 A, Vector2 B, float T)
        {
            return A + (B - A) * T;
        }

        /// <summary>
        /// Lerps from this <see cref="Vector2"/> to <paramref name="B"/> lineraly using <paramref name="T"/>
        /// </summary>
        /// <param name="B">The vector to lerp to</param>
        /// <param name="T">Time where 0.0f is A, and 1.0f is B</param>
        /// <returns></returns>
        public readonly Vector2 Lerp(Vector2 B, float T)
        {
            return this + (B - this) * T;
        }

        /// <summary>
        /// Gets the distance between <paramref name="A"/> and <paramref name="B"/>
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static float Distance(Vector2 A, Vector2 B)
        {
            return MathF.Sqrt(MathF.Pow(B.x - A.x, 2) + MathF.Pow(B.y - A.y, 2));
        }

        /// <summary>
        /// Gets the distance between this <see cref="Vector2"/> and <paramref name="B"/>
        /// </summary>
        /// <param name="B"></param>
        /// <returns></returns>
        public readonly float Distance(Vector2 B)
        {
            return MathF.Sqrt(MathF.Pow(B.x - x, 2) + MathF.Pow(B.y - y, 2));
        }

        /// <summary>
        /// Clamps a vector between two vectors, <paramref name="min"/> and <paramref name="max"/>
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public readonly Vector2 Clamp(Vector2 min, Vector2 max)
        {
            return new Vector2(MathF.Min(MathF.Max(min.x, x), max.x), MathF.Min(MathF.Max(min.y, y), max.y));
        }

        /// <summary>
        /// Clamps a vector between two floats, <paramref name="min"/> and <paramref name="max"/>
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public readonly Vector2 Clamp(float min, float max)
        {
            return new Vector2(MathF.Min(MathF.Max(min, x), max), MathF.Min(MathF.Max(min, y), max));
        }

        /// <summary>
        /// Clamps a vector with four floats, clamps X between <paramref name="xMin"/> and <paramref name="xMax"/>, then clamps
        /// Y between <paramref name="yMin"/> and <paramref name="yMax"/>
        /// </summary>
        /// <param name="xMin"></param>
        /// <param name="xMax"></param>
        /// <param name="yMin"></param>
        /// <param name="yMax"></param>
        /// <returns></returns>
        public readonly Vector2 Clamp(float xMin, float xMax, float yMin, float yMax)
        {
            return new Vector2(MathF.Min(MathF.Max(xMin, x), xMax), MathF.Min(MathF.Max(yMin, y), yMax));
        }

        //https://stackoverflow.com/questions/10163083/parse-method-for-the-custom-class-c-sharp
        /// <summary>
        /// Parses a string where <c>"5,6"</c> would create a vector where X is 5 and Y is 6.
        /// Throws an exception if it cant parse the string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool TryParse(string input, out Vector2 output)
        {
            output = new Vector2(0, 0);
            if (string.IsNullOrWhiteSpace(input)) return false;

            string[] vars = input.Replace(" ", string.Empty).Split(',');
            if (vars.Length != 2) { return false; }

            bool gotX = float.TryParse(vars[0], out float x);
            bool gotY = float.TryParse(vars[0], out float y);
            if (!gotX && !gotY) { return false; }
            output = new Vector2 (x, y);

            return true;
        }

        /// <summary>
        /// Gets the Magnitude of a vector
        /// </summary>
        /// <returns></returns>
        public readonly float Magnitude()
        {
            return MathF.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// Gets the Magnitude of a vector
        /// </summary>
        /// <returns></returns>
        public static float Magnitude(Vector2 vec)
        {
            return MathF.Sqrt(vec.x * vec.x + vec.y * vec.y);
        }

        /// <summary>
        /// Gets the cross product of two Vector2's <paramref name="A"/> and <paramref name="B"/>
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static float Cross(Vector2 A, Vector2 B)
        {
            return A.x * B.y - A.y * B.x;
        }

        /// <summary>
        /// Returns a new Floored Vector2 from <paramref name="vec"/>
        /// </summary>
        public static Vector2 Floor(Vector2 vec)
        {
            return new Vector2(MathF.Floor(vec.x), MathF.Floor(vec.y));
        }

        /// <summary>
        /// Returns a new Floored Vector2 from the current vector
        /// </summary>
        public readonly Vector2 Floor()
        {
            return new Vector2(MathF.Floor(x), MathF.Floor(y));
        }

        /// <summary>
        /// Returns a new Ceil'ed? Vector2 from <paramref name="vec"/>
        /// </summary>
        public static Vector2 Ceil(Vector2 vec)
        {
            return new Vector2(MathF.Ceiling(vec.x), MathF.Ceiling(vec.y));
        }

        /// <summary>
        /// Returns a new Ceil'ed? Vector2 from the current vector
        /// </summary>
        public readonly Vector2 Ceil()
        {
            return new Vector2(MathF.Ceiling(x), MathF.Ceiling(y));
        }

        /// <summary>
        /// Returns a new Rounded Vector2 from <paramref name="vec"/>
        /// </summary>
        public static Vector2 Round(Vector2 vec)
        {
            return new Vector2(MathF.Round(vec.x), MathF.Round(vec.y));
        }

        /// <summary>
        /// Returns a new Rounded Vector2 from the current vector
        /// </summary>
        public readonly Vector2 Round()
        {
            return new Vector2(MathF.Round(x), MathF.Round(y));
        }

        public override readonly string ToString()
        {
            return x.ToString() + "," + y.ToString();
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) { return false; }
            if (obj is Vector2)
            {
                Vector2 ob = (Vector2)obj;
                return ob.x == x && ob.y == y;
            }
            return base.Equals(obj);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);

        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);

        public static Vector2 operator /(Vector2 a, Vector2 b) => new Vector2(a.x / b.x, a.y / b.y);

        public static Vector2 operator *(Vector2 a, Vector2 b) => new Vector2(a.x * b.x, a.y * b.y);

        // Summary here since i dont really think a float multiplied by a vector is that common, normally is the other way around
        /// <summary> returns a new vector of (a * b.x, a * b.y) </summary>
        public static Vector2 operator *(float a, Vector2 b) => new Vector2(a * b.x, a * b.y);

        public static Vector2 operator -(Vector2 a, float b) => new Vector2(a.x - b, a.y - b);

        public static Vector2 operator +(Vector2 a, float b) => new Vector2(a.x + b, a.y + b);

        public static Vector2 operator *(Vector2 a, float b) => new Vector2(a.x * b, a.y * b);

        public static bool operator ==(Vector2 a, Vector2 b) => a.x == b.x && a.y == b.y;

        public static bool operator !=(Vector2 a, Vector2 b) => a.x != b.x || a.y != b.y;

        public static Vector2 operator /(Vector2 a, float b) => new Vector2(a.x / b, a.y / b);

        // Below is vector conversion hell, i still dont know why we all dont use Vec<> but whatever

        public static implicit operator Vector2f(Vector2 vec) => new Vector2f(vec.x, vec.y);

        public static implicit operator Vector2(Vector2f vec) => new Vector2(vec.X, vec.Y);

        public static implicit operator Vec2(Vector2 vec) => new Vec2(vec.x, vec.y);

        public static explicit operator Vector2i(Vector2 vec) => new Vector2i((int)Math.Floor(vec.x), (int)Math.Floor(vec.y));

        public static explicit operator Vector2(Vector2i vec) => new Vector2(vec.X, vec.Y);

        public static explicit operator Vector2(Vector2u v) => new Vector2(v.X, v.Y);

        public static implicit operator Vertex(Vector2 v) => new Vertex(v);

        public override int GetHashCode() // https://stackoverflow.com/questions/7813687/right-way-to-implement-gethashcode-for-this-struct
        {
            unchecked // Overflow is fine, just wrap | no idea what that means! thank you random internet person!!!
            {
                int hash = 17;
                hash = hash * 23 + x.GetHashCode();
                hash = hash * 23 + y.GetHashCode();
                return hash;
            }
        }
    }
}
