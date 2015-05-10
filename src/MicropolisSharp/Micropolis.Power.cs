using MicropolisSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// From Power.cpp
/// </summary>

namespace MicropolisSharp
{
    public partial class Micropolis
    {
        private int powerStackPointer = 0;
        private Position[] powerStackXY = new Position[Constants.PowerStackSize];

        public void DoPowerScan()
        {
            Direction anyDir, dir;
            int conNum;

            // Clear power map.
            PowerGridMap.Clear();

            // Power that the combined coal and nuclear power plants can deliver.
            long maxPower = CoalPowerPop * Constants.CoalPowerStrength +
                            NuclearPowerPop * Constants.NuclearPowerStrength;

            long numPower = 0; // Amount of power used.

            while (powerStackPointer > 0)
            {
                Position pos = PullPowerStack();
                anyDir = Direction.Invalid;
                do
                {
                    numPower++;
                    if (numPower > maxPower)
                    {
                        SendMessage(GeneralMessages.MESSAGE_NOT_ENOUGH_POWER, Constants.NoWhere, Constants.NoWhere, false, false);
                        return;
                    }
                    if (anyDir != Direction.Invalid)
                    {
                        pos.Move(anyDir);
                    }
                    PowerGridMap.WorldSet(pos.X, pos.Y, 1);
                    conNum = 0;
                    dir = Direction.Begin;
                    while (dir < Direction.End && conNum < 2)
                    {
                        if (TestForConductive(pos, dir))
                        {
                            conNum++;
                            anyDir = dir;
                        }
                        dir = dir.Increment90();
                    }
                    if (conNum > 1)
                    {
                        PushPowerStack(pos);
                    }
                } while (conNum.IsTrue());
            }
        }

        public bool TestForConductive(Position pos, Direction testDir)
        {
            Position movedPos = new Position(pos);

            if (movedPos.Move(testDir))
            {
                if ((Map[movedPos.X,movedPos.Y] & (ushort)MapTileBits.Conductivity) == (ushort)MapTileBits.Conductivity)
                {
                    if (PowerGridMap.WorldGet(movedPos.X, movedPos.Y).IsFalse())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void PushPowerStack(Position pos)
        {
            if (powerStackPointer < (Constants.PowerStackSize - 2))
            {
                powerStackPointer++;
                powerStackXY[powerStackPointer] = pos;
            }
        }

        public Position PullPowerStack()
        {
            //TODO: Make this an assert
            if(powerStackPointer > 0)
            {
                powerStackPointer--;
                return powerStackXY[powerStackPointer + 1];
            }
            //TODO: Change this to an Assert really
            return null;
        }
    }
}
