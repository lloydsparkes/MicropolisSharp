using MicropolisSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Disasters.cpp
/// </summary>

namespace MicropolisSharp
{
    public partial class Micropolis
    {
        public short FloodCount { get; private set; }

        private void DoDisasters()
        { /* Chance of disasters at lev 0 1 2 */
            short[] DisChance = {
                10 * 48, // Game level 0
                5 * 48,  // Game level 1
                60 // Game level 2
            };
            //TODO: Reenable Asserts
            //assert(LEVEL_COUNT == LENGTH_OF(DisChance));

            if (FloodCount.IsTrue())
            {
                FloodCount--;
            }

            if (DisasterEvent != Scenario.None)
            {
                ScenarioDisaster();
            }

            if (!EnableDisasters)
            { // Disasters have been disabled
                return;
            }

            Levels x = GameLevel;
            if (x > Levels.Last)
            {
                x = Levels.Easy;
            }

            if (GetRandom(DisChance[(int)x]).IsFalse())
            {
                switch (GetRandom(8))
                {
                    case 0:
                    case 1:
                        SetFire();  // 2/9 chance a fire breaks out
                        break;

                    case 2:
                    case 3:
                        MakeFlood(); // 2/9 chance for a flood
                        break;

                    case 4:
                        // 1/9 chance nothing happens (was airplane crash,
                        // which EA removed after 9/11, and requested it be
                        // removed from this code)
                        break;

                    case 5:
                        MakeTornado(); // 1/9 chance tornado
                        break;

                    case 6:
                        MakeEarthquake(); // 1/9 chance earthquake
                        break;

                    case 7:
                    case 8:
                        // 2/9 chance a scary monster arrives in a dirty town
                        if (PollutionAverage > /* 80 */ 60)
                        {
                            MakeMonster();
                        }
                        break;
                }
            }
        }

        private void ScenarioDisaster()
        {
            switch (DisasterEvent)
            {
                case Scenario.Dullsville:
                    break;

                case Scenario.SanFrancisco:
                    if (DisasterWait == 1)
                    {
                        MakeEarthquake();
                    }
                    break;

                case Scenario.Hamburg:
                    if (DisasterWait % 10 == 0)
                    {
                        MakeFireBombs();
                    }
                    break;

                case Scenario.Bern:
                    break;

                case Scenario.Tokyo:
                    if (DisasterWait == 1)
                    {
                        MakeMonster();
                    }
                    break;

                case Scenario.Detroit:
                    break;

                case Scenario.Boston:
                    if (DisasterWait == 1)
                    {
                        MakeMeltdown();
                    }
                    break;

                case Scenario.Rio:
                    if ((DisasterWait % 24) == 0)
                    {
                        MakeFlood();
                    }
                    break;
            }

            if (DisasterWait > 0)
            {
                DisasterWait--;
            }
            else
            {
                DisasterEvent = Scenario.None;
            }
        }

        public void MakeMeltdown()
        {
            short x, y;

            for (x = 0; x < (Constants.WorldWidth - 1); x++)
            {
                for (y = 0; y < (Constants.WorldHeight - 1); y++)
                {
                    if ((Map[x,y] & (ushort)MapTileBits.LowMask) == (ushort)MapTileCharacters.NUCLEAR)
                    {
                        DoMeltdown(new Position(x, y));
                        return;
                    }
                }
            }
        }

        public void FireBomb()
        {
            int crashX = GetRandom(Constants.WorldWidth - 1);
            int crashY = GetRandom(Constants.WorldHeight - 1);
            MakeExplosion(crashX, crashY);
            SendMessage(GeneralMessages.MESSAGE_FIREBOMBING, (short)crashX, (short)crashY, true, true);
        }

        public void MakeFireBombs()
        {
            int count = 2 + (GetRandom16() & 1);

            while (count > 0)
            {
                FireBomb();
                count--;
            }

            // TODO: Schedule periodic fire bombs over time, every few ticks.
        }

        public void MakeEarthquake()
        {
            short x, y, z;

            int strength = GetRandom(700) + 300; // strength/duration of the earthquake

            DoEarthquake(strength);

            SendMessage(GeneralMessages.MESSAGE_EARTHQUAKE, CityCenterX, CityCenterY, true, false);

            for (z = 0; z < strength; z++)
            {
                x = GetRandom(Constants.WorldWidth - 1);
                y = GetRandom(Constants.WorldHeight - 1);

                if (Vulnerable(Map[x,y]))
                {

                    if ((z & 0x3) != 0)
                    { // 3 of 4 times reduce to rubble
                        Map[x,y] = RandomRubble();
                    }
                    else
                    {
                        // 1 of 4 times start fire
                        Map[x,y] = RandomFire();
                    }
                }
            }
        }

