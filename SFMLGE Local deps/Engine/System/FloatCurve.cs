﻿using SFML_Game_Engine.System;

namespace SFML_Game_Engine.Engine.System
{
    public class FloatCurve
    {
        /// <summary>
        /// All points within this FloatCurve, should never be smaller then 2.
        /// </summary>
        public List<(float position, float value)> points = new List<(float position, float value)>();

        /// <summary>
        /// Creates a new <see cref="FloatCurve"/> going from <paramref name="start"/> to <paramref name="end"/>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public FloatCurve(float start = 0.0f, float end = 1.0f)
        {
            points.Add((0.0f, start));
            points.Add((1.0f, end));
        }

        /// <summary>
        /// Creates a FloatCurve from the given <paramref name="values"/> space apart evenly.
        /// If less then 2 values are given, it will default to 0.0f-1.0f
        /// </summary>
        /// <param name="values">The values to space apart</param>
        public FloatCurve(params float[] values)
        {
            if(values.Length < 2) { points.Add((0.0f, 0.0f)); points.Add((0.0f, 1.0f)); return; }

            float splitSize = 1f / values.Length;

            for (int i = 0; i < values.Length; i++)
            {
                points.Add((splitSize * i, values[i]));
            }
        }

        /// <summary>
        /// Tries to add a point to the float curve, returns false if a point is already at the given position.
        /// </summary>
        /// <param name="position">the position of the new point from 0.0 - 1.0</param>
        /// <param name="value">the value of the new point, can be any float value</param>
        /// <returns></returns>
        public bool AddPoint(float position, float value)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if(points[i].position == position) { return false; }
            }

            points.Add((position, value));
            SortPoints();
            return true;
        }

        /// <summary>
        /// Tries to get the point at the given <paramref name="position"/>, returns true if found, false otherwise.
        /// </summary>
        /// <param name="position">The position to check</param>
        /// <param name="point">The point found, (-1.0, -1.0) if not found.</param>
        /// <returns></returns>
        public bool TryGetPoint(float position, out (float position, float value) point)
        {
            for (int i = 0; i < points.Count; ++i)
            {
                if (points[i].position == position)
                {
                    point = points[i];
                    return true;
                }
            }
            point = (-1.0f, -1.0f);
            return false;
        }

        public bool TrySetPoint(float position, float value)
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

        void SortPoints()
        {
            points.Sort(
                ((float pos, float value) A, (float pos, float value) B) =>
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
        public float Sample(float at)
        {
            if (points == null) throw new NullReferenceException("--> FloatCurve.Points is null!!!");
            if (points.Count == 0) return 0.0f;
            if (points.Count == 1) { return points[0].value; }
            at = MathGE.Clamp(at, 0.0f, 1.0f);

            int toSample = 0;

            for (int i = 0; i < points.Count-1; i++)
            {
                if(at >= points[i].position && at <= points[i+1].position)
                {
                    toSample = i; break;
                }
            }

            float curAt = MathGE.Map(at, points[toSample].position, points[toSample+1].position, 0.0f, 1.0f);
            float startAt = points[toSample].value;
            float endAt = points[toSample+1].value;

            return MathGE.Lerp(startAt, endAt, curAt);
        }

    }
}
