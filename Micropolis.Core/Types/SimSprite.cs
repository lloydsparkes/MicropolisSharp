/* SimSprite.cs
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

using System;

namespace MicropolisSharp.Types;

/// <summary>
///     Sprite in the simulator - From micropolis.h
///     TODO: is never set to anything else than \c "", and only used to detect a non-removed non-active sprite(in a
///     non-robust way).
///     TODO: Remove next
///     TODO: Remove Linked List
///     TODO: Remove Completely as this isnt a game engine thing
/// </summary>
public class SimSprite
{
    public SimSprite()
    {
        Id = Guid.NewGuid().ToString().Substring(0, 8);
    }

    public string Id { get; }

    [Obsolete("Removed - No longer access as a linked list, use array accessor")]
    public SimSprite Next { get; set; }

    /// < Pointer to next # SimSprite object in the list.
    public string Name { get; set; }

    ///< Name of the sprite.
    public SpriteType Type { get; set; }

    /// < Type of the sprite ( TRA -- BUS
    /// )
    /// .
    public int Frame { get; set; }

    /// < Frame (\ c 0 means non-active sprite
    /// )
    public int X { get; set; }

    /// < X coordinate of the sprite in pixels
    /// ?
    public int Y { get; set; }

    /// < Y coordinate of the sprite in pixels
    /// ?
    public int Width { get; set; }

    public int Height { get; set; }
    public int XOffset { get; set; }
    public int YOffset { get; set; }
    public int XHot { get; set; }

    /// < Offset of the hot-spot relative to SimSprite::x
    /// ?
    public int YHot { get; set; }

    /// < Offset of the hot-spot relative to SimSprite::y
    /// ?
    public int OrigX { get; set; }

    public int OrigY { get; set; }
    public int DestX { get; set; }

    ///< Destination X coordinate of the sprite.
    public int DestY { get; set; }

    ///< Destination Y coordinate of the sprite.
    public int Count { get; set; }

    public int SoundCount { get; set; }
    public int Dir { get; set; }
    public int NewDir { get; set; }
    public int Step { get; set; }
    public int Flag { get; set; }
    public int Control { get; set; }
    public int Turn { get; set; }
    public int Accel { get; set; }
    public int Speed { get; set; }

    public override string ToString()
    {
        return
            $"Id: {Id}, Name: {Name}, Type: {Type}, Frame: {Frame}, X: {X}, Y: {Y}, Width: {Width}, Height: {Height}, XOffset: {XOffset}, YOffset: {YOffset}, XHot: {XHot}, YHot: {YHot}, OrigX: {OrigX}, OrigY: {OrigY}, DestX: {DestX}, DestY: {DestY}, Count: {Count}, SoundCount: {SoundCount}, Dir: {Dir}, NewDir: {NewDir}, Step: {Step}, Flag: {Flag}, Control: {Control}, Turn: {Turn}, Accel: {Accel}, Speed: {Speed}";
    }
}