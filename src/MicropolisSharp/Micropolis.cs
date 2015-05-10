/* Micropolis.cs
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
    /// The Overall Unifying Class for Micropolis
    /// 
    /// General Methods for the Simulation
    /// 
    /// Code from micropolis.h & micropolis.cpp
    /// </summary>
    public partial class Micropolis
    {
        public ushort[,] Map { get; private set; }
        public long TotalFunds { get; private set; }
        public bool AutoBulldoze { get; private set; }
        public bool AutoBudget { get; private set; }

        public long MessageLastTime { get; private set; }
        public Levels GameLevel { get; private set; } 
        public short InitSimLoad { get; private set; }
        public Scenario Scenario { get; private set; }
        public short SimSpeed { get; private set; }
        public short SimSpeedMeta { get; private set; }
        public bool EnableSound { get; private set; }
        public bool EnableDisasters { get; private set; }
        public short MessageNumber { get; private set; }
        public bool EvalChanged { get; private set; }
        public short BlinkFlag { get; private set; }

        //Population Counts
        public int RoadTotal { get; private set; }
        public int RailTotal { get; private set; }
        public int FirePop { get; private set; }
        public int ResPop { get; private set; }
        public int ComPop { get; private set; }
        public int IndPop { get; private set; }
        public int TotalPop { get; private set; }
        public int TotalPopLast { get; private set; }
        public int ResZonePop { get; private set; }
        public int ComZonePop { get; private set; }
        public int IndZonePop { get; private set; }
        public int TotalZonePop { get; private set; }

        public int HospitalPop { get; private set; }
        public int ChurchPop { get; private set; }
        public int Faith { get; private set; }
        public int StadiumPop { get; private set; }

        public int PoliceStationPop { get; private set; }
        public int FireStationPop { get; private set; }
        public int NuclearPowerPop { get; private set; }
        public int SeaportPop { get; private set; }
        public int AirportPop { get; private set; }
        public int CoalPowerPop { get; private set; }

        public int CrimeAverage { get; private set; }
        public int PollutionAverage { get; private set; }
        public int LandValueAverage { get; private set; }

        public long CityTime { get; private set; }
        public long CityMonth { get; private set; }
        public long CityYear { get; private set; }
        public int StartingYear { get; private set; }

        public int ResHist10Max { get; private set; }
        public int ResHist120Max { get; private set; }
        public int ComHist10Max { get; private set; }
        public int ComHist120Max { get; private set; }
        public int IndHist10Max { get; private set; }
        public int IndHist120Max { get; private set; }

        public bool CensusChanged { get; private set; }

        public long RoadSpend { get; private set; }
        public long PoliceSpend { get; private set; }
        public long FireSpend { get; private set; }
        public long RoadFund { get; private set; }
        public long PoliceFund { get; private set; }
        public long FireFund { get; private set; }
        public long RoadEffect { get; private set; }
        public long PoliceEffect { get; private set; }
        public long FireEffect { get; private set; }
        public long TaxFund { get; private set; }

        public int CityTax { get; private set; }
        public bool TaxFlag { get; private set; }

        //Histories
        public short[] ResHist;
        public short[] ComHist;
        public short[] IndHist;
        public short[] MoneyHist;
        public short[] PollutionHist;
        public short[] CrimeHist;
        public short[] MiscHist;

        //Maps
        public int MapSerial { get; private set; }

        public ByteMap2 PopulationDensityMap { get; private set; }
        public ByteMap2 TrafficDensityMap { get; private set; }
        public ByteMap2 PollutionDensityMap { get; private set; }
        public ByteMap2 LandValueMap { get; private set; }
        public ByteMap2 CrimeRateMap { get; private set; }
        public ByteMap4 TerrainDensityMap { get; private set; }
        public ShortMap8 RateOfGrowthMap { get; private set; }
        public ByteMap1 PowerGridMap { get; private set; }
        public ShortMap8 FireStationMap { get; private set; }
        public ShortMap8 FireStationEffectMap { get; private set; }
        public ShortMap8 PoliceStationMap { get; private set; }
        public ShortMap8 PoliceStationEffectMap { get; private set; }
        public ShortMap8 ComRateMap { get; private set; }

        public ByteMap2 TempMap1 { get; private set; }
        public ByteMap2 TempMap2 { get; private set; }
        public ByteMap2 TempMap3 { get; private set; }

        public int NeedHospital { get; private set; }
        public int NeedChurch { get; private set; }

        public float RoadPercentage { get; private set; }
        public float PolicePercentage { get; private set; }
        public float FirePercentage { get; private set; }

        public long RoadValue { get; private set; }
        public long PoliceValue { get; private set; }
        public long FireValue { get; private set; }

        public int MustDrawBudget { get; private set; }

        public Micropolis()
        {
            PopulationDensityMap = new ByteMap2();
            TrafficDensityMap = new ByteMap2();
            PollutionDensityMap = new ByteMap2();
            LandValueMap = new ByteMap2();
            CrimeRateMap = new ByteMap2();
            TerrainDensityMap = new ByteMap4();

            TempMap1 = new ByteMap2();
            TempMap2 = new ByteMap2();
            TempMap3 = new ByteMap2();

            PowerGridMap = new ByteMap1();
            RateOfGrowthMap = new ShortMap8();
            FireStationMap = new ShortMap8();
            FireStationEffectMap = new ShortMap8();
            PoliceStationMap = new ShortMap8();
            PoliceStationEffectMap = new ShortMap8();
            ComRateMap = new ShortMap8();

            init();
        }

        private void init()
        {     
            // short roadTotal;
            RoadTotal = 0;

            // short railTotal;
            RailTotal = 0;

            // short firePop;
            FirePop = 0;

            // short resPop;
            ResPop = 0;

            // short comPop;
            ComPop = 0;

            // short indPop;
            IndPop = 0;

            // short totalPop;
            TotalPop = 0;

            // short totalPopLast;
            TotalPopLast = 0;

            // short resZonePop;
            ResZonePop = 0;

            // short comZonePop;
            ComZonePop = 0;

            // short indZonePop;
            IndZonePop = 0;

            // short totalZonePop;
            TotalZonePop = 0;

            // short hospitalPop;
            HospitalPop = 0;

            // short churchPop;
            ChurchPop = 0;

            // short faith;
            Faith = 0;

            // short stadiumPop;
            StadiumPop = 0;

            // short policeStationPop;
            PoliceStationPop = 0;

            // short fireStationPop;
            FireStationPop = 0;

            // short coalPowerPop;
            CoalPowerPop = 0;

            // short nuclearPowerPop;
            NuclearPowerPop = 0;

            // short seaportPop;
            SeaportPop = 0;

            // short airportPop;
            AirportPop = 0;

            // short needHospital;
            NeedHospital = 0;

            // short needChurch;
            NeedChurch = 0;

            // short crimeAverage;
            CrimeAverage = 0;

            // short pollutionAverage;
            PollutionAverage = 0;

            // short landValueAverage;
            LandValueAverage = 0;

            // Quad cityTime;
            CityTime = 0;

            // Quad cityMonth;
            CityMonth = 0;

            // Quad cityYear;
            CityYear = 0;

            // short startingYear;
            StartingYear = 0;

            Map = new ushort[Constants.WorldWidth, Constants.WorldHeight];

            // short resHist10Max;
            ResHist10Max = 0;

            // short resHist120Max;
            ResHist120Max = 0;

            // short comHist10Max;
            ComHist10Max = 0;

            // short comHist120Max;
            ComHist120Max = 0;

            // short indHist10Max;
            IndHist10Max = 0;

            // short indHist120Max;
            IndHist120Max = 0;

            CensusChanged = false;

            // Quad roadSpend;
            RoadSpend = 0;

            // Quad policeSpend;
            PoliceSpend = 0;

            // Quad fireSpend;
            FireSpend = 0;

            // Quad roadFund;
            RoadFund = 0;

            // Quad policeFund;
            PoliceFund = 0;

            // Quad fireFund;
            FireFund = 0;

            RoadEffect = 0;
            PoliceEffect = 0;
            FireEffect = 0;

            // Quad taxFund;
            TaxFund = 0;

            // short cityTax;
            CityTax = 0;

            // bool taxFlag;
            TaxFlag = false;

            PopulationDensityMap.Clear();
            TrafficDensityMap.Clear();
            PollutionDensityMap.Clear();
            LandValueMap.Clear();
            CrimeRateMap.Clear();
            PowerGridMap.Clear();
            TerrainDensityMap.Clear();
            RateOfGrowthMap.Clear();
            FireStationMap.Clear();
            FireStationEffectMap.Clear();
            PoliceStationMap.Clear();
            PoliceStationEffectMap.Clear();
            ComRateMap.Clear();

            // short *resHist;
            ResHist = new short[Constants.HistoryLength];

            // short *comHist;
            ComHist = new short[Constants.HistoryLength];

            // short *indHist;
            IndHist = new short[Constants.HistoryLength];

            // short *moneyHist;
            MoneyHist = new short[Constants.HistoryLength];

            // short *pollutionHist;
            PollutionHist = new short[Constants.HistoryLength];

            // short *crimeHist;
            CrimeHist = new short[Constants.HistoryLength];

            // short *miscHist;
            MiscHist = new short[Constants.HistoryLength];

            // float roadPercent;
            RoadPercentage = (float)0.0;

            // float policePercent;
            PolicePercentage = (float)0.0;

            // float firePercent;
            FirePercentage = (float)0.0;

            // Quad roadValue;
            RoadValue = 0;

            // Quad policeValue;
            PoliceValue = 0;

            // Quad fireValue;
            FireValue = 0;

            // int mustDrawBudget;
            MustDrawBudget = 0;

            // short floodCount;
            FloodCount = 0;

            // short cityYes;
            CityYes = 0;

            // short problemVotes[PROBNUM]; /* these are the votes for each  */
            ProblemVotes = new int[(int)CityVotingProblems.NumberOfProblems];
            ProblemOrder = new int[(int)CityVotingProblems.CountOfProblemsToComplainAbout];

            // Quad cityPop;
            CityPopulation = 0;

            // Quad cityPopDelta;
            CityPopDelta = 0;

            // Quad cityAssessedValue;
            CityAssessedValue = 0;

            CityClassification = CityClassification.Village;

            // short cityScore;
            CityScore = 0;

            // short cityScoreDelta;
            CityScoreDelta = 0;

            // short trafficAverage;
            TrafficAverage = 0;

            // int TreeLevel; /* level for tree creation */
            TerrainTreeLevel = -1;

            // int LakeLevel; /* level for lake creation */
            TerrainLakeLevel = -1;

            // int CurveLevel; /* level for river curviness */
            TerrainCurveLevel = -1;

            // int CreateIsland; /* -1 => 10%, 0 => never, 1 => always */
            TerrainCreateIsland = -1;

            Graph10Max = 0;
            Graph120Max = 0;

            // int simLoops;
            SimLoops = 0;

            // int simPasses;
            SimPasses = 0;

            // int simPass;
            SimPass = 0;

            SimPaused = false; // Simulation is running

            // int simPausedSpeed;
            SimPausedSpeed = 3;

            // int heatSteps;
            HeatSteps = 0;

            // int heatFlow;
            HeatFlow = -7;

            // int heatRule;
            HeatRule = 0;

            // int heatWrap;
            HeatWrap = 3;

            // std::string cityFileName;
            CityFileName = "";

            // std::string cityName;
            CityName = "";

            // bool tilesAnimated;
            TilesAnimated = false;

            // bool doAnimaton;
            DoAnimation = true;

            // bool doMessages;
            DoMessages = true;

            // bool doNotices;
            DoNotices = true;

            // short *cellSrc;
            CellSrc = 0;

            // short *cellDst;
            CellDest = 0;

            // Quad cityPopLast;
            CityPopulationLast = 0;

            // short categoryLast;
            CategoryLast = 0;

            AutoGoTo = false;

            powerStackPointer = 0;

            // Position powerStackXY[POWER_STACK_SIZE];
            for (int i = 0; i < Constants.PowerStackSize; i++)
            {
                powerStackXY[i] = new Position();
            }

            // UQuad nextRandom;
            nextRandom = 1;

            // char *HomeDir;
            homeDir = "";

            // char *ResourceDir;
            resourceDir = "";

            // Resource *resources;
            Resources = null;

            // StringTable *stringTables;
            StringTables = null;


            ////////////////////////////////////////////////////////////////////////
            // scan.cpp

            // short newMap;
            NewMap = 0;

            // short newMapFlags[MAP_TYPE_COUNT];
            NewMapFlags = new short[(int)MapType.Count];

            // short cityCenterX;
            CityCenterX = 0;

            // short cityCenterY;
            CityCenterY = 0;

            // short pollutionMaxX;
            PollutionMaxX = 0;

            // short pollutionMaxY;
            PollutionMaxY = 0;

            // short crimeMaxX;
            CrimeMaxX = 0;

            // short crimeMaxY;
            CrimeMaxY = 0;

            // Quad donDither;
            DonDither = 0;

            ValveFlag = false;

            // short crimeRamp;
            CrimeRamp = 0;

            // short pollutionRamp;
            PollutionRamp = 0;

            ResCap = false; // Do not block residential growth
            ComCap = false; // Do not block commercial growth
            IndCap = false; // Do not block industrial growth

            // short cashFlow;
            CashFlow = 0;

            // float externalMarket;
            ExternalMarket = (float)4.0;

            DisasterEvent = Scenario.None;

            // short disasterWait;
            DisasterWait = 0;

            ScoreType = Scenario.None;

            // short scoreWait;
            scoreWait = 0;

            // short poweredZoneCount;
            PoweredZoneCount = 0;

            // short unpoweredZoneCount;
            UnpoweredZoneCount = 0;

            NewPower = false;

            // short cityTaxAverage;
            CityTaxAverage = 0;

            // short simCycle;
            SimCycle = 0;

            // short phaseCycle;
            PhaseCycle = 0;

            // short speedCycle;
            SpeedCycle = 0;

            // bool doInitialEval
            DoInitialEval = false;

            // int mapSerial;
            MapSerial = 1;

            // short resValve;
            ResValve = 0;

            // short comValve;
            ComValve = 0;

            // short indValve;
            IndValve = 0;

            //SimSprite *spriteList;
            SpriteList = null;

            // SimSprite *freeSprites;
            freeSprites = null;

            // SimSprite *globalSprites[SPRITE_COUNT];
            globalSprites = new SimSprite[(int)SpriteType.Count];

            // int absDist;
            absDist = 0;

            // short spriteCycle;
            spriteCycle = 0;

            // Quad totalFunds;
            TotalFunds = 0;

            AutoBulldoze = true;

            AutoBudget = true;

            GameLevel = Levels.Easy;

            // short initSimLoad;
            InitSimLoad = 0;

            Scenario = Scenario.None;

            // short simSpeed;
            SimSpeed = 0;

            // short simSpeedMeta;
            SimSpeedMeta = 0;

            EnableSound = false;

            EnableDisasters = true;

            EvalChanged = false;

            // short blinkFlag;
            BlinkFlag = 0;

            // short curMapStackPointer;
            curMapStackPointer = 0;

            // Position curMapStackXY[MAX_TRAFFIC_DISTANCE+1];
            for (int i = 0; i < Constants.MaxTrafficDistance + 1; i++)
            {
                curMapStackXY[i] = new Position();
            }

            // short trafMaxX, trafMaxY;
            trafMaxX = 0;
            trafMaxY = 0;

            MustUpdateFunds = false;

            MustUpdateOptions = false;

            // Quad cityTimeLast;
            CityTimeLast = 0;

            // Quad cityYearLast;
            CityYearLast = 0;

            // Quad cityMonthLast;
            CityMonthLast = 0;

            // Quad totalFundsLast;
            TotalFundsLast = 0;

            // Quad resLast;
            ResLast = 0;

            // Quad comLast;
            ComLast = 0;

            // Quad indLast;
            IndLast = 0;

            SimInit();
        }

        private void destory()
        {
            destoryMapArrays();
        }

        public void Spend(long amount) { SetFunds((int)(TotalFunds - amount)); }        
    }
}
