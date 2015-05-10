using System;

namespace MicropolisSharp
{
    public static class Utilities
    {
        /// <summary>
        /// Replacement for clamp from micropolis.h
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static T Restrict<T>(T value, T lower, T upper)
            where T : IComparable
        {
            if(value.CompareTo(lower) < 0)
            {
                return lower;
            }
            if(value.CompareTo(upper) > 0)
            {
                return upper;
            }
            return value;
        }

        public static bool IsTrue<T>(this T value)
            where T : IComparable
        {
            if(value.CompareTo(0) != 0)
            {
                return true;
            }
            return false;
        }

        public static bool IsFalse<T>(this T value)
            where T : IComparable
        {
            if (value.CompareTo(0) == 0)
            {
                return true;
            }
            return false;
        }

        public static short ToShort(this bool value)
        {
            return value ? (short)1 : (short)0;
        }
    }
}
