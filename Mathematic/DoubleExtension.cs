using System;

namespace Mathematic
{
    public static class DoubleExtension
    {
        // ******************************************************************
        // Base on Hans Passant Answer on:
        // http://stackoverflow.com/questions/2411392/double-epsilon-for-equality-greater-than-less-than-less-than-or-equal-to-gre

        /// <summary>
        /// Compare two double taking in account the double precision potential error.
        /// Take care: truncation errors accumulate on calculation. More you do, more you should increase the epsilon.
        public static bool AboutEquals(this double value1, double value2)
        {
            if (double.IsPositiveInfinity(value1))
                return double.IsPositiveInfinity(value2);

            if (double.IsNegativeInfinity(value1))
                return double.IsNegativeInfinity(value2);

            if (double.IsNaN(value1))
                return double.IsNaN(value2);

            double epsilon = Math.Max(Math.Abs(value1), Math.Abs(value2)) * 1E-15;
            return Math.Abs(value1 - value2) <= epsilon;
        }

        // ******************************************************************
        // Base on Hans Passant Answer on:
        // http://stackoverflow.com/questions/2411392/double-epsilon-for-equality-greater-than-less-than-less-than-or-equal-to-gre

        /// <summary>
        /// Compare two double taking in account the double precision potential error.
        /// Take care: truncation errors accumulate on calculation. More you do, more you should increase the epsilon.
        /// You get really better performance when you can determine the contextual epsilon first.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="precalculatedContextualEpsilon"></param>
        /// <returns></returns>
        public static bool AboutEquals(this double value1, double value2, double precalculatedContextualEpsilon)
        {
            if (double.IsPositiveInfinity(value1))
                return double.IsPositiveInfinity(value2);

            if (double.IsNegativeInfinity(value1))
                return double.IsNegativeInfinity(value2);

            if (double.IsNaN(value1))
                return double.IsNaN(value2);

            return Math.Abs(value1 - value2) <= precalculatedContextualEpsilon;
        }

        // ******************************************************************
        public static double GetContextualEpsilon(this double biggestPossibleContextualValue)
        {
            return biggestPossibleContextualValue * 1E-15;
        }

        // ******************************************************************
        /// <summary>
        /// Mathlab equivalent
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static double Mod(this double dividend, double divisor)
        {
            return dividend - System.Math.Floor(dividend / divisor) * divisor;
        }

        // ******************************************************************
    }
}
