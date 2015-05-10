/* Micropolis.Graph.cs
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
    /// Partial Class Containing the content of graph.cpp
    /// </summary>
    public partial class Micropolis
    {
        public bool HistoryInitialized { get; private set; }
        public int Graph10Max { get; private set; }
        public int Graph120Max { get; private set; }

        public char[] DrawMonth(short[] hist, float scale)
        {
            var result = new char[120];
            int val, x;

            for (x = 0; x < 120; x++)
            {
                val = (int)(hist[x] * scale);
                result[119 - x] = (char)Utilities.Restrict(val, 0, 255);
            }
            return result;
        }

        public void ChangeCensus() { CensusChanged = true; }

        public void GraphDoer()
        {
            if (CensusChanged)
            {
                Callback("update", "s", "history");
                CensusChanged = false;
            }
        }

        public void InitGraphs()
        {
            if (!HistoryInitialized)
            {
                HistoryInitialized = true;
            }
        }

        public void InitGraphMax() {
            int x;

            ResHist10Max = 0;
            ComHist10Max = 0;
            IndHist10Max = 0;

            for (x = 118; x >= 0; x--)
            {

                if (ResHist[x] < 0)
                {
                    ResHist[x] = 0;
                }
                if (ComHist[x] < 0)
                {
                    ComHist[x] = 0;
                }
                if (IndHist[x] < 0)
                {
                    IndHist[x] = 0;
                }

                ResHist10Max = Math.Max(ResHist10Max, ResHist[x]);
                ComHist10Max = Math.Max(ComHist10Max, ComHist[x]);
                IndHist10Max = Math.Max(IndHist10Max, IndHist[x]);

            }

            Graph10Max = (short)Math.Max(ResHist10Max, Math.Max(ComHist10Max, IndHist10Max));

            ResHist120Max = 0;
            ComHist120Max = 0;
            IndHist120Max = 0;

            for (x = 238; x >= 120; x--)
            {

                if (ResHist[x] < 0)
                {
                    ResHist[x] = 0;
                }
                if (ComHist[x] < 0)
                {
                    ComHist[x] = 0;
                }
                if (IndHist[x] < 0)
                {
                    IndHist[x] = 0;
                }

                ResHist10Max = Math.Max(ResHist10Max, ResHist[x]);
                ComHist10Max = Math.Max(ComHist10Max, ComHist[x]);
                IndHist10Max = Math.Max(IndHist10Max, IndHist[x]);

            }

            Graph120Max = (short)Math.Max(ResHist120Max, Math.Max(ComHist120Max, IndHist120Max));
        }

        public void GetHistoryRange(HistoryType historyType, HistoryScale historyScale, ref short minValResult, ref short maxValResult) {
            if (historyType < 0 || historyType >= HistoryType.Count || historyScale < 0 || historyScale >= HistoryScale.Count)
            {
                minValResult = 0;
                maxValResult = 0;
                return;
            }

            short[] history = null;
            switch (historyType)
            {
                case HistoryType.Res:
                    history = ResHist;
                    break;
                case HistoryType.Com:
                    history = ComHist;
                    break;
                case HistoryType.Ind:
                    history = IndHist;
                    break;
                case HistoryType.Money:
                    history = MoneyHist;
                    break;
                case HistoryType.Crime:
                    history = CrimeHist;
                    break;
                case HistoryType.Pollution:
                    history = PollutionHist;
                    break;
                default:
                    //NOT_REACHED();
                    break;
            }

            int offset = 0;
            switch (historyScale)
            {
                case HistoryScale.Short:
                    offset = 0;
                    break;
                case HistoryScale.Long:
                    offset = 120;
                    break;
                default:
                    //NOT_REACHED();
                    break;
            }

            short minVal = 32000;
            short maxVal = -32000;

            for (int i = 0; i < Constants.HistoryCount; i++)
            {
                short val = history[i + offset];

                minVal = Math.Min(val, minVal);
                maxVal = Math.Max(val, maxVal);
            }

            minValResult = minVal;
            maxValResult = maxVal;
        }

        public short GetHistory(HistoryType historyType, HistoryScale historyScale, int historyIndex) {
            if (historyType < 0 || historyType >= HistoryType.Count
                || historyScale < 0 || historyScale >= HistoryScale.Count
                || historyIndex < 0 || historyIndex >= Constants.HistoryCount)
            {
                return 0;
            }

            short[] history = null;
            switch (historyType)
            {
                case HistoryType.Res:
                    history = ResHist;
                    break;
                case HistoryType.Com:
                    history = ComHist;
                    break;
                case HistoryType.Ind:
                    history = IndHist;
                    break;
                case HistoryType.Money:
                    history = MoneyHist;
                    break;
                case HistoryType.Crime:
                    history = CrimeHist;
                    break;
                case HistoryType.Pollution:
                    history = PollutionHist;
                    break;
                default:
                    //NOT_REACHED();
                    break;
            }

            int offset = 0;
            switch (historyScale)
            {
                case HistoryScale.Short:
                    offset = 0;
                    break;
                case HistoryScale.Long:
                    offset = 120;
                    break;
                default:
                    //NOT_REACHED();
                    break;
            }

            short result = history[historyIndex + offset];

            return result;
        }

        public void SetHistory(HistoryType historyType, HistoryScale historyScale, int historyIndex, short historyValue)
        {
            if (historyType < 0 || historyType >= HistoryType.Count
                || historyScale < 0 || historyScale >= HistoryScale.Count
                || historyIndex < 0 || historyIndex >= Constants.HistoryCount)
            {
                return;
            }

            short[] history = null;
            switch (historyType)
            {
                case HistoryType.Res:
                    history = ResHist;
                    break;
                case HistoryType.Com:
                    history = ComHist;
                    break;
                case HistoryType.Ind:
                    history = IndHist;
                    break;
                case HistoryType.Money:
                    history = MoneyHist;
                    break;
                case HistoryType.Crime:
                    history = CrimeHist;
                    break;
                case HistoryType.Pollution:
                    history = PollutionHist;
                    break;
                default:
                    //NOT_REACHED();
                    break;
            }

            int offset = 0;
            switch (historyScale)
            {
                case HistoryScale.Short:
                    offset = 0;
                    break;
                case HistoryScale.Long:
                    offset = 120;
                    break;
                default:
                    //NOT_REACHED();
                    break;
            }

            history[historyIndex + offset] = historyValue;
        }
    }
}
