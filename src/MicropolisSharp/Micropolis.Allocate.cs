using MicropolisSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// From Allocate.cpp
/// </summary>

namespace MicropolisSharp
{
    public partial class Micropolis
    {
        private void InitMapArrays()
        {
            Map = new ushort[Constants.WorldWidth, Constants.WorldHeight];

            ResHist = new short[Constants.HistoryLength];
            ComHist = new short[Constants.HistoryLength];
            IndHist = new short[Constants.HistoryLength];
            MoneyHist = new short[Constants.HistoryLength];
            PollutionHist = new short[Constants.HistoryLength];
            CrimeHist = new short[Constants.HistoryLength];
            MiscHist = new short[Constants.MiscHistoryLength];

            PopulationDensityMap = new ByteMap2();
            TrafficDensityMap = new ByteMap2();
            PollutionDensityMap = new ByteMap2();
            LandValueMap = new ByteMap2();
            CrimeRateMap = new ByteMap2();
            TerrainDensityMap = new ByteMap4();
            RateOfGrowthMap = new ShortMap8();
            PowerGridMap = new ByteMap1();
            FireStationMap = new ShortMap8();
            FireStationEffectMap = new ShortMap8();
            PoliceStationMap = new ShortMap8();
            PoliceStationEffectMap = new ShortMap8();
            ComRateMap = new ShortMap8();
        }

        /*public ByteMap2 PopulationDensityMap { get; private set; }
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
        public ShortMap8 ComRateMap { get; private set; }*/

        private void destoryMapArrays()
        {
            InitMapArrays();

            for(int x = 0; x < Constants.WorldWidth; ++x)
            {
                for(int y = 0; y < Constants.WorldHeight; ++y)
                {
                    Map[x, y] = (ushort)MapTileCharacters.DIRT;
                }
            }

            PopulationDensityMap.Clear();
            TrafficDensityMap.Clear();
            PollutionDensityMap.Clear();
            LandValueMap.Clear();
            CrimeRateMap.Clear();
            TerrainDensityMap.Clear();
            RateOfGrowthMap.Clear();
            PowerGridMap.Clear();
            FireStationMap.Clear();
            FireStationEffectMap.Clear();
            PoliceStationMap.Clear();
            PoliceStationEffectMap.Clear();
            ComRateMap.Clear();

            TempMap1.Clear();
            TempMap2.Clear();
            TempMap3.Clear();
        }
    }
}
