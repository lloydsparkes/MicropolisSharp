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
            engine.GenerateMap();

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
                    Map = (ushort[,])engine.Map.Clone()
                };

                history.Add(state);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(history, options);
            File.WriteAllText("engine_history.json", jsonString);
        }
    }
}