        public void SetFire()
        {
            short x, y, z;

            x = GetRandom(Constants.WorldWidth - 1);
            y = GetRandom(Constants.WorldHeight - 1);
            z = (short)Map[x,y];

            if ((z & (short)MapTileBits.CenterOfZone) == 0)
            {
                z = (short)(z & (short)MapTileBits.LowMask);
                if (z > (short)MapTileCharacters.LHTHR && z < (short)MapTileCharacters.LASTZONE)
                {
                    Map[x,y] = RandomFire();
                    SendMessage(GeneralMessages.MESSAGE_FIRE_REPORTED, x, y, true, false);
                }
            }
        }

        public void MakeFire()
        {
            short t, x, y, z;

            for (t = 0; t < 40; t++)
            {
                x = GetRandom(Constants.WorldWidth - 1);
                y = GetRandom(Constants.WorldHeight - 1);
                z = (short)Map[x,y];

                if (((z & (short)MapTileBits.CenterOfZone).IsFalse()) && (z & (short)MapTileBits.Burnable).IsTrue())
                {
                    z = (short)(z & (short)MapTileBits.LowMask);
                    if ((z > 21) && (z < (short)MapTileCharacters.LASTZONE))
                    {
                        Map[x,y] = RandomFire();
                        SendMessage(GeneralMessages.MESSAGE_FIRE_REPORTED, x, y, false, false);
                        return;
                    }
                }
            }
        }

        public void MakeFlood()
        {
            short[] Dx = { 0, 1, 0, -1 };
            short[] Dy = { -1, 0, 1, 0 };
            short xx, yy, c;
            short z, t, x, y;

            for (z = 0; z < 300; z++)
            {
                x = GetRandom(Constants.WorldWidth - 1);
                y = GetRandom(Constants.WorldHeight - 1);
                c = (short)(Map[x,y] & (short)MapTileBits.LowMask);

                if (c > (short)MapTileCharacters.CHANNEL && c <= (short)MapTileCharacters.WATER_HIGH)
                { /* if riveredge  */
                    for (t = 0; t < 4; t++)
                    {
                        xx = (short)(x + Dx[t]);
                        yy = (short)(y + Dy[t]);
                        if (Position.TestBounds(xx, yy))
                        {
                            c = (short)Map[xx,yy];

                            /* tile is floodable */
                            if (c == (short)MapTileCharacters.DIRT
                                  || (c & ((short)MapTileBits.Bulldozable | (short)MapTileBits.Burnable)) == ((short)MapTileBits.Bulldozable | (short)MapTileBits.Burnable))
                            {
                                Map[xx,yy] = (ushort)MapTileCharacters.FLOOD;
                                FloodCount = 30;
                                SendMessage(GeneralMessages.MESSAGE_FLOODING_REPORTED, xx, yy, true, false);
                                return;
                            }
                        }
                    }
                }
            }
        }

        private bool Vulnerable(int tem)
        {
            int tem2 = tem & (short)MapTileBits.LowMask;

            if (tem2 < (short)MapTileCharacters.RESBASE || tem2 > (short)MapTileCharacters.LASTZONE || (tem & (short)MapTileBits.CenterOfZone).IsTrue())
            {
                return false;
            }

            return true;
        }

        private void DoFlood(Position pos)
        {
            short[] Dx = { 0, 1, 0, -1 };
            short[] Dy = { -1, 0, 1, 0 };

            if (FloodCount > 0)
            {
                // Flood is not over yet
                for (int z = 0; z < 4; z++)
                {
                    if ((GetRandom16() & 7) == 0)
                    { // 12.5% chance
                        int xx = pos.X + Dx[z];
                        int yy = pos.Y + Dy[z];
                        if (Position.TestBounds(xx, yy))
                        {
                            ushort c = Map[xx,yy];
                            ushort t = (ushort)(c & (ushort)MapTileBits.LowMask);

                            if ((c & (ushort)MapTileBits.Burnable) == (ushort)MapTileBits.Burnable || c == (ushort)MapTileCharacters.DIRT
                                                    || (t >= (ushort)MapTileCharacters.WOODS5 && t < (ushort)MapTileCharacters.FLOOD))
                            {
                                if ((c & (ushort)MapTileBits.CenterOfZone) == (ushort)MapTileBits.CenterOfZone)
                                {
                                    FireZone(new Position(xx, yy), c);
                                }
                                Map[xx,yy] = (ushort)(MapTileCharacters.FLOOD + GetRandom(2));
                            }
                        }
                    }
                }
            }
            else
            {
                if ((GetRandom16() & 15) == 0)
                { // 1/16 chance
                    Map[pos.X,pos.Y] = (ushort)MapTileCharacters.DIRT;
                }
            }
        }
    }
}
