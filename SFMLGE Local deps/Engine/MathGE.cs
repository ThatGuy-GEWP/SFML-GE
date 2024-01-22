namespace SFML_Game_Engine
{
    public static class MathGE
    {
        /// <summary>
        /// Lerps <paramref name="A"/> to <paramref name="B"/> lineraly using <paramref name="X"/>
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="X"></param>
        /// <returns></returns>
        public static float Lerp(float A, float B, float X)
        {
            X = X > 1.0f ? 1.0f : X;
            return A + (B - A) * X;
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
