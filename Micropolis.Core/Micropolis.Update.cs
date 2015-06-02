/* Micropolis.Update.cs
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

namespace MicropolisSharp
{
    /// <summary>
    /// Partial Class Containing the content of update.cpp
    /// </summary>
    public partial class Micropolis
    {
        public bool MustUpdateFunds { get; private set; }
        public bool MustUpdateOptions { get; private set; }
        public long CityTimeLast { get; private set; }
        public long CityYearLast { get; private set; }
        public long CityMonthLast { get; private set; }
        public long TotalFundsLast { get; private set; }
        public long ResLast { get; private set; }
        public long ComLast { get; private set; }
        public long IndLast { get; private set; }

        public void DoUpdateHeads()
        {
            ShowValves();
            DoTimeStuff();
            ReallyUpdateFunds();
            UpdateOptions();
        }

        public void UpdateMaps() {
            InvalidateMaps();
            DoUpdateHeads();
        }

        public void UpdateGraphs()
        {
            ChangeCensus();
        }

        public void UpdateEvaluation()
        {
            ChangeEval();
        }

        public void UpdateHeads()
        {
            MustUpdateFunds = true;
            ValveFlag = true;
            CityTimeLast = CityYearLast = CityMonthLast = TotalFundsLast =
              ResLast = ComLast = IndLast = -999999;
            DoUpdateHeads();
        }

        /// <summary>
        /// Set a flag that the funds display is out of date.
        /// </summary>
        public void UpdateFunds()
        {
            MustUpdateFunds = true;
        }

        public void ReallyUpdateFunds()
        {
            if (!MustUpdateFunds)
            {
                return;
            }

            MustUpdateFunds = false;

            if (TotalFunds != TotalFundsLast)
            {
                TotalFundsLast = TotalFunds;

                Callback("update", "s", "funds");
            }
        }

        public void DoTimeStuff()
        {
            UpdateDate();
        }

        /// <summary>
        /// TODO: Message is Wrong
        /// </summary>
        public void UpdateDate()
        {
            int megalinium = 1000000;

            CityTimeLast = CityTime >> 2;

            CityYear = ((int)CityTime / 48) + (int)StartingYear;
            CityMonth = ((int)CityTime % 48) >> 2;

            if (CityYear >= megalinium)
            {
                SetYear(StartingYear);
                CityYear = StartingYear;
                SendMessage(GeneralMessages.MESSAGE_NOT_ENOUGH_POWER, Constants.NoWhere, Constants.NoWhere, true, false);

            }

            if ((CityYearLast != CityYear) ||
                (CityMonthLast != CityMonth))
            {

                CityYearLast = CityYear;
                CityMonthLast = CityMonth;

                Callback("update", "s", "date");
            }
        }

        public void ShowValves()
        {
            if (ValveFlag)
            {
                DrawValve();
                ValveFlag = false;
            }
        }

        public void DrawValve()
        {
            float r, c, i;

            r = ResValve;

            if (r < -1500)
            {
                r = -1500;
            }

            if (r > 1500)
            {
                r = 1500;
            }

            c = ComValve;

            if (c < -1500)
            {
                c = -1500;
            }

            if (c > 1500)
            {
                c = 1500;
            }

            i = IndValve;

            if (i < -1500)
            {
                i = -1500;
            }

            if (i > 1500)
            {
                i = 1500;
            }

            if ((r != ResLast) ||
                (c != ComLast) ||
                (i != IndLast))
            {

                ResLast = (int)r;
                ComLast = (int)c;
                IndLast = (int)i;

                SetDemand(r, c, i);
            }
        }

        public void SetDemand(float r, float c, float i)
        {
            Callback("update", "s", "demand");
        }

        public void UpdateOptions()
        {
            if (MustUpdateOptions)
            {
                MustUpdateOptions = false;
                Callback("update", "s", "options");
            }
        }

        public void UpdateUserInterface()
        {
            /// @todo Send all pending update messages to the user interface.

            // city: after load file, load scenario, or generate city
            // map: when small overall map changes
            // editor: when large close-up map changes
            // graph: when graph changes
            // evaluation: when evaluation changes
            // budget: when budget changes
            // date: when date changes
            // funds: when funds change
            // demand: when demand changes
            // level: when level changes
            // speed: when speed changes
            // delay: when delay changes
            // option: when options change
        }

    }
}
