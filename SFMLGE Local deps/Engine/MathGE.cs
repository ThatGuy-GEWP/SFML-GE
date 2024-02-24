namespace SFML_Game_Engine
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


        public static class Interpolation
        {
            static float HALF_PI = MathF.PI / 2;


            public static float Squared(float A, float B, float T)
            {
                return Lerp(A, B, T*T);
            }

            public static float QuadraticEaseOut(float A, float B, float T)
            {
                return Lerp(A, B, 1.0f - (1.0f - T) * (1.0f - T));
            }

            public static float ElasticOut(float A, float B, float T)
            {
                float nt = MathF.Sin(A - 13.0f * (T + 1.0f) * HALF_PI) * MathF.Pow(2.0f, -10.0f * T) + 1.0f;
                return Lerp(A, B, nt);
            }

            public static float SmoothStep(float A, float B, float T)
            {
                float v1 = T * T;
                float v2 = 1.0f - (1.0f * T) * (1.0f - T);
                return Lerp(A, B, Lerp(v1, v2, T));
            }
        }

        /// <summary>
        /// Maps a value from a range, to a range.
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

        public static float Clamp(float value, float min, float max)
        {
            return Math.Min(MathF.Max(value, min), max);
        }

        public static float DegToRad(float degrees)
        {
            return degrees * (MathF.PI / 180); // Convert degrees to radians
        }

        public static float RadToDeg(float radians)
        {
            return radians * (180 / MathF.PI); // Convert radians to degrees
        }
    }
}
