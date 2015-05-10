/* Micropolis.Budget.cs
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
    /// Partial Class Containing the content of budget.cpp
    /// </summary>
    public partial class Micropolis
    {
        public void InitFundingLevel()
        {
            FirePercentage = 1.0f;
            FireValue = 0;
            PolicePercentage = 1.0f;
            PoliceValue = 0;
            RoadPercentage = 1.0f;
            RoadValue = 0;
            MustDrawBudget = 1;
        }

        public void DoBudget()
        {
            DoBudgetNow(false);
        }

        public void DoBudgetFromMenu()
        {
            DoBudgetNow(true);
        }

        public void DoBudgetNow(bool fromMenu)
        {
            long fireInt = (int)(FireFund * FirePercentage);
            long policeInt = (int)(PoliceFund * PolicePercentage);
            long roadInt = (int)(RoadFund * RoadPercentage);

            long total = fireInt + policeInt + roadInt;

            long yumDuckets = TaxFund + TotalFunds;

            if (yumDuckets > total)
            {

                // Enough yumDuckets to fully fund fire, police and road.

                FireValue = fireInt;
                PoliceValue = policeInt;
                RoadValue = roadInt;

                /// @todo Why are we not subtracting from yumDuckets what we
                /// spend, like the code below is doing?

            }
            else if (total > 0)
            {
                //TODO: Reenable Asserts
                //assert(yumDuckets <= total);

                // Not enough yumDuckets to fund everything.
                // First spend on roads, then on fire, then on police.

                if (yumDuckets > roadInt)
                {

                    // Enough yumDuckets to fully fund roads.

                    RoadValue = roadInt;
                    yumDuckets -= roadInt;

                    if (yumDuckets > fireInt)
                    {

                        // Enough yumDuckets to fully fund fire.

                        FireValue = fireInt;
                        yumDuckets -= fireInt;

                        if (yumDuckets > policeInt)
                        {

                            // Enough yumDuckets to fully fund police.
                            // Hey what are we doing here? Should never get here.
                            // We tested for yumDuckets > total above
                            // (where total = fireInt + policeInt + roadInt),
                            // so this should never happen.

                            PoliceValue = policeInt;
                            yumDuckets -= policeInt;

                        }
                        else
                        {

                            // Fuly funded roads and fire.
                            // Partially fund police.

                            PoliceValue = yumDuckets;

                            if (yumDuckets > 0)
                            {

                                // Scale back police percentage to available cash.

                                PolicePercentage = ((float)yumDuckets) / ((float)PoliceFund);

                            }
                            else
                            {

                                // Exactly nothing left, so scale back police percentage to zero.

                                PolicePercentage = 0.0f;

                            }

                        }

                    }
                    else
                    {

                        // Not enough yumDuckets to fully fund fire.

                        FireValue = yumDuckets;

                        // No police after funding roads and fire.

                        PoliceValue = 0;
                        PolicePercentage = 0.0f;

                        if (yumDuckets > 0)
                        {

                            // Scale back fire percentage to available cash.

                            FirePercentage =
                                ((float)yumDuckets) / ((float)FireFund);

                        }
                        else
                        {

                            // Exactly nothing left, so scale back fire percentage to zero.

                            FirePercentage = 0.0f;

                        }

                    }

                }
                else
                {

                    // Not enough yumDuckets to fully fund roads.

                    RoadValue = yumDuckets;

                    // No fire or police after funding roads.

                    FireValue = 0;
                    PoliceValue = 0;
                    FirePercentage = 0.0f;
                    PolicePercentage = 0.0f;

                    if (yumDuckets > 0)
                    {

                        // Scale back road percentage to available cash.

                        RoadPercentage = ((float)yumDuckets) / ((float)RoadFund);

                    }
                    else
                    {

                        // Exactly nothing left, so scale back road percentage to zero.

                        RoadPercentage = 0.0f;

                    }

                }

            }
            else
            {
                //TODO: Reenable Assets
                //assert(yumDuckets == total);
                //assert(total == 0);

                // Zero funding, so no values but full percentages.

                FireValue = 0;
                PoliceValue = 0;
                RoadValue = 0;
                FirePercentage = 1.0f;
                PolicePercentage = 1.0f;
                RoadPercentage = 1.0f;

            }

            noMoney:

            if (!AutoBudget || fromMenu)
            {

                // FIXME: This might have blocked on the Mac, but now it's asynchronous.
                // Make sure the stuff we do just afterwards is intended to be done immediately
                // and is not supposed to wait until after the budget dialog is dismissed.
                // Otherwise don't do it after this and arrange for it to happen when the
                // modal budget dialog is dismissed.
                ShowBudgetWindowAndStartWaiting();

                // FIXME: Only do this AFTER the budget window is accepted.

                if (!fromMenu)
                {

                    FireSpend = FireValue;
                    PoliceSpend = PoliceValue;
                    RoadSpend = RoadValue;

                    total = FireSpend + PoliceSpend + RoadSpend;

                    long moreDough = (long)(TaxFund - total);
                    Spend(-moreDough);

                }

                MustDrawBudget = 1;
                DoUpdateHeads();

            }
            else
            { /* autoBudget & !fromMenu */

                // FIXME: Not sure yumDuckets is the right value here. It gets the
                // amount spent subtracted from it above in some cases, but not if
                // we are fully funded. I think we want to use the original value
                // of yumDuckets, which is taxFund + totalFunds.

                if (yumDuckets > total)
                {

                    long moreDough = (long)(TaxFund - total);
                    Spend(-moreDough);

                    FireSpend = FireFund;
                    PoliceSpend = PoliceFund;
                    RoadSpend = RoadFund;

                    MustDrawBudget = 1;
                    DoUpdateHeads();

                }
                else
                {

                    SetAutoBudget(false); /* force autobudget */
                    MustUpdateOptions = true;
                    SendMessage(GeneralMessages.MESSAGE_NO_MONEY, Constants.NoWhere, Constants.NoWhere, false, true);
                    goto noMoney;

                }

            }
        }

        public void UpdateBudget()
        {
            if (MustDrawBudget > 0)
            {
                Callback("update", "s", "budget");
                MustDrawBudget = 0;
            }
        }

        public void ShowBudgetWindowAndStartWaiting() { Callback("showBudgetAndWait", ""); }

        public void SetCityTax(short tax)
        {
            CityTax = tax;
            Callback("update", "s", "taxRate");
        }
    }
}
