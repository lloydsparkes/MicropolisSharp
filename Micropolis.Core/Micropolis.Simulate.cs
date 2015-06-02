/* Micropolis.Simulate.cs
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
using System.Diagnostics;

namespace MicropolisSharp
{
    /// <summary>
    /// Partial Class Containing the content of simulate.cpp
    /// </summary>
    public partial class Micropolis
    {
        public bool ValveFlag { get; private set; }
        public short CrimeRamp { get; private set; }
        public short PollutionRamp { get; private set; }

        /// <summary>
        /// Block Residential Growth
        /// </summary>
        public bool ResCap { get; private set; }

        /// <summary>
        /// Block Commercial Growth
        /// </summary>
        public bool ComCap { get; private set; }

        /// <summary>
        /// Block Industrial Growth
        /// </summary>
        public bool IndCap { get; private set; }

        public short CashFlow { get; private set; }
        public float ExternalMarket { get; private set; }

        /// <summary>
        /// The disaster for which a count-down is running
        /// </summary>
        public Scenario DisasterEvent { get; private set; }

        /// <summary>
        /// Count Down Timer for the Disaster
        /// </summary>
        public short DisasterWait { get; private set; }

        /// <summary>
        /// The type of score table to use
        /// </summary>
        public Scenario ScoreType { get; private set; }

        /// <summary>
        /// The time to wait before computing the score
        /// </summary>
        public short scoreWait { get; private set; }

        /// <summary>
        /// Number of powered tiles in all zones
        /// </summary>
        public short PoweredZoneCount { get; private set; }

        /// <summary>
        /// Number of unpowered tiles in all zones
        /// </summary>
        public short UnpoweredZoneCount { get; private set; }

        public bool NewPower { get; private set; }

        public int CityTaxAverage { get; private set; }
        public short SimCycle { get; private set; }
        public short PhaseCycle { get; private set; }
        public short SpeedCycle { get; private set; }

        /// <summary>
        /// Do we need to perform the initial city evaluation
        /// </summary>
        public bool DoInitialEval { get; private set; }


        /// <summary>
        /// TODO: Make Private
        /// </summary>
        public short ResValve { get; private set; }

        /// <summary>
        /// TODO: Make Private
        /// </summary>
        public short ComValve { get; private set; }

        /// <summary>
        /// TODO: Make Private
        /// </summary>
        public short IndValve { get; private set; }

        /// <summary>
        /// comefrom: doEditWindow scoreDoer doMapInFront graphDoer doNilEvent
        /// </summary>
        public void SimFrame()
        {
            if (SimSpeed == 0)
            {
                return;
            }

            if (++SpeedCycle > 1023)
            {
                SpeedCycle = 0;
            }

            if (SimSpeed == 1 && (SpeedCycle % 5) != 0)
            {
                return;
            }

            if (SimSpeed == 2 && (SpeedCycle % 3) != 0)
            {
                return;
            }

            Simulate();
        }

        /// <summary>
        /// comefrom: simFrame
        /// </summary>
        public void Simulate() {
            short[] speedPowerScan = { 2,  4,  5 };
            short[] SpeedPollutionTerrainLandValueScan = { 2,  7, 17 };
            short[] speedCrimeScan = { 1,  8, 18 };
            short[] speedPopulationDensityScan = { 1,  9, 19 };
            short[] speedFireAnalysis = { 1, 10, 20 };

            short speedIndex = Utilities.Restrict((short)(SimSpeed - 1), (short)0, (short)2);

            // The simulator has 16 different phases, which we cycle through
            // according to phaseCycle, which is incremented and wrapped at
            // the end of this switch.

            if (InitSimLoad.IsTrue())
            {
                PhaseCycle = 0;
            }
            else
            {
                PhaseCycle &= 15;
            }

            switch (PhaseCycle)
            {

                case 0:

                    if (++SimCycle > 1023)
                    {
                        SimCycle = 0; // This is cosmic!
                    }

                    if (DoInitialEval)
                    {
                        DoInitialEval = false;
                        CityEvaluation();
                    }

                    CityTime++;
                    CityTaxAverage += CityTax;

                    if (!(SimCycle & 1).IsTrue())
                    {
                        SetValves();
                    }

                    ClearCensus();

                    break;

                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:

                    // Scan 1/8 of the map for each of the 8 phases 1..8:
                    mapScan((PhaseCycle - 1) * Constants.WorldWidth / 8, PhaseCycle * Constants.WorldWidth / 8);

                    break;

                case 9:
                    if (CityTime % Constants.CensusFrequency10 == 0)
                    {
                        Take10Census();
                    }

                    if (CityTime % Constants.CensusFrequency120 == 0)
                    {
                        Take120Census();
                    }

                    if (CityTime % Constants.TaxFrequency == 0)
                    {
                        CollectTax();
                        CityEvaluation();
                    }

                    break;

                case 10:

                    if (!(SimCycle % 5).IsTrue())
                    {
                        DecRateOfGrowthMap();
                    }

                    DecTrafficMap();

                    NewMapFlags[(int)MapType.TrafficDensity] = 1;
                    NewMapFlags[(int)MapType.Road] = 1;
                    NewMapFlags[(int)MapType.All] = 1;
                    NewMapFlags[(int)MapType.Res] = 1;
                    NewMapFlags[(int)MapType.Com] = 1;
                    NewMapFlags[(int)MapType.Ind] = 1;
                    NewMapFlags[(int)MapType.Dynamic] = 1;

                    SendMessages();

                    break;

                case 11:

                    if ((SimCycle % speedPowerScan[speedIndex]) == 0)
                    {
                        DoPowerScan();
                        NewMapFlags[(int)MapType.Power] = 1;
                        NewPower = true; /* post-release change */
                    }

                    break;

                case 12:

                    if ((SimCycle % SpeedPollutionTerrainLandValueScan[speedIndex]) == 0)
                    {
                        PollutionTerrainLandValueScan();
                    }

                    break;

                case 13:

                    if ((SimCycle % speedCrimeScan[speedIndex]) == 0)
                    {
                        CrimeScan();
                    }

                    break;

                case 14:

                    if ((SimCycle % speedPopulationDensityScan[speedIndex]) == 0)
                    {
                        PopulationDensityScan();
                    }

                    break;

                case 15:

                    if ((SimCycle % speedFireAnalysis[speedIndex]) == 0)
                    {
                        FireAnalysis();
                    }

                    DoDisasters();

                    break;

            }

            // Go on the the next phase.
            PhaseCycle = (short)((PhaseCycle + 1) & 15);
        }

        /// <summary>
        /// Initialize simulation.
        /// 
        /// TODO: Create constants for initSimLoad.
        /// </summary>
        public void DoSimInit()
        {
            PhaseCycle = 0;
            SimCycle = 0;

            if (InitSimLoad == 2)
            {
                /* if new city */
                InitSimMemory();
            }

            if (InitSimLoad == 1)
            {
                /* if city just loaded */
                SimLoadInit();
            }

            SetValves();
            ClearCensus();
            mapScan(0, Constants.WorldWidth);
            DoPowerScan();
            NewPower = true;         /* post rel */
            PollutionTerrainLandValueScan();
            CrimeScan();
            PopulationDensityScan();
            FireAnalysis();
            NewMap = 1;
            CensusChanged = true;
            TotalPop = 1;
            DoInitialEval = true;
        }

        /// <summary>
        /// Copy bits from powerGridMap to the #PWRBIT in the map for all zones in the world.
        /// </summary>
        public void DoNilPower()
        {
            int x, y;

            for (x = 0; x < Constants.WorldWidth; x++)
            {
                for (y = 0; y < Constants.WorldHeight; y++)
                {
                    ushort z = Map[x,y];
                    if ((z & (ushort)MapTileBits.CenterOfZone).IsTrue())
                    {
                        SetZonePower(new Position(x, y));
                    }
                }
            }
        }

        /// <summary>
        /// Decrease traffic memory
        /// </summary>
        public void DecTrafficMap()
        {     
            /* tends to empty trafficDensityMap */
            int x, y, z;

            for (x = 0; x < Constants.WorldWidth; x += TrafficDensityMap.BlockSize)
            {
                for (y = 0; y < Constants.WorldHeight; y += TrafficDensityMap.BlockSize)
                {
                    z = TrafficDensityMap.WorldGet(x, y);
                    if (z == 0)
                    {
                        continue;
                    }

                    if (z <= 24)
                    {
                        TrafficDensityMap.WorldSet(x, y, 0);
                        continue;
                    }

                    if (z > 200)
                    {
                        TrafficDensityMap.WorldSet(x, y, (byte)(z - 34));
                    }
                    else
                    {
                        TrafficDensityMap.WorldSet(x, y, (byte)(z - 24));
                    }
                }
            }
        }

        /// <summary>
        /// Decrease rate of grow.
        /// 
        /// TODO: Limiting rate should not be done here, but when we add a new value to it.
        /// </summary>
        public void DecRateOfGrowthMap()
        {     
            /* tends to empty rateOfGrowthMap */
            int x, y, z;

            for (x = 0; x < RateOfGrowthMap.width; x++)
            {
                for (y = 0; y < RateOfGrowthMap.height; y++)
                {
                    z = RateOfGrowthMap.Get(x, y);
                    if (z == 0)
                    {
                        continue;
                    }

                    if (z > 0)
                    {
                        z--;
                        z = Utilities.Restrict(z, (short)-200, (short)200);
                        RateOfGrowthMap.Set(x, y, (short)z);
                        continue;
                    }

                    if (z < 0)
                    {
                        z++;
                        z = Utilities.Restrict(z, (short)-200, (short)200);
                        RateOfGrowthMap.Set(x, y, (short)z);
                    }
                }
            }
        }

        /// <summary>
        /// comefrom: doSimInit
        /// </summary>
        public void InitSimMemory() {
            SetCommonInits();

            for (short x = 0; x < 240; x++)
            {
                ResHist[x] = 0;
                ComHist[x] = 0;
                IndHist[x] = 0;
                MoneyHist[x] = 128;
                CrimeHist[x] = 0;
                PollutionHist[x] = 0;
            }

            CrimeRamp = 0;
            PollutionRamp = 0;
            TotalPop = 0;
            ResValve = 0;
            ComValve = 0;
            IndValve = 0;
            ResCap = false; // Do not block residential growth
            ComCap = false; // Do not block commercial growth
            IndCap = false; // Do not block industrial growth

            ExternalMarket = 6.0f;
            DisasterEvent = Scenario.None;
            ScoreType = Scenario.None;

            /* This clears powermem */
            powerStackPointer = 0;
            DoPowerScan();
            NewPower = true; /* post rel */

            InitSimLoad = 0;
        }

        /// <summary>
        /// comefrom: doSimInit
        /// </summary>
        public void SimLoadInit()
        { 
            // Disaster delay table for each scenario
            short[] disasterWaitTable = {
                        0,          // No scenario (free playing)
                        2,          // Dullsville (boredom)
                        10,         // San francisco (earth quake)
                        4 * 10,     // Hamburg (fire bombs)
                        20,         // Bern (traffic)
                        3,          // Tokyo (scary monster)
                        5,          // Detroit (crime)
                        5,          // Boston (nuclear meltdown)
                        2 * 48,     // Rio (flooding)
                    };

            // Time to wait before score calculation for each scenario
            short[] scoreWaitTable = {
                        0,          // No scenario (free playing)
                        30 * 48,    // Dullsville (boredom)
                        5 * 48,     // San francisco (earth quake)
                        5 * 48,     // Hamburg (fire bombs)
                        10 * 48,    // Bern (traffic)
                        5 * 48,     // Tokyo (scary monster)
                        10 * 48,    // Detroit (crime)
                        5 * 48,     // Boston (nuclear meltdown)
                        10 * 48,    // Rio (flooding)
                    };

            ExternalMarket = (float)MiscHist[1];
            ResPop = MiscHist[2];
            ComPop = MiscHist[3];
            IndPop = MiscHist[4];
            ResValve = MiscHist[5];
            ComValve = MiscHist[6];
            IndValve = MiscHist[7];
            CrimeRamp = MiscHist[10];
            PollutionRamp = MiscHist[11];
            LandValueAverage = MiscHist[12];
            CrimeAverage = MiscHist[13];
            PollutionAverage = MiscHist[14];
            GameLevel = (Levels)MiscHist[15];

            if (CityTime < 0)
            {
                CityTime = 0;
            }

            if (!ExternalMarket.IsTrue())
            {
                ExternalMarket = 4.0f;
            }

            // Set game level
            if (GameLevel > Levels.Last || GameLevel < Levels.First)
            {
                GameLevel = Levels.First;
            }
            SetGameLevel(GameLevel);

            SetCommonInits();

            // Load cityClass
            CityClassification = (CityClassification)(MiscHist[16]);
            if (CityClassification > CityClassification.Megalopolis || CityClassification < CityClassification.Village)
            {
                CityClassification = CityClassification.Village;
            }

            CityScore = MiscHist[17];
            if (CityScore > 999 || CityScore < 1)
            {
                CityScore = 500;
            }

            ResCap = false;
            ComCap = false;
            IndCap = false;

            CityTaxAverage = (int)((CityTime % 48) * 7);  /* post */

            // Set power map.
            /// @todo What purpose does this serve? Weird...
            PowerGridMap.Fill(1);

            DoNilPower();

            if (Scenario >= Scenario.Count)
            {
                Scenario = Scenario.None;
            }

            if (Scenario != Scenario.None)
            {
                Debug.Assert(disasterWaitTable.Length == (int)Scenario.Count);
                Debug.Assert(scoreWaitTable.Length == (int)Scenario.Count);

                DisasterEvent = Scenario;
                DisasterWait = disasterWaitTable[(int)DisasterEvent];
                ScoreType = DisasterEvent;
                scoreWait = scoreWaitTable[(int)DisasterEvent];
            }
            else
            {
                DisasterEvent = Scenario.None;
                DisasterWait = 0;
                ScoreType = Scenario.None;
                scoreWait = 0;
            }

            RoadEffect = Constants.MaxRoadEffect;
            PoliceEffect = Constants.MaxPoliceStationEffect;
            FireEffect = Constants.MaxFireStationEffect;
            InitSimLoad = 0;
        }

        /// <summary>
        /// comefrom: initSimMemory simLoadInit
        /// </summary>
        public void SetCommonInits()
        {
            EvalInit();
            RoadEffect = Constants.MaxRoadEffect;
            PoliceEffect = Constants.MaxPoliceStationEffect;
            FireEffect = Constants.MaxFireStationEffect;
            TaxFlag = false;
            TaxFund = 0;
        }

        /// <summary>
        /// comefrom: simulate doSimInit
        /// </summary>
        public void SetValves() { /// @todo Break the tax table out into configurable parameters.
            short[] taxTable = {
                        200, 150, 120, 100, 80, 50, 30, 0, -10, -40, -100,
                        -150, -200, -250, -300, -350, -400, -450, -500, -550, -600,
                    };
            float[] extMarketParamTable = {
                        1.2f, 1.1f, 0.98f,
                    };

            Debug.Assert((int)Levels.Count == extMarketParamTable.Length);

            /// @todo Make configurable parameters.
            short resPopDenom = 8;
            float birthRate = 0.02f;
            float laborBaseMax = 1.3f;
            float internalMarketDenom = 3.7f;
            float projectedIndPopMin = 5.0f;
            float resRatioDefault = 1.3f;
            float resRatioMax = 2;
            float comRatioMax = 2;
            float indRatioMax = 2;
            short taxMax = 20;
            float taxTableScale = 600;

            /// @todo Break the interesting values out into public member
            ///       variables so the user interface can display them.
            float employment, migration, births, laborBase, internalMarket;
            float resRatio, comRatio, indRatio;
            float normalizedResPop, projectedResPop, projectedComPop, projectedIndPop;

            MiscHist[1] = (short)ExternalMarket;
            MiscHist[2] = (short)ResPop;
            MiscHist[3] = (short)ComPop;
            MiscHist[4] = (short)IndPop;
            MiscHist[5] = ResValve;
            MiscHist[6] = ComValve;
            MiscHist[7] = IndValve;
            MiscHist[10] = CrimeRamp;
            MiscHist[11] = PollutionRamp;
            MiscHist[12] = (short)LandValueAverage;
            MiscHist[13] = (short)CrimeAverage;
            MiscHist[14] = (short)PollutionAverage;
            MiscHist[15] = (short)GameLevel;
            MiscHist[16] = (short)CityClassification;
            MiscHist[17] = CityScore;

            normalizedResPop = (float)ResPop / (float)resPopDenom;
            TotalPopLast = TotalPop;
            TotalPop = (short)(normalizedResPop + ComPop + IndPop);

            if (ResPop > 0)
            {
                employment = (ComHist[1] + IndHist[1]) / normalizedResPop;
            }
            else
            {
                employment = 1;
            }

            migration = normalizedResPop * (employment - 1);
            births = normalizedResPop * birthRate;
            projectedResPop = normalizedResPop + migration + births;   // Projected res pop.

            // Compute laborBase
            float temp = ComHist[1] + IndHist[1];
            if (temp > 0.0)
            {
                laborBase = (ResHist[1] / temp);
            }
            else
            {
                laborBase = 1;
            }
            laborBase = Utilities.Restrict(laborBase, 0.0f, laborBaseMax);

            internalMarket = (float)(normalizedResPop + ComPop + IndPop) / internalMarketDenom;

            projectedComPop = internalMarket * laborBase;

            Debug.Assert(GameLevel >= Levels.First && GameLevel <= Levels.Last);
            projectedIndPop = IndPop * laborBase * extMarketParamTable[(int)GameLevel];
            projectedIndPop = Math.Max(projectedIndPop, projectedIndPopMin);

            if (normalizedResPop > 0)
            {
                resRatio = (float)projectedResPop / (float)normalizedResPop; // projected -vs- actual.
            }
            else
            {
                resRatio = resRatioDefault;
            }

            if (ComPop > 0)
            {
                comRatio = (float)projectedComPop / (float)ComPop;
            }
            else
            {
                comRatio = (float)projectedComPop;
            }

            if (IndPop > 0)
            {
                indRatio = (float)projectedIndPop / (float)IndPop;
            }
            else
            {
                indRatio = (float)projectedIndPop;
            }

            resRatio = Math.Min(resRatio, resRatioMax);
            comRatio = Math.Min(comRatio, comRatioMax);
            resRatio = Math.Min(indRatio, indRatioMax);

            // Global tax and game level effects.
            short z = Math.Min((short)(CityTax + GameLevel), taxMax);
            resRatio = (resRatio - 1) * taxTableScale + taxTable[z];
            comRatio = (comRatio - 1) * taxTableScale + taxTable[z];
            indRatio = (indRatio - 1) * taxTableScale + taxTable[z];

            // Ratios are velocity changes to valves.
            ResValve = (short)Utilities.Restrict(ResValve + (short)resRatio, -Constants.ResValveRange, Constants.ResValveRange);
            ComValve = (short)Utilities.Restrict(ComValve + (short)comRatio, -Constants.ComValveRange, Constants.ComValveRange);
            IndValve = (short)Utilities.Restrict(IndValve + (short)indRatio, -Constants.IndValveRange, Constants.IndValveRange);

            if (ResCap && ResValve > 0)
            {
                ResValve = 0; // Need a stadium, so cap resValve.
            }

            if (ComCap && ComValve > 0)
            {
                ComValve = 0; // Need a airport, so cap comValve.
            }

            if (IndCap && IndValve > 0)
            {
                IndValve = 0; // Need an seaport, so cap indValve.
            }

            ValveFlag = true;
        }

        /// <summary>
        /// comefrom: simulate doSimInit
        /// </summary>
        public void ClearCensus()
        {
            PoweredZoneCount = 0;
            UnpoweredZoneCount = 0;
            FirePop = 0;
            RoadTotal = 0;
            RailTotal = 0;
            ResPop = 0;
            ComPop = 0;
            IndPop = 0;
            ResZonePop = 0;
            ComZonePop = 0;
            IndZonePop = 0;
            HospitalPop = 0;
            ChurchPop = 0;
            PoliceStationPop = 0;
            FireStationPop = 0;
            StadiumPop = 0;
            CoalPowerPop = 0;
            NuclearPowerPop = 0;
            SeaportPop = 0;
            AirportPop = 0;
            powerStackPointer = 0; /* Reset before Mapscan */

            FireStationMap.Clear();
            //fireStationEffectMap.clear(); // Added in rev293
            PoliceStationMap.Clear();
            //policeStationEffectMap.clear(); // Added in rev293 
        }

        /// <summary>
        /// Take monthly snaphsot of all relevant data for the historic graphs.
        /// Also update variables that control building new churches and hospitals.
        /// 
        /// TODO: Rename to takeMonthlyCensus (or takeMonthlySnaphshot?).
        /// TODO: A lot of this max stuff is also done in graph.cpp
        /// </summary>
        public void Take10Census()
        { 
            // TODO: Make configurable parameters.
            int resPopDenom = 8;

            int x;

            /* put census#s in Historical Graphs and scroll data  */
            ResHist10Max = 0;
            ComHist10Max = 0;
            IndHist10Max = 0;

            for (x = 118; x >= 0; x--)
            {

                ResHist10Max = Math.Max(ResHist10Max, ResHist[x]);
                ComHist10Max = Math.Max(ComHist10Max, ComHist[x]);
                IndHist10Max = Math.Max(IndHist10Max, IndHist[x]);

                ResHist[x + 1] = ResHist[x];
                ComHist[x + 1] = ComHist[x];
                IndHist[x + 1] = IndHist[x];
                CrimeHist[x + 1] = CrimeHist[x];
                PollutionHist[x + 1] = PollutionHist[x];
                MoneyHist[x + 1] = MoneyHist[x];

            }

            Graph10Max = ResHist10Max;
            Graph10Max = Math.Max(Graph10Max, ComHist10Max);
            Graph10Max = Math.Max(Graph10Max, IndHist10Max);

            ResHist[0] = (short)(ResPop / resPopDenom);
            ComHist[0] = (short)ComPop;
            IndHist[0] = (short)IndPop;

            CrimeRamp += (short)((CrimeAverage - CrimeRamp) / 4);
            CrimeHist[0] = Math.Min(CrimeRamp, (short)255);

            PollutionRamp += (short)((PollutionAverage - PollutionRamp) / 4);
            PollutionHist[0] = Math.Min(PollutionRamp, (short)255);

            x = (CashFlow / 20) + 128;    /* scale to 0..255  */
            MoneyHist[0] = (short)Utilities.Restrict(x, (short)0, (short)255);

            ChangeCensus();

            int resPopScaled = ResPop >> 8;

            if (HospitalPop < resPopScaled)
            {
                NeedHospital = 1;
            }

            if (HospitalPop > resPopScaled)
            {
                NeedHospital = -1;
            }

            if (HospitalPop == resPopScaled)
            {
                NeedHospital = 0;
            }

            int faithfulPop = resPopScaled + Faith;

            if (ChurchPop < faithfulPop)
            {
                NeedChurch = 1;
            }

            if (ChurchPop > faithfulPop)
            {
                NeedChurch = -1;
            }

            if (ChurchPop == faithfulPop)
            {
                NeedChurch = 0;
            }
        }

        /// <summary>
        ///  comefrom: simulate
        /// </summary>
        public void Take120Census()
        { 
            // TODO: Make configurable parameters.
            int resPopDenom = 8;

            /* Long Term Graphs */
            int x;

            ResHist120Max = 0;
            ComHist120Max = 0;
            IndHist120Max = 0;

            for (x = 238; x >= 120; x--)
            {

                ResHist120Max = Math.Max(ResHist120Max, ResHist[x]);
                ComHist120Max = Math.Max(ComHist120Max, ComHist[x]);
                IndHist120Max = Math.Max(IndHist120Max, IndHist[x]);

                ResHist[x + 1] = ResHist[x];
                ComHist[x + 1] = ComHist[x];
                IndHist[x + 1] = IndHist[x];
                CrimeHist[x + 1] = CrimeHist[x];
                PollutionHist[x + 1] = PollutionHist[x];
                MoneyHist[x + 1] = MoneyHist[x];

            }

            Graph120Max = ResHist120Max;
            Graph120Max = Math.Max(Graph120Max, ComHist120Max);
            Graph120Max = Math.Max(Graph120Max, IndHist120Max);

            ResHist[120] = (short)(ResPop / resPopDenom);
            ComHist[120] = (short)ComPop;
            IndHist[120] = (short)IndPop;
            CrimeHist[120] = CrimeHist[0];
            PollutionHist[120] = PollutionHist[0];
            MoneyHist[120] = MoneyHist[0];
            ChangeCensus();
        }

        /// <summary>
        /// Collect taxes
        /// 
        /// TODO:Function seems to be doing different things depending on
        ///         Micropolis::totalPop value.With an non-empty city it does fund
        ///         calculations. For an empty city, it immediately sets effects of
        ///         funding, which seems inconsistent at least, and may be wrong
        /// TODO: If Micropolis::taxFlag is set, no variable is touched which seems
        ///         non-robust at least
        /// </summary>
        public void CollectTax()
        {
            int z;

            /**
             * @todo Break out so the user interface can configure this.
             */
            float[] RLevels = { 0.7f, 0.9f, 1.2f };
            float[] FLevels = { 1.4f, 1.2f, 0.8f };

            Debug.Assert((int)Levels.Count == RLevels.Length);
            Debug.Assert((int)Levels.Count == FLevels.Length);

            CashFlow = 0;

            /**
             * @todo Apparently taxFlag is never set to true in MicropolisEngine
             *       or the TCL code, so this always runs.
             * @todo Check old Mac code to see if it's ever set, and why.
             */

            if (!TaxFlag)
            { // If the Tax Port is clear

                /// @todo Do something with z? Check old Mac code to see if it's used.
                z = CityTaxAverage / 48;  // post release

                CityTaxAverage = 0;

                PoliceFund = (long)PoliceStationPop * 100;
                FireFund = (long)FireStationPop * 100;
                RoadFund = (long)((RoadTotal + (RailTotal * 2)) * RLevels[(int)GameLevel]);
                TaxFund = (long)((((long)TotalPop * LandValueAverage) / 120) * CityTax * FLevels[(int)GameLevel]);

                if (TotalPop > 0)
                {
                    /* There are people to tax. */
                    CashFlow = (short)(TaxFund - (PoliceFund + FireFund + RoadFund));
                    DoBudget();
                }
                else
                {
                    /* Nobody lives here. */
                    RoadEffect = Constants.MaxRoadEffect;
                    PoliceEffect = Constants.MaxPoliceStationEffect;
                    FireEffect = Constants.MaxFireStationEffect;
                }
            }
        }

        /// <summary>
        /// Update effects of (possibly reduced) funding
        /// 
        /// It updates effects with respect to roads, police, and fire.
        /// 
        /// TODO: This function should probably not be used when #totalPop is
        ///        clear(ie with an empty) city.See also bugs of #collectTax()
        /// TODO: I think this should be called after loading a city, or any
        ///        time anything it depends on changes.
        /// </summary>
        public void UpdateFundEffects()
        {     // Compute road effects of funding
            RoadEffect = Constants.MaxRoadEffect;
            if (RoadFund > 0)
            {
                // Multiply with funding fraction
                RoadEffect = (short)((float)RoadEffect * (float)RoadSpend / (float)RoadFund);
            }

            // Compute police station effects of funding
            PoliceEffect = Constants.MaxPoliceStationEffect;
            if (PoliceFund > 0)
            {
                // Multiply with funding fraction
                PoliceEffect = (short)((float)PoliceEffect * (float)PoliceSpend / (float)PoliceFund);
            }

            // Compute fire station effects of funding
            FireEffect = Constants.MaxFireStationEffect;
            if (FireFund > 0)
            {
                // Multiply with funding fraction
                FireEffect = (short)((float)FireEffect * (float)FireSpend / (float)FireFund);
            }

            MustDrawBudget = 1;
        }

        /// <summary>
        /// comefrom: simulate doSimInit
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        public void mapScan(int x1, int x2) {
            int x, y;

            for (x = x1; x < x2; x++)
            {
                for (y = 0; y < Constants.WorldHeight; y++)
                {

                    ushort mapVal = Map[x,y];
                    if (mapVal == (ushort)MapTileCharacters.DIRT)
                    {
                        continue;
                    }

                    ushort tile = (ushort)(mapVal & (ushort)MapTileBits.LowMask);  /* Mask off status bits  */

                    if (tile < (ushort)MapTileCharacters.FLOOD)
                    {
                        continue;
                    }

                    // tile >= FLOOD

                    Position pos = new Position(x, y);


                    if (tile < (ushort)MapTileCharacters.ROADBASE)
                    {

                        if (tile >= (ushort)MapTileCharacters.FIREBASE)
                        {
                            FirePop++;
                            if (!(GetRandom16() & 3).IsTrue())
                            {
                                DoFire(pos);    /* 1 in 4 times */
                            }
                            continue;
                        }

                        if (tile < (ushort)MapTileCharacters.RADTILE)
                        {
                            DoFlood(pos);
                        }
                        else
                        {
                            DoRadTile(pos);
                        }

                        continue;
                    }

                    if (NewPower && (mapVal & (ushort)MapTileBits.Conductivity).IsTrue())
                    {
                        // Copy PWRBIT from powerGridMap
                        SetZonePower(pos);
                    }

                    if (tile >= (ushort)MapTileCharacters.ROADBASE && tile < (ushort)MapTileCharacters.POWERBASE)
                    {
                        DoRoad(pos);
                        continue;
                    }

                    if ((mapVal & (ushort)MapTileBits.CenterOfZone).IsTrue())
                    { /* process Zones */
                        DoZone(pos);
                        continue;
                    }

                    if (tile >= (ushort)MapTileCharacters.RAILBASE && tile < (ushort)MapTileCharacters.RESBASE)
                    {
                        DoRail(pos);
                        continue;
                    }

                    if (tile >= (ushort)MapTileCharacters.SOMETINYEXP && tile <= (ushort)MapTileCharacters.LASTTINYEXP)
                    {
                        /* clear AniRubble */
                        Map[x,y] = RandomRubble();
                    }
                }
            }
        }

        /// <summary>
        /// Handle rail track.
        /// 
        /// Generate a train, and handle road deteriorating effects.
        /// </summary>
        /// <param name="pos">Position of the rail.</param>
        public void DoRail(Position pos)
        {
            RailTotal++;

            GenerateTrain(pos.X, pos.Y);

            if (RoadEffect < (15 * Constants.MaxRoadEffect / 16))
            {

                // roadEffect < 15/16 of max road, enable deteriorating rail
                if (!(GetRandom16() & 511).IsTrue())
                {

                    ushort curValue = Map[pos.X,pos.Y];
                    if (!(curValue & (ushort)MapTileBits.Conductivity).IsTrue())
                    {
                        // Otherwise the '(getRandom16() & 31)' makes no sense
                        Debug.Assert(Constants.MaxRoadEffect == 32);

                        if (RoadEffect < (GetRandom16() & 31))
                        {
                            ushort tile = (ushort)(curValue & (ushort)MapTileBits.LowMask);
                            if (tile < (ushort)MapTileCharacters.RAILBASE + 2)
                            {
                                Map[pos.X, pos.Y] = (ushort)MapTileCharacters.RIVER;
                            }
                            else
                            {
                                Map[pos.X, pos.Y] = RandomRubble();
                            }
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handle decay of radio-active tile
        /// </summary>
        /// <param name="pos">Position of the radio-active tile.</param>
        public void DoRadTile(Position pos)
        {
            if ((GetRandom16() & 4095) == 0)
            {
                Map[pos.X, pos.Y] = (ushort)MapTileCharacters.DIRT; /* Radioactive decay */
            }
        }

        /// <summary>
        /// Handle road tile.
        /// </summary>
        /// <param name="pos">Position of the road.</param>
        public void DoRoad(Position pos) {
            int tden, z;
            short[] densityTable = { (short)MapTileCharacters.ROADBASE, (short)MapTileCharacters.LTRFBASE, (short)MapTileCharacters.HTRFBASE };

            RoadTotal++;

            ushort mapValue = Map[pos.X, pos.Y];
            ushort tile = (ushort)(mapValue & (ushort)MapTileBits.LowMask);

            /* generateBus(pos.X, pos.Y); */

            if (RoadEffect < (15 * Constants.MaxRoadEffect / 16))
            {
                // roadEffect < 15/16 of max road, enable deteriorating road
                if ((GetRandom16() & 511) == 0)
                {
                    if (!(mapValue & (ushort)MapTileBits.Conductivity).IsTrue())
                    {
                        Debug.Assert(Constants.MaxRoadEffect == 32); // Otherwise the '(getRandom16() & 31)' makes no sense
                        if (RoadEffect < (GetRandom16() & 31))
                        {
                            if ((tile & 15) < 2 || (tile & 15) == 15)
                            {
                                Map[pos.X, pos.Y] = (ushort)MapTileCharacters.RIVER;
                            }
                            else
                            {
                                Map[pos.X, pos.Y] = RandomRubble();
                            }
                            return;
                        }
                    }
                }
            }

            if ((mapValue & (ushort)MapTileBits.Burnable) == 0)
            { /* If Bridge */
                RoadTotal += 4; // Bridge counts as 4 road tiles
                if (DoBridge(new Position(pos.X, pos.Y), tile))
                {
                    return;
                }
            }

            if (tile < (ushort)MapTileCharacters.LTRFBASE)
            {
                tden = 0;
            }
            else if (tile < (ushort)MapTileCharacters.HTRFBASE)
            {
                tden = 1;
            }
            else
            {
                RoadTotal++; // Heavy traffic counts as 2 roads.
                tden = 2;
            }

            int trafficDensity = TrafficDensityMap.WorldGet(pos.X, pos.Y) >> 6;

            if (trafficDensity > 1)
            {
                trafficDensity--;
            }

            if (tden != trafficDensity)
            { /* tden 0..2   */
                z = ((tile - (ushort)MapTileCharacters.ROADBASE) & 15) + densityTable[trafficDensity];
                z |= mapValue & ((ushort)MapTileBits.AllBits - (ushort)MapTileBits.Animated);

                if (trafficDensity > 0)
                {
                    z |= (ushort)MapTileBits.Animated;
                }

                Map[pos.X, pos.Y] = (ushort)z;
            }
        }

        /// <summary>
        /// Handle a bridge.
        /// 
        /// TODO: What does this function return
        /// TODO: Discover the structure of all of its magic constants
        /// </summary>
        /// <param name="pos">Position of the bridge.</param>
        /// <param name="tile">Tile value of the bridge.</param>
        /// <returns>????</returns>
        public bool DoBridge(Position pos, ushort tile) {
            short[] HDx = { -2, 2, -2, -1, 0, 1, 2 };
            short[] HDy = { -1, -1, 0, 0, 0, 0, 0 };
            short[] HBRTAB = {
                        (short)MapTileCharacters.HBRDG1 | (short)MapTileBits.Bulldozable, (short)MapTileCharacters.HBRDG3 | (short)MapTileBits.Bulldozable, (short)MapTileCharacters.HBRDG0 | (short)MapTileBits.Bulldozable,
                        (short)MapTileCharacters.RIVER, (short)MapTileCharacters.BRWH | (short)MapTileBits.Bulldozable, (short)MapTileCharacters.RIVER, (short)MapTileCharacters.HBRDG2 | (short)MapTileBits.Bulldozable,
                    };
            short[] HBRTAB2 = {
                        (short)MapTileCharacters.RIVER, (short)MapTileCharacters.RIVER, (short)MapTileCharacters.HBRIDGE | (short)MapTileBits.Bulldozable, (short)MapTileCharacters.HBRIDGE | (short)MapTileBits.Bulldozable, (short)MapTileCharacters.HBRIDGE | (short)MapTileBits.Bulldozable,
                        (short)MapTileCharacters.HBRIDGE | (short)MapTileBits.Bulldozable, (short)MapTileCharacters.HBRIDGE | (short)MapTileBits.Bulldozable,
                    };
            short[] VDx = { 0, 1, 0, 0, 0, 0, 1 };
            short[] VDy = { -2, -2, -1, 0, 1, 2, 2 };
            short[] VBRTAB = {
                        (short)MapTileCharacters.VBRDG0 | (short)MapTileBits.Bulldozable, (short)MapTileCharacters.VBRDG1 | (short)MapTileBits.Bulldozable, (short)MapTileCharacters.RIVER, (short)MapTileCharacters.BRWV | (short)MapTileBits.Bulldozable,
                        (short)MapTileCharacters.RIVER, (short)MapTileCharacters.VBRDG2 | (short)MapTileBits.Bulldozable, (short)MapTileCharacters.VBRDG3 | (short)MapTileBits.Bulldozable,
                    };
            short[] VBRTAB2 = {
                        (short)MapTileCharacters.VBRIDGE | (short)MapTileBits.Bulldozable, (short)MapTileCharacters.RIVER, (short)MapTileCharacters.VBRIDGE | (short)MapTileBits.Bulldozable, (short)MapTileCharacters.VBRIDGE | (short)MapTileBits.Bulldozable,
                        (short)MapTileCharacters.VBRIDGE | (short)MapTileBits.Bulldozable, (short)MapTileCharacters.VBRIDGE | (short)MapTileBits.Bulldozable, (short)MapTileCharacters.RIVER,
                    };
            int z, x, y, MPtem;

            if (tile == (short)MapTileCharacters.BRWV)
            { /*  Vertical bridge close */

                if ((!(GetRandom16() & 3).IsTrue()) && GetBoatDistance(pos) > 340)
                {

                    for (z = 0; z < 7; z++)
                    { /* Close */

                        x = pos.X + VDx[z];
                        y = pos.Y + VDy[z];

                        if (Position.TestBounds(x, y))
                        {

                            if ((Map[x,y] & (short)MapTileBits.LowMask) == (VBRTAB[z] & (short)MapTileBits.LowMask))
                            {
                                Map[x,y] = (ushort)VBRTAB2[z];
                            }

                        }
                    }
                }
                return true;
            }

            if (tile == (short)MapTileCharacters.BRWH)
            { /*  Horizontal bridge close  */

                if ((!(GetRandom16() & 3).IsTrue()) && GetBoatDistance(pos) > 340)
                {

                    for (z = 0; z < 7; z++)
                    { /* Close */

                        x = pos.X + HDx[z];
                        y = pos.Y + HDy[z];

                        if (Position.TestBounds(x, y))
                        {

                            if ((Map[x,y] & (short)MapTileBits.LowMask) == (HBRTAB[z] & (short)MapTileBits.LowMask))
                            {

                                Map[x,y] = (ushort)HBRTAB2[z];

                            }
                        }
                    }
                }
                return true;
            }

            if (GetBoatDistance(pos) < 300 || (!(GetRandom16() & 7).IsTrue()))
            {
                if ((tile & 1).IsTrue())
                {
                    if (pos.X < Constants.WorldWidth - 1)
                    {
                        if (Map[pos.X + 1,pos.Y] == (short)MapTileCharacters.CHANNEL)
                        { /* Vertical open */

                            for (z = 0; z < 7; z++)
                            {

                                x = pos.X + VDx[z];
                                y = pos.Y + VDy[z];

                                if (Position.TestBounds(x, y))
                                {

                                    MPtem = Map[x,y];
                                    if (MPtem == (short)MapTileCharacters.CHANNEL || ((MPtem & 15) == (VBRTAB2[z] & 15)))
                                    {
                                        Map[x,y] = (ushort)VBRTAB[z];
                                    }
                                }
                            }
                            return true;
                        }
                    }
                    return false;

                }
                else
                {

                    if (pos.Y > 0)
                    {
                        if (Map[pos.X,pos.Y - 1] == (short)MapTileCharacters.CHANNEL)
                        {

                            /* Horizontal open  */
                            for (z = 0; z < 7; z++)
                            {

                                x = pos.X + HDx[z];
                                y = pos.Y + HDy[z];

                                if (Position.TestBounds(x, y))
                                {

                                    MPtem = Map[x,y];
                                    if (((MPtem & 15) == (HBRTAB2[z] & 15)) || MPtem == (short)MapTileCharacters.CHANNEL)
                                    {
                                        Map[x,y] = (ushort)HBRTAB[z];
                                    }
                                }
                            }
                            return true;
                        }
                    }
                    return false;
                }

            }
            return false;
        }

        /// <summary>
        /// Compute distance to nearest boat from a given bridge.
        /// </summary>
        /// <param name="pos">Position of bridge.</param>
        /// <returns>Distance to nearest boat.</returns>
        public int GetBoatDistance(Position pos)
        {
            int sprDist;

            int dist = 99999;
            int mx = pos.X * 16 + 8;
            int my = pos.Y * 16 + 8;

            foreach(SimSprite sprite in SpriteList)
            {
                if (sprite.Type == SpriteType.Ship && sprite.Frame != 0)
                {

                    sprDist = Math.Abs(sprite.X + sprite.XHot - mx)
                            + Math.Abs(sprite.Y + sprite.YHot - my);

                    dist = Math.Min(dist, sprDist);
                }
            }
            return dist;
        }

        /// <summary>
        /// Handle tile being on fire.
        /// 
        /// TODO: Needs a notion of iterative neighbour tiles computing.
        /// TODO: Use a getFromMap()-like function here.
        /// TODO: Extract constants of fire station effectiveness from here.
        /// </summary>
        /// <param name="pos">Position of the fire.</param>
        public void DoFire(Position pos) {
            short[] DX = { -1, 0, 1, 0 };
            short[] DY = { 0, -1, 0, 1 };

            // Try to set neighbouring tiles on fire as well
            int z = 0;
            for (z = 0; z < 4; z++)
            {

                if ((GetRandom16() & 7) == 0)
                {

                    int xTem = pos.X + DX[z];
                    int yTem = pos.Y + DY[z];

                    if (Position.TestBounds(xTem, yTem))
                    {

                        ushort c = Map[xTem,yTem];
                        if (!(c & (ushort)MapTileBits.Burnable).IsTrue())
                        {
                            continue;
                        }

                        if ((c & (ushort)MapTileBits.CenterOfZone).IsTrue())
                        {
                            // Neighbour is a zone and burnable
                            FireZone(new Position(xTem, yTem), c);

                            if ((c & (ushort)MapTileBits.LowMask) > (ushort)MapTileCharacters.IZB)
                            { /* Explode */
                                MakeExplosionAt(xTem * 16 + 8, yTem * 16 + 8);
                            }
                        }

                        Map[xTem,yTem] = RandomFire();
                    }
                }
            }

            // Compute likelyhood of fire running out of fuel
            int rate = 10; // Likelyhood of extinguishing (bigger means less chance)
            z = FireStationEffectMap.WorldGet(pos.X, pos.Y);

            if (z > 0)
            {
                rate = 3;
                if (z > 20)
                {
                    rate = 2;
                }
                if (z > 100)
                {
                    rate = 1;
                }
            }

            // Decide whether to put out the fire.
            if (GetRandom((short)rate) == 0)
            {
                Map[pos.X, pos.Y] = RandomRubble();
            }
        }

        /// <summary>
        /// Handle a zone on fire.
        /// 
        /// Decreases rate of growth of the zone, and makes remaining tiles bulldozable.
        /// </summary>
        /// <param name="pos">Position of the zone on fire.</param>
        /// <param name="ch">Character of the zone.</param>
        public void FireZone(Position pos, ushort ch)
        {
            int XYmax;

            int value = RateOfGrowthMap.WorldGet(pos.X, pos.Y);
            value = Utilities.Restrict(value - 20, -200, 200);
            RateOfGrowthMap.WorldSet(pos.X, pos.Y, (short)value);

            ch = (ushort)(ch & (ushort)MapTileBits.LowMask);

            if (ch < (ushort)MapTileCharacters.PORTBASE)
            {
                XYmax = 2;
            }
            else
            {
                if (ch == (ushort)MapTileCharacters.AIRPORT)
                {
                    XYmax = 5;
                }
                else
                {
                    XYmax = 4;
                }
            }

            // Make remaining tiles of the zone bulldozable
            for (short x = -1; x < XYmax; x++)
            {
                for (short y = -1; y < XYmax; y++)
                {

                    int xTem = pos.X + x;
                    int yTem = pos.Y + y;

                    if (!Position.TestBounds(xTem, yTem))
                    {
                        continue;
                    }

                    if ((ushort)(Map[xTem,yTem] & (ushort)MapTileBits.LowMask) >= (ushort)MapTileCharacters.ROADBASE)
                    {
                        /* post release */
                        Map[xTem, yTem] |= (ushort)MapTileBits.Bulldozable;
                    }

                }
            }
        }

        /// <summary>
        /// Repair a zone at \a pos.
        /// </summary>
        /// <param name="pos">Center-tile position of the zone.</param>
        /// <param name="zCent">Value of the center tile.</param>
        /// <param name="zSize">Size of the zone (in both directions).</param>
        public void RepairZone(Position pos, ushort zCent, short zSize)
        {
            ushort tile = (ushort)(zCent - 2 - zSize);

            // y and x loops one position shifted to compensate for the center-tile position.
            for (short y = -1; y < zSize - 1; y++)
            {
                for (short x = -1; x < zSize - 1; x++)
                {

                    int xx = pos.X + x;
                    int yy = pos.Y + y;

                    tile++;

                    if (Position.TestBounds(xx, yy))
                    {

                        ushort mapValue = Map[xx,yy];

                        if ((mapValue & (ushort)MapTileBits.CenterOfZone).IsTrue())
                        {
                            continue;
                        }

                        if ((mapValue & (ushort)MapTileBits.Animated).IsTrue())
                        {
                            continue;
                        }

                        ushort mapTile = (ushort)(mapValue & (ushort)MapTileBits.LowMask);

                        if (mapTile < (ushort)MapTileCharacters.RUBBLE || mapTile >= (ushort)MapTileCharacters.ROADBASE)
                        {
                            Map[xx,yy] = (ushort)(tile | (ushort)MapTileBits.Conductivity | (ushort)MapTileBits.Burnable);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update special zones.
        /// </summary>
        /// <param name="pos">Position of the zone.</param>
        /// <param name="powerOn">Zone is powered.</param>
        public void DoSpecialZone(Position pos, bool powerOn)
        {     // Bigger numbers reduce chance of nuclear melt down
            short[] meltdownTable = { 30000, 20000, 10000 };

            ushort tile = (ushort)(Map[pos.X, pos.Y] & (ushort)MapTileBits.LowMask);

            switch ((MapTileCharacters)tile)
            {

                case MapTileCharacters.POWERPLANT:

                    CoalPowerPop++;

                    if ((CityTime & 7) == 0)
                    {
                        RepairZone(pos, (ushort)MapTileCharacters.POWERPLANT, 4); /* post */
                    }

                    PushPowerStack(pos);
                    CoalSmoke(pos);

                    return;

                case MapTileCharacters.NUCLEAR:

                    Debug.Assert((int)Levels.Count == meltdownTable.Length);

                    if (EnableDisasters && !GetRandom(meltdownTable[(int)GameLevel]).IsTrue())
                    {
                        DoMeltdown(pos);
                        return;
                    }

                    NuclearPowerPop++;

                    if ((CityTime & 7) == 0)
                    {
                        RepairZone(pos, (ushort)MapTileCharacters.NUCLEAR, 4); /* post */
                    }

                    PushPowerStack(pos);

                    return;

                case MapTileCharacters.FIRESTATION:
                    {

                        int z;

                        FireStationPop++;

                        if (!(CityTime & 7).IsTrue())
                        {
                            RepairZone(pos, (ushort)MapTileCharacters.FIRESTATION, 3); /* post */
                        }

                        if (powerOn)
                        {
                            z = (int)FireEffect;                   /* if powered get effect  */
                        }
                        else
                        {
                            z = (int)(FireEffect / 2);               /* from the funding ratio  */
                        }

                        Position pos2 = new Position(pos);
                        bool foundRoad = FindPerimeterRoad(pos2);

                        if (!foundRoad)
                        {
                            z = z / 2;                        /* post FD's need roads  */
                        }

                        int value = FireStationMap.WorldGet(pos2.X, pos2.Y);
                        value += z;
                        FireStationMap.WorldSet(pos2.X, pos2.Y, (short)value);

                        return;
                    }

                case MapTileCharacters.POLICESTATION:
                    {

                        int z;

                        PoliceStationPop++;

                        if (!(CityTime & 7).IsTrue())
                        {
                            RepairZone(pos, (ushort)MapTileCharacters.POLICESTATION, 3); /* post */
                        }

                        if (powerOn)
                        {
                            z = (int)PoliceEffect;
                        }
                        else
                        {
                            z = (int)(PoliceEffect / 2);
                        }

                        Position pos2 = new Position(pos);
                        bool foundRoad = FindPerimeterRoad(pos2);

                        if (!foundRoad)
                        {
                            z = z / 2; /* post PD's need roads */
                        }

                        int value = PoliceStationMap.WorldGet(pos2.X, pos2.Y);
                        value += z;
                        PoliceStationMap.WorldSet(pos2.X, pos2.Y, (short)value);

                        return;
                    }

                case MapTileCharacters.STADIUM:  // Empty stadium

                    StadiumPop++;

                    if (!(CityTime & 15).IsTrue())
                    {
                        RepairZone(pos, (ushort)MapTileCharacters.STADIUM, 4);
                    }

                    if (powerOn)
                    {
                        // Every now and then, display a match
                        if (((CityTime + pos.X + pos.Y) & 31) == 0)
                        {
                            DrawStadium(pos, (ushort)MapTileCharacters.FULLSTADIUM);
                            Map[pos.X + 1, pos.Y] = (ushort)(MapTileCharacters.FOOTBALLGAME1 + (ushort)MapTileBits.Animated);
                            Map[pos.X + 1, pos.Y + 1] = (ushort)(MapTileCharacters.FOOTBALLGAME2 + (ushort)MapTileBits.Animated);
                        }
                    }

                    return;

                case MapTileCharacters.FULLSTADIUM:  // Full stadium

                    StadiumPop++;

                    if (((CityTime + pos.X + pos.Y) & 7) == 0)
                    {
                        // Stop the match
                        DrawStadium(pos, (ushort)MapTileCharacters.STADIUM);
                    }

                    return;

                case MapTileCharacters.AIRPORT:

                    AirportPop++;

                    if ((CityTime & 7) == 0)
                    {
                        RepairZone(pos, (ushort)MapTileCharacters.AIRPORT, 6);
                    }

                    // If powered, display a rotating radar
                    if (powerOn)
                    {
                        if ((Map[pos.X + 1, pos.Y - 1] & (ushort)MapTileBits.LowMask) == (ushort)MapTileCharacters.RADAR)
                        {
                            Map[pos.X + 1, pos.Y - 1] = (ushort)(MapTileCharacters.RADAR0 + (ushort)MapTileBits.Animated + (ushort)MapTileBits.Conductivity + (ushort)MapTileBits.Burnable);
                        }
                    }
                    else
                    {
                        Map[pos.X + 1, pos.Y - 1] = (ushort)(MapTileCharacters.RADAR + (ushort)MapTileBits.Conductivity + (ushort)MapTileBits.Burnable);
                    }

                    if (powerOn)
                    { // Handle the airport only if there is power
                        DoAirport(pos);
                    }

                    return;

                case MapTileCharacters.PORT:

                    SeaportPop++;

                    if ((CityTime & 15) == 0)
                    {
                        RepairZone(pos, (ushort)MapTileCharacters.PORT, 4);
                    }

                    // If port has power and there is no ship, generate one
                    if (powerOn && GetSprite(SpriteType.Ship) == null)
                    {
                        GenerateShip();
                    }

                    return;
            }
        }

        /// <summary>
        /// Draw a stadium (either full or empty).
        /// </summary>
        /// <param name="center">Center tile position of the stadium.</param>
        /// <param name="z">Base tile value.</param>
        public void DrawStadium(Position center, ushort z)
        {
            int x, y;

            z = (ushort)(z - 5);

            for (y = center.Y - 1; y < center.Y + 3; y++)
            {
                for (x = center.X - 1; x < center.X + 3; x++)
                {
                    Map[x, y] = (ushort)(z | (ushort)MapTileBits.BurnableOrConductive);
                    z++;
                }
            }

            Map[center.X, center.Y] |= (ushort)MapTileBits.CenterOfZone | (ushort)MapTileBits.Power;
        }

        /// <summary>
        /// Generate a airplane or helicopter every now and then.
        /// </summary>
        /// <param name="pos">Position of the airport to start from.</param>
        public void DoAirport(Position pos)
        {
            if (GetRandom(5) == 0)
            {
                GeneratePlane(pos);
                return;
            }

            if (GetRandom(12) == 0)
            {
                GenerateCopter(pos);
            }
        }

        /// <summary>
        /// Draw coal smoke tiles around given position (of a coal power plant).
        /// </summary>
        /// <param name="pos">Center tile of the coal power plant</param>
        public void CoalSmoke(Position pos)
        {
            short[] SmTb = {
                    (short)MapTileCharacters.COALSMOKE1, (short)MapTileCharacters.COALSMOKE2,
                    (short)MapTileCharacters.COALSMOKE3, (short)MapTileCharacters.COALSMOKE4,
                };
            short[] dx = { 1, 2, 1, 2 };
            short[] dy = { -1, -1, 0, 0 };

            for (short x = 0; x < 4; x++)
            {
                Map[pos.X + dx[x], pos.Y + dy[x]] = (ushort)
                    (SmTb[x] | (ushort)MapTileBits.Animated | (ushort)MapTileBits.Conductivity | (ushort)MapTileBits.Power | (ushort)MapTileBits.Burnable);
            }
        }

        /// <summary>
        /// Perform a nuclear melt-down disaster
        /// </summary>
        /// <param name="pos">Position of the nuclear power plant that melts.</param>
        public void DoMeltdown(Position pos)
        {
            MakeExplosion(pos.X - 1, pos.Y - 1);
            MakeExplosion(pos.X - 1, pos.Y + 2);
            MakeExplosion(pos.X + 2, pos.Y - 1);
            MakeExplosion(pos.X + 2, pos.Y + 2);

            // Whole power plant is at fire
            for (int x = pos.X - 1; x < pos.X + 3; x++)
            {
                for (int y = pos.Y - 1; y < pos.Y + 3; y++)
                {
                    Map[x, y] = RandomFire();
                }
            }

            // Add lots of radiation tiles around the plant
            for (int z = 0; z < 200; z++)
            {

                int x = pos.X - 20 + GetRandom(40);
                int y = pos.Y - 15 + GetRandom(30);

                if (!Position.TestBounds(x, y))
                { // Ignore off-map positions
                    continue;
                }

                ushort t = Map[x,y];

                if ((t & (ushort)MapTileBits.CenterOfZone).IsTrue())
                {
                    continue; // Ignore zones
                }

                if ((t & (ushort)MapTileBits.Burnable).IsTrue() || t == (ushort)MapTileCharacters.DIRT)
                {
                    Map[x, y] = (ushort)MapTileCharacters.RADTILE; // Make tile radio-active
                }

            }

            // Report disaster to the user
            SendMessage(GeneralMessages.MESSAGE_NUCLEAR_MELTDOWN, (short)pos.X, (short)pos.Y, true, true);
        }
         
        public ushort RandomFire()
        {
            return (ushort)((ushort)((ushort)MapTileCharacters.FIRE + (GetRandom16() & 7)) | (ushort)MapTileBits.Animated);
        }

        public ushort RandomRubble()
        {
            return (ushort)((ushort)((ushort)MapTileCharacters.RUBBLE + (GetRandom16() & 7)) | (ushort)MapTileBits.Bulldozable);
        }
    }
}
