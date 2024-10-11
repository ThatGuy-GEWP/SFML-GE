using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_GE.System
{
    /// <summary>
    /// Thread safe random, stolen shamelessly from <seealso href="https://devblogs.microsoft.com/pfxteam/getting-random-numbers-in-a-thread-safe-way/">here</seealso>
    /// </summary>
    public static class RandomGen
    {
        private static Random _global = new Random(
            DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond);

        [ThreadStatic]
        private static Random _local = null!;

        static float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }

        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0
        /// </summary>
        /// <returns>A single-precision floating point number that is greater than or equal to 0.0 and less than 1.0</returns>
        public static float NextSingle()
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return inst.NextSingle();
        }

        /// <summary>
        /// Returns a random non-negative integer
        /// </summary>
        /// <returns>A 32-Bit signed integer that is greater than or equal to 0 and less than <see cref="int.MaxValue"/></returns>
        public static int Next()
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return inst.Next();
        }

        /// <summary>
        /// returns a random integer that is within a specified range
        /// </summary>
        /// <param name="minValue">the lowest (inclusive) number of the random integer</param>
        /// <param name="maxValue">the highest (exclusive) number of the random integer</param>
        /// <returns>A 32-bit signed integer greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/></returns>
        public static int Next(int minValue, int maxValue)
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return inst.Next(minValue, maxValue);
        }

        /// <summary>
        /// Returns a random integer that is greater than or equal to 0, and less than <paramref name="maxValue"/>
        /// </summary>
        /// <returns>A 32-bit signed integer that is greater than or equal to 0 and less than <paramref name="maxValue"/></returns>
        public static int Next(int maxValue)
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return inst.Next(0, maxValue);
        }

        /// <summary>
        /// returns a random float that is within a specified range
        /// </summary>
        /// <param name="minValue">the lowest (inclusive) number of the random float</param>
        /// <param name="maxValue">the highest (exclusive) number of the random float</param>
        /// <returns>A 32-bit signed integer greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/></returns>
        public static float NextSingle(float minValue, float maxValue)
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return Map(inst.NextSingle(), 0.0f, 1.0f, minValue, maxValue);
        }

        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to 0.0, and less than <paramref name="maxValue"/>
        /// </summary>
        /// <returns>A single-precision floating point number that is greater than or equal to 0.0 and less than <paramref name="maxValue"/></returns>
        public static float NextSingle(float maxValue)
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return inst.NextSingle() * maxValue;
        }
    }
}
