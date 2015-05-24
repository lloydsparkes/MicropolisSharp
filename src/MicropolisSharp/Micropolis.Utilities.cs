/* Micropolis.Utilities.cs
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
using MicropolisSharp.Types;
using System;

namespace MicropolisSharp
{
    /// <summary>
    /// Partial Class Containing the content of utilities.cpp
    /// </summary>
    public partial class Micropolis
    {
        /// <summary>
        /// Pause a simulation
        /// </summary>
        public void Pause()
        {
            if (!SimPaused)
            {
                SimPausedSpeed = SimSpeedMeta;
                SetSpeed(0);
                SimPaused = true;
            }

            // Call back even if the state did not change.
            Callback("update", "s", "paused");
        }

        /// <summary>
        /// Resume simulation after pausing it
        /// </summary>
        public void Resume()
        {
            if (SimPaused)
            {
                SimPaused = false;
                SetSpeed((short)SimPausedSpeed);
            }

            // Call back even if the state did not change.
            Callback("update", "s", "paused");
        }

        public void SetSpeed(short speed)
        {
            if (speed < 0)
            {
                speed = 0;
            }
            else if (speed > 3)
            {
                speed = 3;
            }

            SimSpeedMeta = speed;

            if (SimPaused)
            {
                SimPausedSpeed = SimSpeedMeta;
                speed = 0;
            }

            SimSpeed = speed;

            Callback("update", "s", "speed");
        }

        public void SetPasses(int passes)
        {
            SimPasses = passes;
            SimPass = 0;
            Callback("update", "s", "passes");
        }

        /// <summary>
        /// Set the game level and initial funds.
        /// </summary>
        /// <param name="level"></param>
        public void SetGameLevelFunds(Levels level)
        {
            switch (level)
            {

                default:
                case Levels.Easy:
                    SetFunds(20000);
                    SetGameLevel(Levels.Easy);
                    break;

                case Levels.Medium:
                    SetFunds(10000);
                    SetGameLevel(Levels.Medium);
                    break;

                case Levels.Hard:
                    SetFunds(5000);
                    SetGameLevel(Levels.Hard);
                    break;

            }
        }

        /// <summary>
        /// Set/change the game level.
        /// </summary>
        /// <param name="level"></param>
        public void SetGameLevel(Levels level)
        {
            //TODO: Reenabled Asserts
            //assert(level >= Levels.First && level <= Levels.Last);
            GameLevel = level;
            UpdateGameLevel();
        }

        /// <summary>
        /// Report to the front-end that a new game level has been set.
        /// </summary>
        public void UpdateGameLevel()
        {
            Callback("update", "s", "gameLevel");
        }

        /// <summary>
        /// Set the name of the city.
        /// </summary>
        /// <param name="name"></param>
        public void SetCityName(string name)
        {
            string cleanName = "";

            int i;
            int n = name.Length;
            for (i = 0; i < n; i++)
            {
                char ch = name[i];
                if (!Char.IsDigit(ch))
                {
                    ch = '_';
                }
                cleanName += ch;
            }

            SetCleanCityName(cleanName);
        }

        /// <summary>
        /// Set the name of the city.
        /// </summary>
        /// <param name="name"></param>
        public void SetCleanCityName(string name)
        {
            CityName = name;

            Callback("update", "s", "cityName");
        }

        public void SetYear(int year)
        {     // Must prevent year from going negative, since it screws up the non-floored modulo arithmetic.
            if (year < StartingYear)
            {
                year = StartingYear;
            }

            year = (int)((year - StartingYear) - (CityTime / 48));
            CityTime += year * 48;
            DoTimeStuff();
        }

        public int CurrentYear()
        {
            return (int)((CityTime / 48) + StartingYear);
        }

        /// <summary>
        /// Notify the user interface to start a new game.
        /// </summary>
        public void DoNewGame()
        {
            Callback("newGame", "");
        }

        /// <summary>
        /// set the enableDisasters flag, and set the flag to update the user interface.
        /// </summary>
        /// <param name="value"></param>
        public void SetEnableDisasters(bool value)
        {
            EnableDisasters = value;
            MustUpdateOptions = true;
        }

        /// <summary>
        /// Set the auto-budget to the given value.
        /// </summary>
        /// <param name="value"></param>
        public void SetAutoBudget(bool value)
        {
            AutoBudget = value;
            MustUpdateOptions = true;
        }

        /// <summary>
        /// Set the autoBulldoze flag to the given value,
        /// and set the mustUpdateOptions flag to update
        /// the user interface.
        /// </summary>
        /// <param name="value"></param>
        public void SetAutoBulldoze(bool value)
        {
            AutoBulldoze = value;
            MustUpdateOptions = true;
        }

        /// <summary>
        /// Set the autoGoto flag to the given value,
        /// and set the mustUpdateOptions flag to update
        /// the user interface.
        /// </summary>
        /// <param name="value"></param>
        public void SetAutoGoTo(bool value)
        {
            AutoGoTo = value;
            MustUpdateOptions = true;
        }

        /// <summary>
        /// Set the enableSound flag to the given value,
        /// and set the mustUpdateOptions flag to update
        /// the user interface.
        /// </summary>
        /// <param name="value"></param>
        public void SetEnableSound(bool value)
        {
            EnableSound = value;
            MustUpdateOptions = true;
        }

        /// <summary>
        /// Set the doAnimation flag to the given value,
        /// and set the mustUpdateOptions flag to update
        /// the user interface.
        /// </summary>
        /// <param name="value"></param>
        public void SetDoAnimation(bool value)
        {
            DoAnimation = value;
            MustUpdateOptions = true;
        }

        /// <summary>
        /// Set the doMessages flag to the given value,
        /// and set the mustUpdateOptions flag to update
        /// the user interface.
        /// </summary>
        /// <param name="value"></param>
        public void SetDoMessages(bool value)
        {
            DoMessages = value;
            MustUpdateOptions = true;
        }

        /// <summary>
        /// Set the doNotices flag to the given value,
        /// and set the mustUpdateOptions flag to update
        /// the user interface.
        /// </summary>
        /// <param name="value"></param>
        public void SetDoNotices(bool value)
        {
            DoNotices = value;
            MustUpdateOptions = true;
        }

        /// <summary>
        /// Return the residential, commercial and industrial
        /// development demands, as floating point numbers
        /// from -1 (lowest demand) to 1 (highest demand).
        /// </summary>
        /// <param name="resDemandResult"></param>
        /// <param name="comDemandResult"></param>
        /// <param name="indDemandResult"></param>
        public void GetDemands(ref float resDemandResult, ref float comDemandResult, ref float indDemandResult)
        {
            resDemandResult = (float)ResValve / (float)Constants.ResValveRange;
            comDemandResult = (float)ComValve / (float)Constants.ComValveRange;
            indDemandResult = (float)IndValve / (float)Constants.IndValveRange;
        }

        /// <summary>
        /// comefrom: drawTaxesCollected incBoxValue decBoxValue drawCurrentFunds drawActualBox updateFunds updateCurrentCost
        /// </summary>
        /// <param name="numStr"></param>
        /// <param name="dollarStr"></param>
        public void MakeDollarDecimalStr(string numStr, char[] dollarStr)
        {
            int leftMostSet;
            int numOfDigits;
            int numOfChars;
            int numOfCommas;
            int dollarIndex = 0;
            int numIndex = 0;
            int x;

            numOfDigits = (short)numStr.Length;

            if (numOfDigits == 1)
            {
                dollarStr[0] = '$';
                dollarStr[1] = numStr[0];
                dollarStr[2] = (char)0;
                return;
            }
            else if (numOfDigits == 2)
            {
                dollarStr[0] = '$';
                dollarStr[1] = numStr[0];
                dollarStr[2] = numStr[1];
                dollarStr[3] = (char)0;
                return;
            }
            else if (numOfDigits == 3)
            {
                dollarStr[0] = '$';
                dollarStr[1] = numStr[0];
                dollarStr[2] = numStr[1];
                dollarStr[3] = numStr[2];
                dollarStr[4] = (char)0;
            }
            else
            {
                leftMostSet = numOfDigits % 3;

                if (leftMostSet == 0)
                {
                    leftMostSet = 3;
                }

                numOfCommas = (numOfDigits - 1) / 3;

                /* add 1 for the dollar sign */
                numOfChars = numOfDigits + numOfCommas + 1;

                dollarStr[numOfChars] = (char)0;

                dollarStr[dollarIndex++] = '$';

                for (x = 1; x <= leftMostSet; x++)
                {
                    dollarStr[dollarIndex++] = numStr[numIndex++];
                }

                for (x = 1; x <= numOfCommas; x++)
                {
                    dollarStr[dollarIndex++] = ',';
                    dollarStr[dollarIndex++] = numStr[numIndex++];
                    dollarStr[dollarIndex++] = numStr[numIndex++];
                    dollarStr[dollarIndex++] = numStr[numIndex++];
                }

            }
        }
    }
}
