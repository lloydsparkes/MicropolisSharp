using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicropolisSharp.Types;
/// <summary>
/// From Initialize.cpp
/// </summary>

namespace MicropolisSharp
{
    public partial class Micropolis
    {
        public void InitWillStuff()
        {
            RandomlySeedRandom();
            InitGraphMax();
            DestroyAllSprites();

            RoadEffect = Constants.MaxRoadEffect;
            PoliceEffect = Constants.MaxPoliceStationEffect;
            FireEffect = Constants.MaxFireStationEffect;
            CityScore = 500;
            CityPopulation = -1;
            CityTimeLast = -1;
            CityYearLast = -1;
            CityMonthLast = -1;
            TotalFundsLast = -1;
            ResLast = ComLast = IndLast = -999999;
            RoadFund = 0;
            PoliceFund = 0;
            FireFund = 0;
            ValveFlag = true;
            DisasterEvent = Scenario.None;
            TaxFlag = false;

            PopulationDensityMap.Clear();
            TrafficDensityMap.Clear();
            PollutionDensityMap.Clear();
            LandValueMap.Clear();
            CrimeRateMap.Clear();
            TerrainDensityMap.Clear();
            RateOfGrowthMap.Clear();
            ComRateMap.Clear();
            PoliceStationMap.Clear();
            PoliceStationEffectMap.Clear();
            FireStationMap.Clear();
            FireStationEffectMap.Clear();

            DoNewGame();
            DoUpdateHeads();
        }

        public void ResetMapState()
        {
            //NO OP
        }

        public void ResetEditorState()
        {
            //NO OP
        }
    }
}
