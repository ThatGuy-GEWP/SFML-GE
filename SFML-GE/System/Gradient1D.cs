using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_GE.System
{
    /// <summary>
    /// A 1 Dimentional <see cref="Color"/> Gradient, containing any amount of points between a start and an end <see cref="Color"/>.
    /// You can sample this gradient using <see cref="Sample(float)"/> using any value between 0.0 - 1.0.
    /// For wraping existing values to that range, see <see cref="MathGE.Map(float, float, float, float, float)"/>
    /// </summary>
    public class Gradient1D
    {
        /// <summary>
        /// All points within this FloatCurve, should never be smaller then 2.
        /// </summary>
        public List<(float position, Color value)> points = new List<(float position, Color value)>();

        /// <summary>
        /// Creates a new <see cref="FloatCurve"/> going from black to white
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public Gradient1D()
        {
            points.Add((0.0f, Color.Black));
            points.Add((1.0f, Color.White));
        }

        /// <summary>
        /// Creates a new <see cref="FloatCurve"/> going from <paramref name="start"/> to <paramref name="end"/>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public Gradient1D(Color start, Color end)
        {
            points.Add((0.0f, start));
            points.Add((1.0f, end));
        }

        /// <summary>
        /// Tries to add a point to the float curve, returns false if a point is already at the given position.
        /// </summary>
        /// <param name="position">the position of the new point from 0.0 - 1.0</param>
        /// <param name="value">the value of the new point, can be any float value</param>
        /// <returns></returns>
        public bool AddPoint(float position, Color value)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].position == position) { return false; }
            }

            points.Add((position, value));
            SortPoints();
            return true;
        }

        /// <summary>
        /// Tries to get the point at the given <paramref name="position"/>, returns true if found, false otherwise.
        /// </summary>
        /// <param name="position">The position to check</param>
        /// <param name="point">The point found, (-1.0, (0,0,0,0)) if not found.</param>
        /// <returns></returns>
        public bool TryGetPoint(float position, out (float position, Color value) point)
        {
            for (int i = 0; i < points.Count; ++i)
            {
                if (points[i].position == position)
                {
                    point = points[i];
                    return true;
                }
            }
            point = (-1.0f, Color.Transparent);
            return false;
        }

        /// <summary>
        /// Tries to set an existing point at the given <paramref name="position"/> to the Color <paramref name="value"/>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="value"></param>
        /// <returns>False if no point exists at that position, true if set successfully</returns>
        public bool TrySetPoint(float position, Color value)
        {
            for (int i = 0; i < points.Count; ++i)
            {
                if (points[i].position == position)
                {
                    points[i] = (position, value);
                    return true;
                }
            }

            return false;
        }
        
        // sorts the points internally!
        void SortPoints()
        {
            points.Sort(
                ((float pos, Color value) A, (float pos, Color value) B) =>
                {
                    if (A.pos < B.pos) { return -1; }
                    if (A.pos > B.pos) { return 1; }
                    return 0;
                }
            );
        }

        /// <summary>
        /// Samples the <see cref="FloatCurve"/> at the given position <paramref name="at"/>.
        /// </summary>
        /// <param name="at">where to sample the float curve from ranging 0.0f to 1.0f </param>
        public Color Sample(float at)
        {
            if (points == null) throw new NullReferenceException("--> FloatCurve.Points is null!!!");
            if (points.Count == 0) return Color.Black;
            if (points.Count == 1) { return points[0].value; }
            at = MathGE.Clamp(at, 0.0f, 1.0f);

            int toSample = 0;

            for (int i = 0; i < points.Count - 1; i++)
            {
                if (at >= points[i].position && at <= points[i + 1].position)
                {
                    toSample = i; break;
                }
            }

            float curAt = MathGE.Map(at, points[toSample].position, points[toSample + 1].position, 0.0f, 1.0f);
            Color startAt = points[toSample].value;
            Color endAt = points[toSample + 1].value;

            byte r = (byte)MathF.Round(MathGE.Lerp(startAt.R, endAt.R, curAt));
            byte g = (byte)MathF.Round(MathGE.Lerp(startAt.G, endAt.G, curAt));
            byte b = (byte)MathF.Round(MathGE.Lerp(startAt.B, endAt.B, curAt));
            byte a = (byte)MathF.Round(MathGE.Lerp(startAt.A, endAt.A, curAt));

            return new Color(r, g, b, a);
        }
    }
}
