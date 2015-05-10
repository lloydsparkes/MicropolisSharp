using System.Collections.Generic;

/// <summary>
/// From Tool.h & Tool.cpp
/// </summary>
/// 
namespace MicropolisSharp.Types
{
    public class ToolEffects
    {
        private Micropolis simulator;
        private int cost;
        private Dictionary<Position, ushort> modificationMap = new Dictionary<Position, ushort>();
        private List<FrontendMessage> messages = new List<FrontendMessage>();

        public ToolEffects(Micropolis sim)
        {
            this.simulator = sim;
            Clear();
        }

        public void Clear()
        {
            cost = 0;
            modificationMap.Clear();
            messages.Clear();
        }

        public void ModifyWorld()
        {
            simulator.Spend(cost);
            simulator.UpdateFunds();

            foreach(KeyValuePair<Position, ushort> entry in modificationMap)
            {
                simulator.Map[entry.Key.X, entry.Key.Y] = entry.Value;
            }

            foreach(FrontendMessage msg in messages)
            {
                msg.SendMessage(simulator);
            }
            Clear();
        }

        public bool ModifyIfEnoughFunding()
        {
            if(simulator.TotalFunds < cost)
            {
                return false;
            }
            ModifyWorld();
            return true;
        }

        public ushort GetMapValue(Position pos)
        {
            if (modificationMap.ContainsKey(pos))
            {
                return modificationMap[pos];
            }
            return simulator.Map[pos.X, pos.Y];
        }

        public ushort GetMapValue(int x, int y) { return GetMapValue(new Position(x, y)); }
        public ushort GetMapTile(Position pos) { return (ushort)(GetMapValue(pos) & (ushort)MapTileBits.LowMask); }
        public ushort GetMapTile(int x, int y) { return (ushort)(GetMapValue(x, y) & (ushort)MapTileBits.LowMask); }
        public int GetCost() { return cost; }

        public void AddCost(int amount) { cost += amount;  }
        public void SetMapValue(Position pos, ushort mapVal) { }
        public void SetMapValue(int x, int y, ushort mapVal) { SetMapValue(new Position(x, y), mapVal); }
        public void AddFrontendMessage(FrontendMessage message) { messages.Add(message); }
    }
}
