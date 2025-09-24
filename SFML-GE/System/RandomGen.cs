using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_GE.System
{
    /// <summary>
    /// A Thread Safe class for various random generation based methods.
    /// Made for games not for cryptography! 
    /// </summary>
    public static class RandomGen
    {
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
            return Random.Shared.NextSingle();
        }

        /// <summary>
        /// Returns a random non-negative integer
        /// </summary>
        /// <returns>A 32-Bit signed integer that is greater than or equal to 0 and less than <see cref="int.MaxValue"/></returns>
        public static int Next()
        {
            return Random.Shared.Next();
        }

        /// <summary>
        /// Picks a random item from the given array.
        /// </summary>
        /// <typeparam name="T">the type in the array</typeparam>
        /// <param name="table">the table to pick from</param>
        /// <returns>a random item from that list</returns>
        public static T PickRandom<T>(T[] table)
        {
            return table[Next(table.Length)];
        }

        /// <summary>
        /// Picks a random item from the given list.
        /// </summary>
        /// <typeparam name="T">the type in the list</typeparam>
        /// <param name="table">the list to pick from</param>
        /// <returns>a random item from that list</returns>
        public static T PickRandom<T>(List<T> table)
        {
            return table[Next(table.Count)];
        }

        /// <summary>
        /// returns a random integer that is within a specified range
        /// </summary>
        /// <param name="minValue">the lowest (inclusive) number of the random integer</param>
        /// <param name="maxValue">the highest (exclusive) number of the random integer</param>
        /// <returns>A 32-bit signed integer greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/></returns>
        public static int Next(int minValue, int maxValue)
        {
            return Random.Shared.Next(minValue, maxValue);
        }

        /// <summary>
        /// Returns a random integer that is greater than or equal to 0, and less than <paramref name="maxValue"/>
        /// </summary>
        /// <returns>A 32-bit signed integer that is greater than or equal to 0 and less than <paramref name="maxValue"/></returns>
        public static int Next(int maxValue)
        {
            return Random.Shared.Next(0, maxValue);
        }

        /// <summary>
        /// returns a random float that is within a specified range
        /// </summary>
        /// <param name="minValue">the lowest (inclusive) number of the random float</param>
        /// <param name="maxValue">the highest (exclusive) number of the random float</param>
        /// <returns>A 32-bit signed integer greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/></returns>
        public static float NextSingle(float minValue, float maxValue)
        {
            return Map(Random.Shared.NextSingle(), 0.0f, 1.0f, minValue, maxValue);
        }

        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to 0.0, and less than <paramref name="maxValue"/>
        /// </summary>
        /// <returns>A single-precision floating point number that is greater than or equal to 0.0 and less than <paramref name="maxValue"/></returns>
        public static float NextSingle(float maxValue)
        {
            return Random.Shared.NextSingle() * maxValue;
        }


        /// <summary>
        /// Returns a random true or false bool.
        /// </summary>
        /// <returns>A bool that is either true or false.</returns>
        public static bool NextBool()
        {
            return Random.Shared.Next(0, 2) == 0;
        }

        /// <summary>
        /// Returns a <see cref="Vector2"/> with X and Y each in a random range of (0.0 - 1.0)
        /// </summary>
        /// <returns></returns>
        public static Vector2 NextVector()
        {
            return new Vector2(NextSingle(), NextSingle());
        }

        /// <summary>
        /// Returns a <see cref="Vector2"/> with X and Y each in a random range of (<paramref name="min"/> - <paramref name="max"/>)
        /// </summary>
        /// <returns></returns>
        public static Vector2 NextVector(float min, float max)
        {
            return new Vector2(NextSingle(min, max), NextSingle(min, max));
        }

        /// <summary>
        /// Returns a <see cref="Vector2"/> with X in a range of (<paramref name="minX"/> - <paramref name="maxX"/>) and Y in a range of (<paramref name="minY"/> and <paramref name="maxY"/>)
        /// </summary>
        /// <returns></returns>
        public static Vector2 NextVector(float minX, float maxX, float minY, float maxY)
        {
            return new Vector2(NextSingle(minX, maxX), NextSingle(minY, maxY));
        }

        /// <summary>
        /// Returns a Vector2 facing in a random normalized direction, from -1 to 1 in both X and Y
        /// </summary>
        /// <returns></returns>
        public static Vector2 NextVectorDirection()
        {
            // rotating a 1 length vector here as im *fairly* sure the result is still normalized while not using sqrt (because sqrt is evil and slow)
            return Vector2.Rotate(new Vector2(1, 0), RandomGen.NextSingle(360));
        }
    }
}
