namespace SFML_Game_Engine.System
{
    public static class MathGE
    {
        /// <summary>
        /// Lerps <paramref name="A"/> to <paramref name="B"/> lineraly using <paramref name="T"/>
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="T"></param>
        /// <returns></returns>
        public static float Lerp(float A, float B, float T)
        {
            return A + (B - A) * T;
        }

        /// <summary>
        /// Contains some helpful easing functions, feel free to expand!
        /// </summary>
        public static class Interpolation
        {
            static float HALF_PI = MathF.PI / 2;

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
                float nt = MathF.Sin(A - 13.0f * (T + 1.0f) * HALF_PI) * MathF.Pow(2.0f, -10.0f * T) + 1.0f;
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
