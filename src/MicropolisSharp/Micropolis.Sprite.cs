﻿/* Micropolis.Sprite.cs
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
using System.Collections.Generic;

namespace MicropolisSharp
{
    /// <summary>
    /// Partial Class Containing the content of sprite.cpp
    /// </summary>
    public partial class Micropolis
    {
        public List<SimSprite> SpriteList { get; private set; }

        private Stack<SimSprite> freeSprites;
        private SimSprite[] globalSprites;
        private int absDist;
        private short spriteCycle;

        public SimSprite NewSprite(string name, SpriteType type, int x, int y)
        {
            SimSprite sprite;

            // If a sprite is available at the pool, use one.
            // else, allocate a new one.
            if (freeSprites != null && freeSprites.Count > 0)
            {
                sprite = freeSprites.Pop();
            }
            else
            {
                sprite = new SimSprite();
            }

            sprite.Name = name;
            sprite.Type = type;

            InitSprite(sprite, x, y);

            SpriteList.Add(sprite);

            return sprite;
        }

        public void InitSprite(SimSprite sprite, int x, int y) {
            sprite.X = x;
            sprite.Y = y;
            sprite.Frame = 0;
            sprite.OrigX = 0;
            sprite.OrigY = 0;
            sprite.DestX = 0;
            sprite.DestY = 0;
            sprite.Count = 0;
            sprite.SoundCount = 0;
            sprite.Dir = 0;
            sprite.NewDir = 0;
            sprite.Step = 0;
            sprite.Flag = 0;
            sprite.Control = -1;
            sprite.Turn = 0;
            sprite.Accel = 0;
            sprite.Speed = 100;

            if (globalSprites[(int)sprite.Type] == null)
            {
                globalSprites[(int)sprite.Type] = sprite;
            }

            switch (sprite.Type)
            {

                case SpriteType.Train:
                    sprite.Width = 32;
                    sprite.Height = 32;
                    sprite.XOffset = 32;
                    sprite.YOffset = -16;
                    sprite.XHot = 40;
                    sprite.YHot = -8;
                    sprite.Frame = 1;
                    sprite.Dir = 4;
                    break;

                case SpriteType.Ship:
                    sprite.Width = 48;
                    sprite.Height = 48;
                    sprite.XOffset = 32;
                    sprite.YOffset = -16;
                    sprite.XHot = 48;
                    sprite.YHot = 0;

                    if (x < (4 << 4))
                    {
                        sprite.Frame = 3;
                    }
                    else if (x >= ((Constants.WorldWidth - 4) << 4))
                    {
                        sprite.Frame = 7;
                    }
                    else if (y < (4 << 4))
                    {
                        sprite.Frame = 5;
                    }
                    else if (y >= ((Constants.WorldHeight - 4) << 4))
                    {
                        sprite.Frame = 1;
                    }
                    else
                    {
                        sprite.Frame = 3;
                    }

                    sprite.NewDir = sprite.Frame;
                    sprite.Dir = 10;
                    sprite.Count = 1;
                    break;

                case SpriteType.Monster:
                    sprite.Width = 48;
                    sprite.Height = 48;
                    sprite.XOffset = 24;
                    sprite.YOffset = 0;
                    sprite.XHot = 40;
                    sprite.YHot = 16;

                    if (x > ((Constants.WorldWidth << 4) / 2))
                    {
                        if (y > ((Constants.WorldHeight << 4) / 2))
                        {
                            sprite.Frame = 10;
                        }
                        else
                        {
                            sprite.Frame = 7;
                        }
                    }
                    else if (y > ((Constants.WorldHeight << 4) / 2))
                    {
                        sprite.Frame = 1;
                    }
                    else
                    {
                        sprite.Frame = 4;
                    }

                    sprite.Count = 1000;
                    sprite.DestX = PollutionMaxX << 4;
                    sprite.DestY = PollutionMaxY << 4;
                    sprite.OrigX = sprite.X;
                    sprite.OrigY = sprite.Y;
                    break;

                case SpriteType.Helicopter:
                    sprite.Width = 32;
                    sprite.Height = 32;
                    sprite.XOffset = 32;
                    sprite.YOffset = -16;
                    sprite.XHot = 40;
                    sprite.YHot = -8;
                    sprite.Frame = 5;
                    sprite.Count = 1500;
                    sprite.DestX = GetRandom((Constants.WorldWidth << 4) - 1);
                    sprite.DestY = GetRandom((Constants.WorldHeight << 4) - 1);
                    sprite.OrigX = x - 30;
                    sprite.OrigY = y;
                    break;

                case SpriteType.Airplane:
                    sprite.Width = 48;
                    sprite.Height = 48;
                    sprite.XOffset = 24;
                    sprite.YOffset = 0;
                    sprite.XHot = 48;
                    sprite.YHot = 16;
                    if (x > ((Constants.WorldWidth - 20) << 4))
                    {
                        sprite.X -= 100 + 48;
                        sprite.DestX = sprite.X - 200;
                        sprite.Frame = 7;
                    }
                    else
                    {
                        sprite.DestX = sprite.X + 200;
                        sprite.Frame = 11;
                    }
                    sprite.DestY = sprite.Y;
                    break;

                case SpriteType.Tornado:
                    sprite.Width = 48;
                    sprite.Height = 48;
                    sprite.XOffset = 24;
                    sprite.YOffset = 0;
                    sprite.XHot = 40;
                    sprite.YHot = 36;
                    sprite.Frame = 1;
                    sprite.Count = 200;
                    break;

                case SpriteType.Explosion:
                    sprite.Width = 48;
                    sprite.Height = 48;
                    sprite.XOffset = 24;
                    sprite.YOffset = 0;
                    sprite.XHot = 40;
                    sprite.YHot = 16;
                    sprite.Frame = 1;
                    break;

                case SpriteType.Bus:
                    sprite.Width = 32;
                    sprite.Height = 32;
                    sprite.XOffset = 30;
                    sprite.YOffset = -18;
                    sprite.XHot = 40;
                    sprite.YHot = -8;
                    sprite.Frame = 1;
                    sprite.Dir = 1;
                    break;

            }
        }

        public void DestroyAllSprites()
        {
            foreach(SimSprite sprite in SpriteList)
            {
                sprite.Frame = 0;
            }
        }

        public void DestorySprite(SimSprite sprite)
        {
            if (globalSprites[(int)sprite.Type] == sprite)
            {
                globalSprites[(int)sprite.Type] = null;
            }

            if (sprite.Name != null)
            {
                sprite.Name = null;
            }
        }

        public SimSprite GetSprite(SpriteType type)
        {
            SimSprite sprite = globalSprites[(int)type];
            if (sprite == null || sprite.Frame == 0)
            {
                return null;
            }
            else
            {
                return sprite;
            }
        }

        public SimSprite MakeSprite(SpriteType type, int x, int y)
        {
            SimSprite sprite;

            sprite = globalSprites[(int)type];
            if (sprite == null)
            {
                sprite = NewSprite("", type, x, y);
            }
            else
            {
                InitSprite(sprite, x, y);
            }
            return sprite;
        }

        public short GetChar(int x, int y)
        {     // Convert sprite coordinates to tile coordinates.
            x >>= 4;
            y >>= 4;

            if (!Position.TestBounds(x, y))
            {
                return -1;
            }
            else
            {
                return (short)(Map[x,y] & (ushort)MapTileBits.LowMask);
            }
        }

        public short TurnTo(int p, int d)
        {
            if (p == d)
            {
                return (short)p;
            }

            if (p < d)
            {
                if (d - p < 4)
                {
                    p++;
                }
                else
                {
                    p--;
                }
            }
            else
            {
                if (p - d < 4)
                {
                    p--;
                }
                else
                {
                    p++;
                }
            }

            if (p > 8)
            {
                p = 1;
            }

            if (p < 1)
            {
                p = 8;
            }

            return (short)p;
        }

        public bool TryOther(int Tpoo, int Told, int Tnew)
        {
            int z;

            z = Told + 4;

            if (z > 8)
            {
                z -= 8;
            }

            if (Tnew != z)
            {
                return false;
            }

            if (Tpoo == (int)MapTileCharacters.POWERBASE || Tpoo == (int)MapTileCharacters.POWERBASE + 1
                  || Tpoo == (int)MapTileCharacters.RAILBASE || Tpoo == (int)MapTileCharacters.RAILBASE + 1)
            {
                return true;
            }

            return false;
        }

        public bool SpriteNotInBounds(SimSprite sprite)
        {
            int x = sprite.X + sprite.XHot;
            int y = sprite.Y + sprite.YHot;

            return x < 0 || y < 0 || x >= (Constants.WorldWidth << 4) || y >= (Constants.WorldHeight << 4);
        }

        public short GetDir(int orgX, int orgY, int desX, int desY)
        {
            short[] Gdtab = { 0, 3, 2, 1, 3, 4, 5, 7, 6, 5, 7, 8, 1 };
            int dispX, dispY, z;

            dispX = desX - orgX;
            dispY = desY - orgY;

            if (dispX < 0)
            {
                if (dispY < 0)
                {
                    z = 11;
                }
                else
                {
                    z = 8;
                }
            }
            else
            {
                if (dispY < 0)
                {
                    z = 2;
                }
                else
                {
                    z = 5;
                }
            }

            dispX = Math.Abs(dispX);
            dispY = Math.Abs(dispY);
            absDist = dispX + dispY;

            if (dispX * 2 < dispY)
            {
                z++;
            }
            else if (dispY * 2 < dispY)
            {  // XXX This never holds!!
                z--;
            }

            if (z < 0 || z > 12)
            {
                z = 0;
            }

            return Gdtab[z];
        }

        public int GetDistance(int x1, int y1, int x2, int y2) { return Math.Abs(x1 - x2) + Math.Abs(y1 - y2); }

        public bool CheckSpriteCollision(SimSprite s1, SimSprite s2)
        {
            return s1.Frame != 0 && s2.Frame != 0 && GetDistance(s1.X + s1.XHot, s1.Y + s1.YHot, s2.X + s2.XHot, s2.Y + s2.YHot) < 30;
        }

        public void MoveObjects()
        {
            if (!SimSpeed.IsTrue())
            {
                return;
            }

            spriteCycle++;

            foreach(SimSprite sprite in SpriteList)
            {
                if (sprite.Frame > 0)
                {
                    switch (sprite.Type)
                    {

                        case SpriteType.Train:
                            DoTrainSprite(sprite);
                            break;

                        case SpriteType.Helicopter:
                            DoCopterSprite(sprite);
                            break;

                        case SpriteType.Airplane:
                            DoAirplaneSprite(sprite);
                            break;

                        case SpriteType.Ship:
                            DoShipSprite(sprite);
                            break;

                        case SpriteType.Monster:
                            DoMonsterSprite(sprite);
                            break;

                        case SpriteType.Tornado:
                            DoTornadoSprite(sprite);
                            break;

                        case SpriteType.Explosion:
                            DoExplosionSprite(sprite);
                            break;

                        case SpriteType.Bus:
                            DoBusSprite(sprite);
                            break;

                    }
                }
                else
                {
                    if (String.IsNullOrWhiteSpace(sprite.Name))
                    {
                        SimSprite s = sprite;
                        DestorySprite(s);
                    }
                }
            }
        }

        public void DoTrainSprite(SimSprite sprite)
        {     
            /* Offset in pixels of sprite x and y to map tile */
            short[] Cx = { 0, 16, 0, -16 };
            short[] Cy = { -16, 0, 16, 0 };
            /* X and Y movement of the sprite in pixels */
            short[] Dx = { 0, 4, 0, -4, 0 };
            short[] Dy = { -4, 0, 4, 0, 0 };

            short[] TrainPic2 = { 1, 2, 1, 2, 5 };
            int z, dir, dir2;
            int c;

            if (sprite.Frame == 3 || sprite.Frame == 4)
            {
                sprite.Frame = TrainPic2[sprite.Dir];
            }

            sprite.X += Dx[sprite.Dir];
            sprite.Y += Dy[sprite.Dir];

            if ((spriteCycle & 3) == 0)
            {

                dir = GetRandom16() & 3;
                for (z = dir; z < dir + 4; z++)
                {
                    dir2 = z & 3;

                    if (sprite.Dir != 4)
                    {
                        if (dir2 == ((sprite.Dir + 2) & 3))
                        {
                            continue;
                        }
                    }

                    c = GetChar(sprite.X + Cx[dir2] + 48, sprite.Y + Cy[dir2]);

                    if ((c >= (ushort)MapTileCharacters.RAILBASE && c <= (ushort)MapTileCharacters.LASTRAIL) /* track? */
                              || c == (ushort)MapTileCharacters.RAILVPOWERH || c == (ushort)MapTileCharacters.RAILHPOWERV)
                    {

                        if (sprite.Dir != dir2 && sprite.Dir != 4)
                        {

                            if (sprite.Dir + dir2 == 3)
                            {
                                sprite.Frame = 3;
                            }
                            else
                            {
                                sprite.Frame = 4;
                            }

                        }
                        else
                        {
                            sprite.Frame = TrainPic2[dir2];
                        }

                        if (c == (ushort)MapTileCharacters.HRAIL || c == (ushort)MapTileCharacters.VRAIL)
                        {
                            sprite.Frame = 5;
                        }

                        sprite.Dir = dir2;
                        return;
                    }
                }

                if (sprite.Dir == 4)
                {
                    sprite.Frame = 0;
                    return;
                }

                sprite.Dir = 4;
            }
        }

        public void DoCopterSprite(SimSprite sprite)
        {
            short[] CDx = { 0, 0, 3, 5, 3, 0, -3, -5, -3 };
            short[] CDy = { 0, -5, -3, 0, 3, 5, 3, 0, -3 };

            if (sprite.SoundCount > 0)
            {
                sprite.SoundCount--;
            }

            if (sprite.Control < 0)
            {

                if (sprite.Count > 0)
                {
                    sprite.Count--;
                }

                if (sprite.Count == 0)
                {

                    /* Attract copter to monster so it blows up more often */
                    SimSprite s = GetSprite(SpriteType.Monster);

                    if (s != null)
                    {
                        sprite.DestX = s.X;
                        sprite.DestY = s.Y;
                    }
                    else
                    {

                        /* Attract copter to tornado so it blows up more often */
                        s = GetSprite(SpriteType.Tornado);

                        if (s != null)
                        {
                            sprite.DestX = s.X;
                            sprite.DestY = s.Y;
                        }
                        else
                        {
                            sprite.DestX = sprite.OrigX;
                            sprite.DestY = sprite.OrigY;
                        }

                    }
                }

                if (sprite.Count == 0)
                { 
                    /* land */
                    GetDir(sprite.X, sprite.Y, sprite.OrigX, sprite.OrigY);

                    if (absDist < 30)
                    {
                        sprite.Frame = 0;
                        return;
                    }

                }

            }
            else
            {

                GetDir(sprite.X, sprite.Y, sprite.DestX, sprite.DestY);

                if (absDist < 16)
                {
                    sprite.DestX = sprite.OrigX;
                    sprite.DestY = sprite.OrigY;
                    sprite.Control = -1;
                }

            }

            if (sprite.SoundCount == 0)
            { /* send report  */

                // Convert sprite coordinates to world coordinates.
                int x = (sprite.X + 48) / 16;
                int y = sprite.Y / 16;

                if (x >= 0 && x < Constants.WorldWidth && y >= 0 && y < Constants.WorldHeight)
                {

                    /* Don changed from 160 to 170 to shut the #$%#$% thing up! */

                    int chopperX = x + 1;
                    int chopperY = y + 1;
                    if (TrafficDensityMap.WorldGet(x, y) > 170 && (GetRandom16() & 7) == 0)
                    {
                        SendMessage(GeneralMessages.MESSAGE_HEAVY_TRAFFIC, (short)chopperX, (short)chopperY, true);
                        MakeSound("city", "HeavyTraffic", chopperX, chopperY); /* chopper */
                        sprite.SoundCount = 200;
                    }

                }

            }

            int z = sprite.Frame;

            if ((spriteCycle & 3) == 0)
            {
                short d = GetDir(sprite.X, sprite.Y, sprite.DestX, sprite.DestY);
                z = TurnTo(z, d);
                sprite.Frame = z;
            }

            sprite.X += CDx[z];
            sprite.Y += CDy[z];
        }

        public void DoAirplaneSprite(SimSprite sprite)
        {
            short[] CDx = { 0, 0, 6, 8, 6, 0, -6, -8, -6, 8, 8, 8 };
            short[] CDy = { 0, -8, -6, 0, 6, 8, 6, 0, -6, 0, 0, 0 };

            int z = sprite.Frame;

            if ((spriteCycle % 5) == 0)
            {

                if (z > 8)
                { /* TakeOff  */
                    z--;
                    if (z < 9)
                    {
                        z = 3;
                    }
                    sprite.Frame = z;
                }
                else
                { /* goto destination */
                    short d = GetDir(sprite.X, sprite.Y, sprite.DestX, sprite.DestY);
                    z = TurnTo(z, d);
                    sprite.Frame = z;
                }

            }

            if (absDist < 50)
            { /* at destination  */
                sprite.DestX = GetRandom((Constants.WorldWidth * 16) + 100) - 50;
                sprite.DestY = GetRandom((Constants.WorldHeight * 16) + 100) - 50;
            }

            /* deh added test for enableDisasters */
            if (EnableDisasters)
            {
                bool explode = false;

                /* Check whether another sprite is near enough to collide with */
                foreach(SimSprite s in SpriteList)
                {
                    if (s.Frame == 0 || s == sprite)
                    {
                        /* Non-active sprite, or self: skip */
                        continue;
                    }

                    if ((s.Type == SpriteType.Helicopter || s.Type == SpriteType.Airplane)
                                                  && CheckSpriteCollision(sprite, s))
                    {
                        ExplodeSprite(s);
                        explode = true;
                    }
                }

                if (explode)
                {
                    ExplodeSprite(sprite);
                }
            }

            sprite.X += CDx[z];
            sprite.Y += CDy[z];

            if (SpriteNotInBounds(sprite))
            {
                sprite.Frame = 0;
            }
        }

        public void DoShipSprite(SimSprite sprite)
        {
            short[] BDx = { 0, 0, 1, 1, 1, 0, -1, -1, -1 };
            short[] BDy = { 0, -1, -1, 0, 1, 1, 1, 0, -1 };
            short[] BPx = { 0, 0, 2, 2, 2, 0, -2, -2, -2 };
            short[] BPy = { 0, -2, -2, 0, 2, 2, 2, 0, -2 };
            short[] BtClrTab = { (short)MapTileCharacters.RIVER, (short)MapTileCharacters.CHANNEL, (short)MapTileCharacters.POWERBASE, (short)MapTileCharacters.POWERBASE + 1,
                                       (short)MapTileCharacters.RAILBASE, (short)MapTileCharacters.RAILBASE + 1, (short)MapTileCharacters.BRWH, (short)MapTileCharacters.BRWV };
            int x, y, z, t = (short)MapTileCharacters.RIVER;
            int tem, pem;

            if (sprite.SoundCount > 0)
            {
                sprite.SoundCount--;
            }

            if (!sprite.SoundCount.IsTrue())
            {

                if ((GetRandom16() & 3) == 1)
                {

                    // Convert sprite coordinates to tile coordinates.
                    int shipX = sprite.X >> 4;
                    int shipY = sprite.Y >> 4;

                    if (Scenario == Scenario.SanFrancisco && GetRandom(10) < 5)
                    {
                        MakeSound("city", "FogHornLow", shipX, shipY);
                    }
                    else
                    {
                        MakeSound("city", "HonkHonkLow", shipX, shipY);
                    }

                }

                sprite.SoundCount = 200;
            }

            if (sprite.Count > 0)
            {
                sprite.Count--;
            }

            if (sprite.Count == 0)
            {

                sprite.Count = 9;

                if (sprite.Frame != sprite.NewDir)
                {
                    sprite.Frame = TurnTo(sprite.Frame, sprite.NewDir);
                    return;
                }

                tem = GetRandom16() & 7;

                for (pem = tem; pem < (tem + 8); pem++)
                {

                    z = (pem & 7) + 1;

                    if (z == sprite.Dir)
                    {
                        continue;
                    }

                    x = ((sprite.X + (48 - 1)) >> 4) + BDx[z];
                    y = (sprite.Y >> 4) + BDy[z];

                    if (Position.TestBounds(x, y))
                    {

                        t = Map[x,y] & (ushort)MapTileBits.LowMask;

                        if (t == (short)MapTileCharacters.CHANNEL || t == (short)MapTileCharacters.BRWH 
                            || t == (short)MapTileCharacters.BRWV || TryOther(t, sprite.Dir, z))
                        {

                            sprite.NewDir = z;
                            sprite.Frame = TurnTo(sprite.Frame, sprite.NewDir);
                            sprite.Dir = z + 4;

                            if (sprite.Dir > 8)
                            {
                                sprite.Dir -= 8;
                            }

                            break;
                        }
                    }
                }

                if (pem == (tem + 8))
                {
                    sprite.Dir = 10;
                    sprite.NewDir = (GetRandom16() & 7) + 1;
                }

            }
            else
            {

                z = sprite.Frame;

                if (z == sprite.NewDir)
                {
                    sprite.X += BPx[z];
                    sprite.Y += BPy[z];
                }
            }

            if (SpriteNotInBounds(sprite))
            {
                sprite.Frame = 0;
                return;
            }

            for (z = 0; z < 8; z++)
            {

                if (t == BtClrTab[z])
                {
                    break;
                }

                if (z == 7)
                {
                    ExplodeSprite(sprite);
                    DestoryMapTile(sprite.X + 48, sprite.Y);
                }

            }
        }

        public void DoMonsterSprite(SimSprite sprite)
        {
            short[] Gx = { 2, 2, -2, -2, 0 };
            short[] Gy = { -2, 2, 2, -2, 0 };
            short[] ND1 = { 0, 1, 2, 3 };
            short[] ND2 = { 1, 2, 3, 0 };
            short[] nn1 = { 2, 5, 8, 11 };
            short[] nn2 = { 11, 2, 5, 8 };
            int d, z, c;

            if (sprite.SoundCount > 0)
            {
                sprite.SoundCount--;
            }

            if (sprite.Control < 0)
            {
                /* business as usual */

                if (sprite.Control == -2)
                {

                    d = (sprite.Frame - 1) / 3;
                    z = (sprite.Frame - 1) % 3;

                    if (z == 2)
                    {
                        sprite.Step = 0;
                    }

                    if (z == 0)
                    {
                        sprite.Step = 1;
                    }

                    if (sprite.Step.IsTrue())
                    {
                        z++;
                    }
                    else
                    {
                        z--;
                    }

                    c = GetDir(sprite.X, sprite.Y, sprite.DestX, sprite.DestY);

                    if (absDist < 18)
                    {

                        sprite.Control = -1;
                        sprite.Count = 1000;
                        sprite.Flag = 1;
                        sprite.DestX = sprite.OrigX;
                        sprite.DestY = sprite.OrigY;

                    }
                    else
                    {

                        c = (c - 1) / 2;

                        if ((c != d && GetRandom(5) == 0) || GetRandom(20) == 0)
                        {

                            int diff = (c - d) & 3;

                            if (diff == 1 || diff == 3)
                            {
                                d = c;
                            }
                            else
                            {

                                if ((GetRandom16() & 1).IsTrue())
                                {
                                    d++;
                                }
                                else
                                {
                                    d--;
                                }

                                d &= 3;
                            }
                        }
                        else
                        {

                            if (GetRandom(20) == 0)
                            {

                                if ((GetRandom16() & 1).IsTrue())
                                {
                                    d++;
                                }
                                else
                                {
                                    d--;
                                }

                                d &= 3;
                            }
                        }
                    }
                }
                else
                {

                    d = (sprite.Frame - 1) / 3;

                    if (d < 4)
                    { /* turn n s e w */

                        z = (sprite.Frame - 1) % 3;

                        if (z == 2)
                        {
                            sprite.Step = 0;
                        }

                        if (z == 0)
                        {
                            sprite.Step = 1;
                        }

                        if (sprite.Step.IsTrue())
                        {
                            z++;
                        }
                        else
                        {
                            z--;
                        }

                        GetDir(sprite.X, sprite.Y, sprite.DestX, sprite.DestY);

                        if (absDist < 60)
                        {

                            if (sprite.Flag == 0)
                            {

                                sprite.Flag = 1;
                                sprite.DestX = sprite.OrigX;
                                sprite.DestY = sprite.OrigY;

                            }
                            else
                            {

                                sprite.Frame = 0;
                                return;

                            }

                        }

                        c = GetDir(sprite.X, sprite.Y, sprite.DestX, sprite.DestY);
                        c = (c - 1) / 2;

                        if ((c != d) && (!GetRandom(10).IsTrue()))
                        {

                            if ((GetRandom16() & 1).IsTrue())
                            {
                                z = ND1[d];
                            }
                            else
                            {
                                z = ND2[d];
                            }

                            d = 4;

                            if (!sprite.SoundCount.IsTrue())
                            {
                                // Convert sprite coordinates to tile coordinates.
                                int monsterX = sprite.X >> 4;
                                int monsterY = sprite.Y >> 4;
                                MakeSound("city", "Monster", monsterX, monsterY); /* monster */
                                sprite.SoundCount = 50 + GetRandom(100);
                            }

                        }

                    }
                    else
                    {

                        d = 4;
                        c = sprite.Frame;
                        z = (c - 13) & 3;

                        if (!(GetRandom16() & 3).IsTrue())
                        {

                            if ((GetRandom16() & 1).IsTrue())
                            {
                                z = nn1[z];
                            }
                            else
                            {
                                z = nn2[z];
                            }

                            d = (z - 1) / 3;
                            z = (z - 1) % 3;

                        }

                    }

                }

            }
            else
            {

                /* somebody's taken control of the monster */

                d = sprite.Control;
                z = (sprite.Frame - 1) % 3;

                if (z == 2)
                {
                    sprite.Step = 0;
                }

                if (z == 0)
                {
                    sprite.Step = 1;
                }

                if (sprite.Step.IsTrue())
                {
                    z++;
                }
                else
                {
                    z--;
                }

            }

            z = d * 3 + z + 1;

            if (z > 16)
            {
                z = 16;
            }

            sprite.Frame = z;

            sprite.X += Gx[d];
            sprite.Y += Gy[d];

            if (sprite.Count > 0)
            {
                sprite.Count--;
            }

            c = GetChar(sprite.X + sprite.XHot, sprite.Y + sprite.YHot);

            if (c == -1
                  || (c == (ushort)MapTileCharacters.RIVER && sprite.Count != 0 && sprite.Control == -1))
            {
                sprite.Frame = 0; /* kill scary monster */
            }

            {
                foreach(SimSprite s in SpriteList)
                {
                    if (s.Frame != 0 &&
                            (s.Type == SpriteType.Airplane || s.Type == SpriteType.Helicopter
                              || s.Type == SpriteType.Ship || s.Type == SpriteType.Train) &&
                            CheckSpriteCollision(sprite, s))
                    {
                        ExplodeSprite(s);
                    }
                }
            }

            DestoryMapTile(sprite.X + 48, sprite.Y + 16);
        }

        public void DoTornadoSprite(SimSprite sprite) {
            short[] CDx = { 2, 3, 2, 0, -2, -3 };
            short[] CDy = { -2, 0, 2, 3, 2, 0 };
            int z;

            z = sprite.Frame;

            if (z == 2)
            {

                /* cycle animation... post Rel */

                if (sprite.Flag.IsTrue())
                {
                    z = 3;
                }
                else
                {
                    z = 1;
                }

            }
            else
            {

                if (z == 1)
                {
                    sprite.Flag = 1;
                }
                else
                {
                    sprite.Flag = 0;
                }

                z = 2;
            }

            if (sprite.Count > 0)
            {
                sprite.Count--;
            }

            sprite.Frame = z;

            {
                foreach(SimSprite s in SpriteList)
                {
                    if (s.Frame != 0 &&
                        (s.Type == SpriteType.Airplane || s.Type == SpriteType.Helicopter
                         || s.Type == SpriteType.Ship || s.Type == SpriteType.Train) &&
                        CheckSpriteCollision(sprite, s))
                    {
                        ExplodeSprite(s);
                    }
                }
            }

            z = GetRandom(5);
            sprite.X += CDx[z];
            sprite.Y += CDy[z];

            if (SpriteNotInBounds(sprite))
            {
                sprite.Frame = 0;
            }

            if (sprite.Count != 0 && GetRandom(500) == 0)
            {
                sprite.Frame = 0;
            }

            DestoryMapTile(sprite.X + 48, sprite.Y + 40);
        }

        public void DoExplosionSprite(SimSprite sprite)
        {
            int x, y;

            if ((spriteCycle & 1) == 0)
            {

                if (sprite.Frame == 1)
                {
                    // Convert sprite coordinates to tile coordinates.
                    int explosionX = sprite.X >> 4;
                    int explosionY = sprite.Y >> 4;
                    MakeSound("city", "ExplosionHigh", explosionX, explosionY); /* explosion */
                    x = (sprite.X >> 4) + 3;
                    y = (sprite.Y >> 4);
                    SendMessage(GeneralMessages.MESSAGE_EXPLOSION_REPORTED, (short)x, (short)y);
                }

                sprite.Frame++;
            }

            if (sprite.Frame > 6)
            {
                sprite.Frame = 0;

                StartFire(sprite.X + 48 - 8,  sprite.Y + 16);
                StartFire(sprite.X + 48 - 24, sprite.Y);
                StartFire(sprite.X + 48 + 8,  sprite.Y);
                StartFire(sprite.X + 48 - 24, sprite.Y + 32);
                StartFire(sprite.X + 48 + 8,  sprite.Y + 32);
            }
        }

        public void DoBusSprite(SimSprite sprite) {
            short[] Dx = { 0, 1, 0, -1, 0 };
            short[] Dy = { -1, 0, 1, 0, 0 };
            short[] Dir2Frame = { 1, 2, 1, 2 };

            int dx, dy, tx, ty, otx, oty;
            int turned = 0;
            int speed = 0;
            int z;

            if (sprite.Turn.IsTrue())
            {

                if (sprite.Turn < 0)
                { /* ccw */

                    if ((sprite.Dir & 1).IsTrue())
                    { /* up or down */
                        sprite.Frame = 4;
                    }
                    else
                    { /* left or right */
                        sprite.Frame = 3;
                    }

                    sprite.Turn++;
                    sprite.Dir = (sprite.Dir - 1) & 3;

                }
                else
                { /* cw */

                    if ((sprite.Dir & 1).IsTrue())
                    { /* up or down */
                        sprite.Frame = 3;
                    }
                    else
                    { /* left or right */
                        sprite.Frame = 4;
                    }

                    sprite.Turn--;
                    sprite.Dir = (sprite.Dir + 1) & 3;

                }

                turned = 1;

            }
            else
            {

                /* finish turn */
                if ((sprite.Frame == 3) ||
                    (sprite.Frame == 4))
                {
                    turned = 1;
                    sprite.Frame = Dir2Frame[sprite.Dir];
                }
            }

            if (sprite.Speed == 0)
            {

                /* brake */
                dx = 0; dy = 0;

            }
            else
            { /* cruise at traffic speed */

                tx = (sprite.X + sprite.XHot) >> 5;
                ty = (sprite.Y + sprite.YHot) >> 5;

                if (tx >= 0 && tx < Constants.WorldWidth2 && ty >= 0 && ty < Constants.WorldHeight2)
                {

                    z = TrafficDensityMap.WorldGet(tx << 1, ty << 1) >> 6;

                    if (z > 1)
                    {
                        z--;
                    }

                }
                else
                {

                    z = 0;

                }

                switch (z)
                {

                    case 0:
                        speed = 8;
                        break;

                    case 1:
                        speed = 4;
                        break;

                    case 2:
                        speed = 1;
                        break;

                }

                /* govern speed */
                if (speed > sprite.Speed)
                {
                    speed = sprite.Speed;
                }

                if (turned.IsTrue())
                {

                    if (speed > 1)
                    {
                        speed = 1;
                    }

                    dx = Dx[sprite.Dir] * speed;
                    dy = Dy[sprite.Dir] * speed;

                }
                else
                {

                    dx = Dx[sprite.Dir] * speed;
                    dy = Dy[sprite.Dir] * speed;

                    tx = (sprite.X + sprite.XHot) >> 4;
                    ty = (sprite.Y + sprite.YHot) >> 4;

                    /* drift into the right lane */
                    switch (sprite.Dir)
                    {

                        case 0: /* up */

                            z = ((tx << 4) + 4) - (sprite.X + sprite.XHot);

                            if (z < 0)
                            {
                                dx = -1;
                            }
                            else if (z > 0)
                            {
                                dx = 1;
                            }
                            break;

                        case 1: /* right */

                            z = ((ty << 4) + 4) - (sprite.Y + sprite.YHot);

                            if (z < 0)
                            {
                                dy = -1;
                            }
                            else if (z > 0)
                            {
                                dy = 1;
                            }
                            break;

                        case 2: /* down */

                            z = (tx << 4) - (sprite.X + sprite.XHot);

                            if (z < 0)
                            {
                                dx = -1;
                            }
                            else if (z > 0)
                            {
                                dx = 1;
                            }
                            break;

                        case 3: /* left */

                            z = (ty << 4) - (sprite.Y + sprite.YHot);

                            if (z < 0)
                            {
                                dy = -1;
                            }
                            else if (z > 0)
                            {
                                dy = 1;
                            }
                            break;
                    }
                }
            }

            const int AHEAD = 8;

            otx = (sprite.X + sprite.XHot + (Dx[sprite.Dir] * AHEAD)) >> 4;
            oty = (sprite.Y + sprite.YHot + (Dy[sprite.Dir] * AHEAD)) >> 4;

            otx = Utilities.Restrict(otx, 0, Constants.WorldWidth - 1);
            oty = Utilities.Restrict(oty, 0, Constants.WorldHeight - 1);

            tx = (sprite.X + sprite.XHot + dx + (Dx[sprite.Dir] * AHEAD)) >> 4;
            ty = (sprite.Y + sprite.YHot + dy + (Dy[sprite.Dir] * AHEAD)) >> 4;

            tx = Utilities.Restrict(tx, 0, Constants.WorldWidth - 1);
            ty = Utilities.Restrict(ty, 0, Constants.WorldHeight - 1);

            if (tx != otx || ty != oty)
            {
                z = CanDriveOn(tx, ty);
                if (z == 0)
                {

                    /* can't drive forward into a new tile */
                    if (speed == 8)
                    {
                        BulldozerTool((short)tx, (short)ty);
                    }
                    else
                    {
                    }

                }
                else
                {

                    /* drive forward into a new tile */
                    if (z > 0)
                    {
                        /* smooth */
                    }
                    else
                    {
                        /* bumpy */
                        dx /= 2;
                        dy /= 2;
                    }

                }
            }

            tx = (sprite.X + sprite.XHot + dx) >> 4;
            ty = (sprite.Y + sprite.YHot + dy) >> 4;

            z = CanDriveOn(tx, ty);

            if (z > 0)
            {
                /* cool, cruise along */
            }
            else
            {
                if (z < 0)
                {
                    /* bumpy */
                }
                else
                {
                    /* something in the way */
                }
            }

            sprite.X += dx;
            sprite.Y += dy;

            if (EnableDisasters)
            {
                int explode = 0;

                foreach(SimSprite s in SpriteList)
                {
                    if (sprite != s && s.Frame != 0
                            && (s.Type == SpriteType.Bus
                                || (s.Type == SpriteType.Train && s.Frame != 5))
                            && CheckSpriteCollision(sprite, s))
                    {
                        ExplodeSprite(s);
                        explode = 1;
                    }
                }

                if (explode.IsTrue())
                {
                    ExplodeSprite(sprite);
                }

            }
        }

        public int CanDriveOn(int x, int y)
        {
            ushort tile;

            if (!Position.TestBounds(x, y))
            {
                return 0;
            }

            tile = (ushort)(Map[x,y] & (ushort)MapTileBits.LowMask);

            if ((tile >= (ushort)MapTileCharacters.ROADBASE && tile <= (ushort)MapTileCharacters.LASTROAD 
                && tile != (ushort)MapTileCharacters.BRWH && tile != (ushort)MapTileCharacters.BRWV)
                      || tile == (ushort)MapTileCharacters.HRAILROAD || tile == (ushort)MapTileCharacters.VRAILROAD)
            {
                return 1;
            }

            if (tile == (ushort)MapTileCharacters.DIRT || Tally(tile))
            {
                return -1;
            }

            return 0;
        }

        public void ExplodeSprite(SimSprite sprite)
        {
            int x, y;

            sprite.Frame = 0;

            x = sprite.X + sprite.XHot;
            y = sprite.Y + sprite.YHot;
            MakeExplosionAt(x, y);

            x = (x >> 4);
            y = (y >> 4);

            switch (sprite.Type)
            {

                case SpriteType.Airplane:
                    SendMessage(GeneralMessages.MESSAGE_PLANE_CRASHED, (short)x, (short)y, true);
                    break;

                case SpriteType.Ship:
                    SendMessage(GeneralMessages.MESSAGE_SHIP_CRASHED, (short)x, (short)y, true);
                    break;

                case SpriteType.Train:
                    SendMessage(GeneralMessages.MESSAGE_TRAIN_CRASHED, (short)x, (short)y, true);
                    break;

                case SpriteType.Helicopter:
                    SendMessage(GeneralMessages.MESSAGE_HELICOPTER_CRASHED, (short)x, (short)y, true);
                    break;

                case SpriteType.Bus:
                    SendMessage(GeneralMessages.MESSAGE_TRAIN_CRASHED, (short)x, (short)y, true); /* XXX for now */
                    break;

            }

            // Convert sprite coordinates to tile coordinates.
            MakeSound("city", "ExplosionHigh", x, y); /* explosion */

            return;
        }

        public bool CheckWet(int x)
        {
            if (x == (ushort)MapTileCharacters.HPOWER || x == (ushort)MapTileCharacters.VPOWER 
                || x == (ushort)MapTileCharacters.HRAIL || x == (ushort)MapTileCharacters.VRAIL
                || x == (ushort)MapTileCharacters.BRWH || x == (ushort)MapTileCharacters.BRWV)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void DestoryMapTile(int ox, int oy) {
            int t, z, x, y;

            x = ox >> 4;
            y = oy >> 4;

            if (!Position.TestBounds(x, y))
            {
                return;
            }

            z = Map[x,y];
            t = z & (ushort)MapTileBits.LowMask;

            if (t >= (ushort)MapTileCharacters.TREEBASE)
            {
                if (!(z & (ushort)MapTileBits.Burnable).IsTrue())
                {

                    if (t >= (ushort)MapTileCharacters.ROADBASE && t <= (ushort)MapTileCharacters.LASTROAD)
                    {
                        Map[x,y] = (ushort)MapTileCharacters.RIVER;
                    }

                    return;
                }

                if ((z & (ushort)MapTileBits.CenterOfZone).IsTrue())
                {

                    StartFireInZone(x, y, z);

                    if (t > (ushort)MapTileCharacters.RZB)
                    {
                        MakeExplosionAt(ox, oy);
                    }

                }

                if (CheckWet(t))
                {
                    Map[x,y] = (ushort)MapTileCharacters.RIVER;
                }
                else
                {
                    Map[x,y] = (ushort)((DoAnimation ? (ushort)MapTileCharacters.TINYEXP : ((ushort)MapTileCharacters.LASTTINYEXP - 3))
                              | (ushort)MapTileBits.Bulldozable | (ushort)MapTileBits.Animated);
                }
            }
        }

        public void StartFireInZone(int Xloc, int Yloc, int ch) {
            int Xtem, Ytem;
            short x, y, XYmax;

            int value = RateOfGrowthMap.WorldGet(Xloc, Yloc);
            value = Utilities.Restrict(value - 20, -200, 200);
            RateOfGrowthMap.WorldSet(Xloc, Yloc, (short)value);

            ch &= (short)MapTileBits.LowMask;

            if (ch < (short)MapTileCharacters.PORTBASE)
            {
                XYmax = 2;
            }
            else
            {
                if (ch == (short)MapTileCharacters.AIRPORT)
                {
                    XYmax = 5;
                }
                else
                {
                    XYmax = 4;
                }
            }

            for (x = -1; x < XYmax; x++)
            {
                for (y = -1; y < XYmax; y++)
                {

                    Xtem = Xloc + x;
                    Ytem = Yloc + y;

                    if (Position.TestBounds(Xtem, Ytem) && (Map[Xtem,Ytem] & (ushort)MapTileBits.LowMask) >= (short)MapTileCharacters.ROADBASE)
                    {
                        Map[Xtem,Ytem] |= (ushort)MapTileBits.Bulldozable;
                    }

                }
            }
        }

        public void StartFire(int x, int y)
        {
            int t, z;

            x >>= 4;
            y >>= 4;

            if (!Position.TestBounds(x, y))
            {
                return;
            }

            z = Map[x,y];
            t = z & (ushort)MapTileBits.LowMask;

            if (!(z & (ushort)MapTileBits.Burnable).IsTrue() && t != (ushort)MapTileCharacters.DIRT)
            {
                return;
            }

            if ((z & (ushort)MapTileBits.CenterOfZone).IsTrue()) ;
            {
                return;
            }

            Map[x,y] = RandomFire();
        }

        public void GenerateTrain(int x, int y)
        {
            if (TotalPop > 20 && GetSprite(SpriteType.Train) == null && GetRandom(25) == 0)
            {
                MakeSprite(SpriteType.Train, (x << 4) + Constants.TraGrooveX, (y << 4) + Constants.TraGrooveY);
            }
        }

        public void GenerateBus(int x, int y)
        {
            if (GetSprite(SpriteType.Bus) == null && GetRandom(25) == 0)
            {
                MakeSprite(SpriteType.Bus, (x << 4) + Constants.BusGrooveX, (y << 4) + Constants.BusGrooveY);
            }
        }

        public void GenerateShip()
        {
            int x, y;

            if (!(GetRandom16() & 3).IsTrue())
            {
                for (x = 4; x < Constants.WorldWidth - 2; x++)
                {
                    if (Map[x,0] == (ushort)MapTileCharacters.CHANNEL)
                    {
                        MakeShipHere(x, 0);
                        return;
                    }
                }
            }

            if (!(GetRandom16() & 3).IsTrue())
            {
                for (y = 1; y < Constants.WorldHeight - 2; y++)
                {
                    if (Map[0,y] == (ushort)MapTileCharacters.CHANNEL)
                    {
                        MakeShipHere(0, y);
                        return;
                    }
                }
            }

            if (!(GetRandom16() & 3).IsTrue())
            {
                for (x = 4; x < Constants.WorldWidth - 2; x++)
                {
                    if (Map[x, Constants.WorldHeight - 1] == (ushort)MapTileCharacters.CHANNEL)
                    {
                        MakeShipHere(x, Constants.WorldHeight - 1);
                        return;
                    }
                }
            }

            if (!(GetRandom16() & 3).IsTrue())
            {
                for (y = 1; y < Constants.WorldHeight - 2; y++)
                {
                    if (Map[Constants.WorldWidth - 1,y] == (ushort)MapTileCharacters.CHANNEL)
                    {
                        MakeShipHere(Constants.WorldWidth - 1, y);
                        return;
                    }
                }
            }
        }

        public void MakeShipHere(int x, int y) { MakeSprite(SpriteType.Ship, (x << 4) - (48 - 1), (y << 4)); }

        public void MakeMonster()
        {
            int x, y, z, done = 0;
            SimSprite sprite;

            sprite = GetSprite(SpriteType.Monster);
            if (sprite != null)
            {
                sprite.SoundCount = 1;
                sprite.Count = 1000;
                sprite.DestX = PollutionMaxX << 4;
                sprite.DestY = PollutionMaxY << 4;
                return;
            }

            for (z = 0; z < 300; z++)
            {

                x = GetRandom(Constants.WorldWidth - 20) + 10;
                y = GetRandom(Constants.WorldHeight - 10) + 5;

                if (Map[x,y] == (ushort)MapTileCharacters.RIVER || Map[x,y] == (ushort)MapTileCharacters.RIVER + (ushort)MapTileBits.Bulldozable)
                {
                    MakeMonsterAt(x, y);
                    done = 1;
                    break;
                }

            }

            if (done.IsFalse())
            {
                MakeMonsterAt(60, 50);
            }
        }

        public void MakeMonsterAt(int x, int y)
        {
            MakeSprite(SpriteType.Monster, (x << 4) + 48, (y << 4));
            SendMessage(GeneralMessages.MESSAGE_MONSTER_SIGHTED, (short)(x + 5), (short)y, true, true);
        }

        public void GenerateCopter(Position pos)
        {
            if (GetSprite(SpriteType.Helicopter) != null)
            {
                return;
            }

            MakeSprite(SpriteType.Helicopter, (pos.X << 4), (pos.Y << 4) + 30);
        }

        public void GeneratePlane(Position pos)
        {
            if (GetSprite(SpriteType.Airplane) != null)
            {
                return;
            }

            MakeSprite(SpriteType.Airplane, (short)((pos.X << 4) + 48), (short)((pos.Y << 4) + 12));
        }

        public void MakeTornado()
        {
            int x, y;
            SimSprite sprite;

            sprite = GetSprite(SpriteType.Tornado);
            if (sprite != null)
            {
                sprite.Count = 200;
                return;
            }

            x = GetRandom((Constants.WorldWidth << 4) - 800) + 400;
            y = GetRandom((Constants.WorldHeight << 4) - 200) + 100;

            MakeSprite(SpriteType.Tornado, x, y);
            SendMessage(GeneralMessages.MESSAGE_TORNADO_SIGHTED, (short)((x >> 4) + 3), (short)((y >> 4) + 2), true, true);
        }

        public void MakeExplosion(int x, int y)
        {
            if (Position.TestBounds(x, y))
            {
                MakeExplosionAt((x << 4) + 8, (y << 4) + 8);
            }
        }

        public void MakeExplosionAt(int x, int y) { NewSprite("", SpriteType.Explosion, x - 40, y - 16); }
    }
}