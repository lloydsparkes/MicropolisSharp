/* Micropolis.Evaluate.cs
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
    /// Partial Class Containing the content of evaluate.cpp
    /// </summary>
    public partial class Micropolis
    {
        public short CityYes { get; private set; }
        public int[] ProblemVotes { get; private set; }
        public int[] ProblemOrder { get; private set; }
        public long CityPopulation { get; private set; }
        public long CityPopDelta { get; private set; }
        public long CityAssessedValue { get; private set; }
        public CityClassification CityClassification { get; private set; }
        public short CityScore { get; private set; }
        public short CityScoreDelta { get; private set; }
        public short TrafficAverage { get; private set; }

        public void CityEvaluation()
        {
            if (TotalPop > 0)
            {
                int[] problemTable = new int[(int)CityVotingProblems.NumberOfProblems]; // Score for each problem, higher the more severe the problem is.
                for (int z = 0; z < (int)CityVotingProblems.NumberOfProblems; z++)
                {
                    problemTable[z] = 0;
                }

                GetAssessedValue();
                DoPopNum();
                DoProblems(problemTable);
                GetScore(problemTable);
                DoVotes();  // How well is the mayor doing?
                ChangeEval();
            }
            else
            {
                EvalInit();
                CityYes = 50; // No population => no voting. Let's say 50/50.
                ChangeEval();
            }
        }

        public void EvalInit()
        {
            CityYes = 0;
            CityPopulation = 0;
            CityPopDelta = 0;
            CityAssessedValue = 0;
            CityClassification = CityClassification.Village;
            CityScore = 500;
            CityScoreDelta = 0;
            for (int i = 0; i < (int)CityVotingProblems.NumberOfProblems; i++)
            {
                ProblemVotes[i] = 0;
            }
            for (int i = 0; i < (int)CityVotingProblems.CountOfProblemsToComplainAbout; i++)
            {
                ProblemOrder[i] = (int)CityVotingProblems.Count;
            }
        }

        public void DoScoreCard()
        {
            Callback("update", "s", "evaluation");

            // The user interface should pull these raw values out and format
            // them. The simulator core used to format them and push them out,
            // but the user interface should pull them out and format them
            // itself.

            // City Evaluation ${FormatYear(currentYear())}
            // Public Opinion
            //   Is the mayor doing a good job?
            //     Yes: ${FormatPercent(cityYes)}
            //     No: ${FormatPercent(100 - cityYes)}
            //   What are the worst problems?
            //     for i in range(0, CVP_PROBLEM_COMPLAINTS),
            //                 while problemOrder[i] < CVP_NUMPROBLEMS:
            //     ${probStr[problemOrder[i]]}:
            //                  ${FormatPercent(problemVotes[problemOrder[i]])}
            // Statistics
            //   Population: ${FormatNumber(cityPop)}
            //   Net Migration: ${FormatNumber(cityPopDelta)} (last year)
            //   Assessed Value: ${FormatMoney(cityAssessedValue))
            //   Category: ${cityClassStr[cityClass]}
            //   Game Level: ${cityLevelStr[gameLevel]}
        }

        public void ChangeEval()
        {
            EvalChanged = true;
        }

        public void ScoreDoer()
        {
            if (EvalChanged)
            {
                DoScoreCard();
                EvalChanged = false;
            }
        }

        public int CountProblems()
        {
            int i;
            for (i = 0; i < (int)CityVotingProblems.CountOfProblemsToComplainAbout; i++)
            {
                if (ProblemOrder[i] == (int)CityVotingProblems.NumberOfProblems)
                {
                    break;
                }
            }
            return i;
        }

        public int GetProblemNumber(int i)
        {
            if (i < 0 || i >= (int)CityVotingProblems.CountOfProblemsToComplainAbout || ProblemOrder[i] == (int)CityVotingProblems.NumberOfProblems)
            {
                return -1;
            }
            else
            {
                return ProblemOrder[i];
            }
        }

        public int GetProblemVotes(int i)
        {
            if (i < 0 || i >= (int)CityVotingProblems.CountOfProblemsToComplainAbout || ProblemOrder[i] == (int)CityVotingProblems.NumberOfProblems)
            {
                return -1;
            }
            else
            {
                return ProblemVotes[ProblemOrder[i]];
            }
        }

        private void GetAssessedValue() {
            long z;

            z = RoadTotal * 5;
            z += RailTotal * 10;
            z += PoliceStationPop * 1000;
            z += FireStationPop * 1000;
            z += HospitalPop * 400;
            z += StadiumPop * 3000;
            z += SeaportPop * 5000;
            z += AirportPop * 10000;
            z += CoalPowerPop * 3000;
            z += NuclearPowerPop * 6000;

            CityAssessedValue = z * 1000;
        }

        private void DoPopNum()
        {
            long oldCityPop = CityPopulation;

            CityPopulation = GetPopulation();

            if (oldCityPop == -1)
            {
                oldCityPop = CityPopulation;
            }

            CityPopDelta = CityPopulation - oldCityPop;
            CityClassification = GetCityClass(CityPopulation);
        }

        private long GetPopulation()
        {
            long pop = (ResPop + (ComPop + IndPop) * 8L) * 20L;
            return pop;
        }

        private CityClassification GetCityClass(long cityPopulation)
        {
            CityClassification cityClassification = CityClassification.Village;

            if (cityPopulation > 2000)
            {
                cityClassification = CityClassification.Town;
            }
            if (cityPopulation > 10000)
            {
                cityClassification = CityClassification.City;
            }
            if (cityPopulation > 50000)
            {
                cityClassification = CityClassification.Capital;
            }
            if (cityPopulation > 100000)
            {
                cityClassification = CityClassification.Metropolis;
            }
            if (cityPopulation > 500000)
            {
                cityClassification = CityClassification.Megalopolis;
            }

            return cityClassification;
        }

        private void DoProblems(int[] problemTable) {
            bool[] problemTaken = new bool[(int)CityVotingProblems.NumberOfProblems]; // Which problems are taken?

            for (int z = 0; z < (int)CityVotingProblems.NumberOfProblems; z++)
            {
                problemTaken[z] = false;
                problemTable[z] = 0;
            }

            problemTable[(int)CityVotingProblems.Crime] = CrimeAverage;                /* Crime */
            problemTable[(int)CityVotingProblems.Pollution] = PollutionAverage;            /* Pollution */
            problemTable[(int)CityVotingProblems.Housing] = LandValueAverage * 7 / 10;   /* Housing */
            problemTable[(int)CityVotingProblems.Taxes] = CityTax * 10;                /* Taxes */
            problemTable[(int)CityVotingProblems.Traffic] = GetTrafficAverage();         /* Traffic */
            problemTable[(int)CityVotingProblems.Unemployment] = GetUnemployment();           /* Unemployment */
            problemTable[(int)CityVotingProblems.Fire] = GetFireSeverity();           /* Fire */
            VoteProblems(problemTable);

            for (int z = 0; z < (int)CityVotingProblems.CountOfProblemsToComplainAbout; z++)
            {
                // Find biggest problem not taken yet
                int maxVotes = 0;
                int bestProblem = (int)CityVotingProblems.CountOfProblemsToComplainAbout;
                for (int i = 0; i < (int)CityVotingProblems.CountOfProblemsToComplainAbout; i++)
                {
                    if ((ProblemVotes[i] > maxVotes) && (!problemTaken[i]))
                    {
                        bestProblem = i;
                        maxVotes = ProblemVotes[i];
                    }
                }

                // bestProblem == CVP_NUMPROBLEMS means no problem found
                ProblemOrder[z] = bestProblem;
                if (bestProblem < (int)CityVotingProblems.CountOfProblemsToComplainAbout)
                {
                    problemTaken[bestProblem] = true;
                }
                // else: No problem found.
                //       Repeating the procedure will give the same result.
                //       Optimize by filling all remaining entries, and breaking out
            }
        }

        private void VoteProblems(int[] problemTable)
        {
            for (int z = 0; z < (int)CityVotingProblems.NumberOfProblems; z++)
            {
                ProblemVotes[z] = 0;
            }

            int problem = 0; // Problem to vote for
            int voteCount = 0; // Number of votes
            int loopCount = 0; // Number of attempts
            while (voteCount < 100 && loopCount < 600)
            {
                if (GetRandom(300) < problemTable[problem])
                {
                    ProblemVotes[problem]++;
                    voteCount++;
                }
                problem++;
                if (problem > (int)CityVotingProblems.NumberOfProblems)
                {
                    problem = 0;
                }
                loopCount++;
            }
        }

        private short GetTrafficAverage()
        {
            long trafficTotal;
            int x, y, count;

            trafficTotal = 0;
            count = 1;
            for (x = 0; x < Constants.WorldWidth; x += LandValueMap.BlockSize)
            {
                for (y = 0; y < Constants.WorldHeight; y += LandValueMap.BlockSize)
                {
                    if (LandValueMap.WorldGet(x, y) > 0)
                    {
                        trafficTotal += TrafficDensityMap.WorldGet(x, y);
                        count++;
                    }
                }
            }

            TrafficAverage = (short)((trafficTotal / count) * 2.4);

            return TrafficAverage;
        }

        private short GetUnemployment()
        {
            int b = (ComPop + IndPop) * 8;

            if (b == 0)
            {
                return 0;
            }

            // Ratio total people / working. At least 1.
            float r = ((float)ResPop) / b;

            b = (short)((r - 1) * 255); // (r - 1) is the fraction unemployed people
            return (short)Math.Min(b, (short)255);
        }

        private short GetFireSeverity() { return (short)Math.Min(FirePop * 5, 255); }

        private void GetScore(int[] problemTable) {
            int x, z;
            short cityScoreLast;

            cityScoreLast = CityScore;
            x = 0;

            for (z = 0; z < (int)CityVotingProblems.NumberOfProblems; z++)
            {
                x += problemTable[z];       /* add 7 probs */
            }

            /**
             * @todo Should this expression depend on CVP_NUMPROBLEMS?
             */
            x = x / 3;                    /* 7 + 2 average */
            x = Math.Min(x, 256);

            z = Utilities.Restrict((256 - x) * 4, 0, 1000);

            if (ResCap)
            {
                z = (int)(z * .85);
            }

            if (ComCap)
            {
                z = (int)(z * .85);
            }

            if (IndCap)
            {
                z = (int)(z * .85);
            }

            if (RoadEffect < Constants.MaxRoadEffect)
            {
                z -= (int)(Constants.MaxRoadEffect - RoadEffect);
            }

            if (PoliceEffect < Constants.MaxPoliceStationEffect)
            {
                // 10.0001 = 10000.1 / 1000, 1/10.0001 is about 0.1
                z = (int)(z * (0.9 + (PoliceEffect / (10.0001 * Constants.MaxPoliceStationEffect))));
            }

            if (FireEffect < Constants.MaxFireStationEffect)
            {
                // 10.0001 = 10000.1 / 1000, 1/10.0001 is about 0.1
                z = (int)(z * (0.9 + (FireEffect / (10.0001 * Constants.MaxFireStationEffect))));
            }

            if (ResValve < -1000)
            {
                z = (int)(z * .85);
            }

            if (ComValve < -1000)
            {
                z = (int)(z * .85);
            }

            if (IndValve < -1000)
            {
                z = (int)(z * .85);
            }

            float SM = 1.0f;
            if (CityPopulation == 0 || CityPopDelta == 0)
            {
                SM = 1.0f; // there is nobody or no migration happened

            }
            else if (CityPopDelta == CityPopulation)
            {
                SM = 1.0f; // city sprang into existence or doubled in size

            }
            else if (CityPopDelta > 0)
            {
                SM = ((float)CityPopDelta / CityPopulation) + 1.0f;

            }
            else if (CityPopDelta < 0)
            {
                SM = 0.95f + ((float)CityPopDelta / (CityPopulation - CityPopDelta));
            }

            z = (int)(z * SM);
            z = z - GetFireSeverity() - CityTax; // dec score for fires and taxes

            float TM = UnpoweredZoneCount + PoweredZoneCount;   // dec score for unpowered zones
            if (TM > 0.0)
            {
                z = (int)(z * (float)(PoweredZoneCount / TM));
            }
            else
            {
            }

            z = Utilities.Restrict(z, 0, 1000);

            CityScore = (short)((CityScore + z) / 2);

            CityScoreDelta = (short)(CityScore - cityScoreLast);
        }

        private void DoVotes()
        {
            int z;

            CityYes = 0;

            for (z = 0; z < 100; z++)
            {
                if (GetRandom(1000) < CityScore)
                {
                    CityYes++;
                }
            }
        }
    }
}
