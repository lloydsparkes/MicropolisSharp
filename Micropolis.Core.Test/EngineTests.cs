using MicropolisSharp;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Xunit;

namespace Micropolis.Core.Test
{
    public class EngineTests
    {
        [Fact]
        public void RunSimulationAndCaptureState()
        {
            Micropolis engine = new Micropolis();
            engine.SimInit();

            string cityPath = "../../../Micropolis.Windows/cities/kobe.cty";
            bool success = engine.LoadCity(cityPath);
            Assert.True(success, "Failed to load city file.");

            List<EngineState> history = new List<EngineState>();

            for (int i = 0; i < 100; i++)
            {
                engine.SimTick();

                var state = new EngineState
                {
                    CityTime = engine.CityTime,
                    TotalPop = engine.TotalPop,
                    TotalFunds = engine.TotalFunds,
                    ResPop = engine.ResPop,
                    ComPop = engine.ComPop,
                    IndPop = engine.IndPop,
                    ResValve = engine.ResValve,
                    ComValve = engine.ComValve,
                    IndValve = engine.IndValve,
                    CrimeAverage = engine.CrimeAverage,
                    PollutionAverage = engine.PollutionAverage,
                    LandValueAverage = engine.LandValueAverage,
                    RoadFund = engine.RoadFund,
                    PoliceFund = engine.PoliceFund,
                    FireFund = engine.FireFund,
                    RoadSpend = engine.RoadSpend,
                    PoliceSpend = engine.PoliceSpend,
                    FireSpend = engine.FireSpend,
                    PhaseCycle = engine.PhaseCycle,
                    Map = ConvertMapToJagged(engine.Map),
                    PowerGridMap = ConvertPowerGridMapToJagged(engine.PowerGridMap),
                    AsciiPowerMap = GenerateAsciiPowerMap(engine.PowerGridMap)
                };

                history.Add(state);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(history, options);
            File.WriteAllText("engine_history.json", jsonString);
        }

        private ushort[][] ConvertMapToJagged(ushort[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            ushort[][] jaggedMap = new ushort[height][];

            for (int y = 0; y < height; y++)
            {
                jaggedMap[y] = new ushort[width];
                for (int x = 0; x < width; x++)
                {
                    jaggedMap[y][x] = map[x, y];
                }
            }

            return jaggedMap;
        }

        private byte[][] ConvertPowerGridMapToJagged(MicropolisSharp.Types.ByteMap1 powerGridMap)
        {
            var width = powerGridMap.width;
            var height = powerGridMap.height;
            var data1D = powerGridMap.getBase();
            var jaggedMap = new byte[height][];

            for (int y = 0; y < height; y++)
            {
                jaggedMap[y] = new byte[width];
                for (int x = 0; x < width; x++)
                {
                    jaggedMap[y][x] = data1D[x * height + y];
                }
            }

            return jaggedMap;
        }

        private string GenerateAsciiPowerMap(MicropolisSharp.Types.ByteMap1 powerGridMap)
        {
            var width = powerGridMap.width;
            var height = powerGridMap.height;
            var data = powerGridMap.getBase();
            var sb = new System.Text.StringBuilder();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var value = data[x * height + y];
                    sb.Append(value > 0 ? '#' : ' ');
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
