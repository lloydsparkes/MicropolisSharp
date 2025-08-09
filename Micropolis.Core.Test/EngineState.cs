namespace Micropolis.Core.Test
{
    public class EngineState
    {
        public long CityTime { get; set; }
        public int TotalPop { get; set; }
        public long TotalFunds { get; set; }
        public int ResPop { get; set; }
        public int ComPop { get; set; }
        public int IndPop { get; set; }
        public short ResValve { get; set; }
        public short ComValve { get; set; }
        public short IndValve { get; set; }
        public int CrimeAverage { get; set; }
        public int PollutionAverage { get; set; }
        public int LandValueAverage { get; set; }
        public long RoadFund { get; set; }
        public long PoliceFund { get; set; }
        public long FireFund { get; set; }
        public long RoadSpend { get; set; }
        public long PoliceSpend { get; set; }
        public long FireSpend { get; set; }
        public short PhaseCycle { get; set; }
        public ushort[,] Map { get; set; }
    }
}
