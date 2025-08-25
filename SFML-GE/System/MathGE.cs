﻿using SFML.Graphics;
using System.Runtime.CompilerServices;

namespace SFML_GE.System
{
    /// <summary>
    /// Contains a bunch of helper math functions for things like 
    /// <para/><see cref="Lerp(float, float, float)"/> (and <see cref="MathGE.Interpolation"/>)
    /// <para/><see cref="DegToRad(float)"/>, <see cref="RadToDeg(float)"/>
    /// <para/><see cref="Map(float, float, float, float, float)"/>
    /// </summary>
    public static class MathGE
    {
        /// <summary>
        /// Lerps a float <paramref name="A"/> to another float <paramref name="B"/> lineraly using <paramref name="T"/>
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="T"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float A, float B, float T)
        {
            return A + (B - A) * T;
        }

        /// <summary>
        /// Lerps a color <paramref name="A"/> to another color <paramref name="B"/> lineraly using <paramref name="T"/>
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="T"></param>
        /// <returns></returns>
        public static Color Lerp(Color A, Color B, float T)
        {
            byte r = (byte)MathF.Round(Lerp(A.R, B.R, T));
            byte g = (byte)MathF.Round(Lerp(A.G, B.G, T));
            byte b = (byte)MathF.Round(Lerp(A.B, B.B, T));
            byte a = (byte)MathF.Round(Lerp(A.A, B.A, T));

            return new Color(r, g, b, a);
        }

        /// <summary>
        /// Lerps a float <paramref name="A"/> to another float <paramref name="B"/> lineraly using <paramref name="T"/>
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="T"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Lerp(double A, double B, double T)
        {
            return A + (B - A) * T;
        }

        // Feel free to expand!!!
        /// <summary>
        /// Contains some helpful easing functions.
        /// </summary>
        public static class Interpolation
        {
            private static readonly double HALF_PI = Math.PI / 2.0;

            public static float QuadraticEaseIn(float A, float B, float T)
            {
                T = Clamp(T, 0, 1);
                return Lerp(A, B, T * T);
            }

            public static float QuadraticEaseOut(float A, float B, float T)
            {
                T = Clamp(T, 0, 1);
                return Lerp(A, B, 1.0f - (1.0f - T) * (1.0f - T));
            }

            public static float ElasticOut(float A, float B, float T)
            {
                T = Clamp(T, 0, 1);
                float nt = MathF.Sin(A - 13.0f * (T + 1.0f) * (float)HALF_PI) * MathF.Pow(2.0f, -10.0f * T) + 1.0f;
                return Lerp(A, B, nt);
            }

            public static float SmoothStep(float A, float B, float T)
            {
                T = Clamp(T, 0, 1);
                float v1 = T * T;
                float v2 = 1.0f - (1.0f - T) * (1.0f - T);
                return Lerp(A, B, Lerp(v1, v2, T));
            }
        }

        /// <summary>
        /// Maps a <paramref name="value"/> from a range of (<paramref name="fromMin"/> -> <paramref name="fromMax"/>) to (<paramref name="toMin"/> -> <paramref name="toMax"/>)
        /// </summary>
        /// <param name="value">Value to map</param>
        /// <param name="fromMin">The Minimum of the value</param>
        /// <param name="fromMax">The Maximum of the value</param>
        /// <param name="toMin">The target Minimum</param>
        /// <param name="toMax">The target Maximum</param>
        /// <returns></returns>
        public static float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        }

        /// <summary>
        /// Clamps a <paramref name="value"/> to a minimum of <paramref name="min"/> and a maximum of <paramref name="max"/>
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <param name="min">The minimum the value can be</param>
        /// <param name="max">The max the value can be</param>
        /// <returns></returns>
        public static float Clamp(float value, float min, float max)
        {
            return Math.Min(MathF.Max(value, min), max);
        }

        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        /// <returns></returns>
        public static float DegToRad(float degrees)
        {
            return degrees * (MathF.PI / 180); // Convert degrees to radians
        }

        /// <summary>
        /// the sine of x from between 0.0 and 1.0, equal to MathF.Sin(x)*0.5 + 0.5
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float ZeroSin(float x)
        {
            return MathF.Sin(x) / 2f + 0.5f;
        }

        /// <summary>
        /// the cosine of x from between 0.0 and 1.0, equal to MathF.Cos(x)*0.5 + 0.5
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float ZeroCos(float x)
        {
            return MathF.Cos(x) / 2f + 0.5f;
        }

        /// <summary>
        /// Converts radians to degrees
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static float RadToDeg(float radians)
        {
            return radians * (180 / MathF.PI); // Convert radians to degrees
        }
    }
}
