/* Utilities.cs
 *
 * MicropolisSharp. This library is a port of the original code 
 *   (http://wiki.laptop.org/go/Micropolis) to C#/.Net. See the README 
 * for more details.
 *
 * If you need assistance with the Port please raise an issue on GitHub
 *   (https://github.com/lloydsparkes/MicropolisSharp/issues)
 * 
 * The Original code is - Copyright (C) 1989 - 2007 Electronic Arts Inc.  
 * If you need assistance with this program, you may contact:
 *   http://wiki.laptop.org/go/Micropolis  or email  micropolis@laptop.org.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or (at
 * your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.  You should have received a
 * copy of the GNU General Public License along with this program.  If
 * not, see <http://www.gnu.org/licenses/>.
 *
 *             ADDITIONAL TERMS per GNU GPL Section 7
 *
 * No trademark or publicity rights are granted.  This license does NOT
 * give you any right, title or interest in the trademark SimCity or any
 * other Electronic Arts trademark.  You may not distribute any
 * modification of this program using the trademark SimCity or claim any
 * affliation or association with Electronic Arts Inc. or its employees.
 *
 * Any propagation or conveyance of this program must include this
 * copyright notice and these terms.
 *
 * If you convey this program (or any modifications of it) and assume
 * contractual liability for the program to recipients of it, you agree
 * to indemnify Electronic Arts for any liability that those contractual
 * assumptions impose on Electronic Arts.
 *
 * You may not misrepresent the origins of this program; modified
 * versions of the program must be marked as such and not identified as
 * the original program.
 *
 * This disclaimer supplements the one included in the General Public
 * License.  TO THE FULLEST EXTENT PERMISSIBLE UNDER APPLICABLE LAW, THIS
 * PROGRAM IS PROVIDED TO YOU "AS IS," WITH ALL FAULTS, WITHOUT WARRANTY
 * OF ANY KIND, AND YOUR USE IS AT YOUR SOLE RISK.  THE ENTIRE RISK OF
 * SATISFACTORY QUALITY AND PERFORMANCE RESIDES WITH YOU.  ELECTRONIC ARTS
 * DISCLAIMS ANY AND ALL EXPRESS, IMPLIED OR STATUTORY WARRANTIES,
 * INCLUDING IMPLIED WARRANTIES OF MERCHANTABILITY, SATISFACTORY QUALITY,
 * FITNESS FOR A PARTICULAR PURPOSE, NONINFRINGEMENT OF THIRD PARTY
 * RIGHTS, AND WARRANTIES (IF ANY) ARISING FROM A COURSE OF DEALING,
 * USAGE, OR TRADE PRACTICE.  ELECTRONIC ARTS DOES NOT WARRANT AGAINST
 * INTERFERENCE WITH YOUR ENJOYMENT OF THE PROGRAM; THAT THE PROGRAM WILL
 * MEET YOUR REQUIREMENTS; THAT OPERATION OF THE PROGRAM WILL BE
 * UNINTERRUPTED OR ERROR-FREE, OR THAT THE PROGRAM WILL BE COMPATIBLE
 * WITH THIRD PARTY SOFTWARE OR THAT ANY ERRORS IN THE PROGRAM WILL BE
 * CORRECTED.  NO ORAL OR WRITTEN ADVICE PROVIDED BY ELECTRONIC ARTS OR
 * ANY AUTHORIZED REPRESENTATIVE SHALL CREATE A WARRANTY.  SOME
 * JURISDICTIONS DO NOT ALLOW THE EXCLUSION OF OR LIMITATIONS ON IMPLIED
 * WARRANTIES OR THE LIMITATIONS ON THE APPLICABLE STATUTORY RIGHTS OF A
 * CONSUMER, SO SOME OR ALL OF THE ABOVE EXCLUSIONS AND LIMITATIONS MAY
 * NOT APPLY TO YOU.
 */

using System;

namespace MicropolisSharp
{
    /// <summary>
    /// Some utility methods, and Extension Methods, that are useful when dealing with the code
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Replacement for clamp from micropolis.h
        /// 
        /// Returns a value which is between lower and upper.
        /// </summary>
        /// <typeparam name="T">Type of the Value being "Restricted"</typeparam>
        /// <param name="value">The value to be restricted</param>
        /// <param name="lower">The lower bound (Inclusive)</param>
        /// <param name="upper">The upper bound (Inclusive)</param>
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

        /// <summary>
        /// IsTrue - If a Value != 0 return true, else false.
        /// 
        /// Used to replace C type functionality if(65) {} which doesnt work in C
        /// 
        /// </summary>
        /// <typeparam name="T">Type of the value to be tested for true</typeparam>
        /// <param name="value">The value to test</param>
        /// <returns>bool, true if value != 0</returns>
        public static bool IsTrue<T>(this T value)
            where T : IComparable
        {
            if(value.CompareTo(default(T)) != 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// IsFalse - If a Value == 0 return true, else false.
        /// 
        /// Used to replace C type functionality if(!65) {} which doesnt work in C
        /// 
        /// </summary>
        /// <typeparam name="T">Type of the value to be tested for false</typeparam>
        /// <param name="value">The value to test</param>
        /// <returns>bool, true if value == 0</returns>
        public static bool IsFalse<T>(this T value)
            where T : IComparable
        {
            if (value.CompareTo(default(T)) == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Turn's a bool back into a number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static short ToShort(this bool value)
        {
            return value ? (short)1 : (short)0;
        }
    }
}
