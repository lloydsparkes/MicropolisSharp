using MicropolisSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// From Stub.cpp
/// </summary>

namespace MicropolisSharp
{
    public partial class Micropolis
    {
        public void InvalidateMaps()
        {
            MapSerial++;
            Callback("update", "s", "map");
        }

        public void MakeSound(String channel, String sound, int x, int y)
        {
            Callback("makeSound", "ssdd", channel, sound, x.ToString(), y.ToString());
        }

        public void SetFunds(int dollars)
        {
            TotalFunds = dollars;
            UpdateFunds();
        }

        public long TickCount()
        {
            return DateTime.Now.Ticks;
        }

        public void InitGame()
        {
            SimPaused = false;
            SimPausedSpeed = 0;
            SimPass = 0;
            SimPasses = 1;
            HeatSteps = 0;
            SetSpeed(0);
        }

        public void Callback(String name, params String[] values)
        {
            //TODO: Implement Call Backs - its not fullr requires atm
            throw new NotImplementedException();
        }

        public void DoEarthquake(int strength)
        {
            MakeSound("city", "ExplosionLow", Constants.NoWhere, Constants.NoWhere); // Make the sound all over.

            Callback("startEarthquake", "d", strength.ToString());
        }

        public int GetTile(int x, int y)
        {
            if (!Position.TestBounds(x, y))
            {
                return (ushort)MapTileCharacters.DIRT;
            }

            return Map[x,y];
        }

        public void SetTile(int x, int y, int tile)
        {
            if (!Position.TestBounds(x, y))
            {
                return;
            }

            Map[x,y] = (ushort)tile;
        }

        public ushort[,] GetMapBuffer()
        {
            return Map;
        }

        public int GetPowerGrid(int x, int y)
        {
            return PowerGridMap.WorldGet(x, y);
        }

        public void SetPowerGrid(int x, int y, int power)
        {
            PowerGridMap.WorldSet(x, y, (byte)power);
        }

        public ByteMap1 GetPowerGridMapBuffer()
        {
            return PowerGridMap;
        }

        public int GetPopulationDensity(int x, int y)
        {
            return PopulationDensityMap.Get(x, y);
        }

        public void SetPopulationDensity(int x, int y, int density)
        {
            PopulationDensityMap.Set(x, y, (byte)density);
        }

        public ByteMap2 GetPopulationDensityMapBuffer()
        {
            return PopulationDensityMap;
        }

        public int GetRateOfGrowth(int x, int y)
        {
            return RateOfGrowthMap.Get(x, y);
        }

        public void SetRateOfGrowth(int x, int y, int rate)
        {
            RateOfGrowthMap.Set(x, y, (short)rate);
        }

        public ShortMap8 GetRateOfGrowthMapBuffer()
        {
            return RateOfGrowthMap;
        }

        int GetTrafficDensity(int x, int y)
        {
            return TrafficDensityMap.Get(x, y);
        }

        void SetTrafficDensity(int x, int y, int density)
        {
            TrafficDensityMap.Set(x, y, (byte)density);
        }

        public ByteMap2 GetTrafficDensityMapBuffer()
        {
            return TrafficDensityMap;
        }

        public int GetPollutionDensity(int x, int y)
        {
            return PollutionDensityMap.Get(x, y);
        }

        public void SetPollutionDensity(int x, int y, int density)
        {
            PollutionDensityMap.Set(x, y, (byte)density);
        }

        public ByteMap2 GetPollutionDensityMapBuffer()
        {
            return PollutionDensityMap;
        }

        public int GetCrimeRate(int x, int y)
        {
            return CrimeRateMap.Get(x, y);
        }

        public void SetCrimeRate(int x, int y, int rate)
        {
            CrimeRateMap.Set(x, y, (byte)rate);
        }

        public ByteMap2 getCrimeRateMapBuffer()
        {
            return CrimeRateMap;
        }

        public int GetLandValue(int x, int y)
        {
            return LandValueMap.Get(x, y);
        }

        public void SetLandValue(int x, int y, int value)
        {
            LandValueMap.Set(x, y, (byte)value);
        }

        public ByteMap2 GetLandValueMapBuffer()
        {
            return LandValueMap;
        }

        public int GetFireCoverage(int x, int y)
        {
            return FireStationEffectMap.Get(x, y);
        }

        public void SetFireCoverage(int x, int y, int coverage)
        {
            FireStationEffectMap.Set(x, y, (short)coverage);
        }

        public ShortMap8 getFireCoverageMapBuffer()
        {
            return FireStationEffectMap;
        }

        public int GetPoliceCoverage(int x, int y)
        {
            return PoliceStationEffectMap.Get(x, y);
        }

        public void SetPoliceCoverage(int x, int y, int coverage)
        {
            PoliceStationEffectMap.Set(x, y, (short)coverage);
        }

        public ShortMap8 GetPoliceCoverageMapBuffer()
        {
            return PoliceStationEffectMap;
        }
    }
}
