/* Map.cs
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
///     Generic Class for Maps (weridly not used by the main map)
///     Taken from map_type.h
///     TODO: Refactor
///     TODO: Tests
/// </summary>
/// <typeparam name="DataType">The data type for each cell of the map</typeparam>
public class Map<DataType>
    where DataType : struct
{
    public readonly int height;

    public readonly int width;

    /// <summary>
    ///     TODO: Make this a two dimensional map? Maybe this is better given the logic in main.cpp (heatflow())
    /// </summary>
    private readonly DataType[] data;

    private readonly DataType defaultValue;

    /// <summary>
    ///     Simple Initialiser - set defaults
    /// </summary>
    /// <param name="blockSize">The size of each block. I.e if 2 then each cell in this map represents 4 in the real map</param>
    public Map(int blockSize)
    {
        BlockSize = blockSize;

        width = (Constants.WorldWidth + BlockSize - 1) / BlockSize;
        height = (Constants.WorldHeight + BlockSize - 1) / BlockSize;
        defaultValue = default;
        data = new DataType[width * height];
        Fill(defaultValue);
    }

    /// <summary>
    ///     Simple Initialiser - plus a specific default value for the map
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <param name="blockSize"></param>
    public Map(DataType defaultValue, int blockSize) : this(blockSize)
    {
        this.defaultValue = defaultValue;
        Fill(defaultValue);
    }

    /// <summary>
    ///     Copy Constructor - copy from another map
    /// </summary>
    /// <param name="map"></param>
    public Map(Map<DataType> map) : this(map.BlockSize)
    {
        for (var i = 0; i < width * height; ++i) data[i] = map.data[i];
    }

    public int BlockSize { get; }

    /// <summary>
    ///     Compare two maps - checks every cell
    /// </summary>
    /// <param name="other">The map to compare to</param>
    /// <returns>True if every cell is the same</returns>
    public bool Compare(Map<DataType> other)
    {
        if (other == null) return false;

        if (other == this) return true;

        if (other.BlockSize != BlockSize) return false;

        for (var i = 0; i < width * height; ++i)
            if (!data[i].Equals(other.data[i]))
                return false;
        return true;
    }

    /// <summary>
    ///     Fill the map with a particular value
    /// </summary>
    /// <param name="value">The value to set every cell to</param>
    public void Fill(DataType value)
    {
        for (var i = 0; i < width * height; ++i) data[i] = value;
    }

    /// <summary>
    ///     Clear the map - set every cell to the default value
    /// </summary>
    public void Clear()
    {
        Fill(defaultValue);
    }

    /// <summary>
    ///     Set a value on the map - with a local position
    ///     TODO: Remove this
    /// </summary>
    /// <param name="x">Position to set in the X axis</param>
    /// <param name="y">Position to set in the Y axis</param>
    /// <param name="value">The value to set</param>
    [Obsolete("Use WorldSet - and ensure you change X/Y as required")]
    public void Set(int x, int y, DataType value)
    {
        if (OnMap(x, y))
            data[x * height + y] = value;
    }

    /// <summary>
    ///     Gets a value from the map - using a local position
    ///     TODO: Remove this
    /// </summary>
    /// <param name="x">Position to get in the X axis</param>
    /// <param name="y">Position to get in the Y axis</param>
    /// <returns></returns>
    [Obsolete("Use WorldGet - and ensure you change X/Y as required")]
    public DataType Get(int x, int y)
    {
        if (OnMap(x, y))
            return data[x * height + y];

        return defaultValue;
    }

    /// <summary>
    ///     Is the local position offered within the bounds of the map?
    /// </summary>
    /// <param name="x">Position X wanted</param>
    /// <param name="y">Position Y wanted</param>
    /// <returns>Returns true, if X,Y is on the map (local bounds)</returns>
    public bool OnMap(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    /// <summary>
    ///     Sets the value at the World-Relative position in this map, to the given value
    /// </summary>
    /// <param name="x">Relative Position to set in the X axis</param>
    /// <param name="y">Relative Position to set in the Y axis</param>
    /// <param name="value">The value to set on the map</param>
    public void WorldSet(int x, int y, DataType value)
    {
        if (WorldOnMap(x, y))
        {
            x /= BlockSize;
            y /= BlockSize;
            data[x * height + y] = value;
        }
    }

    /// <summary>
    ///     Gets the value at the World-Relative position in this map
    /// </summary>
    /// <param name="x">Relative Position to get in the X axis</param>
    /// <param name="y">Relative Position to get in the Y axis</param>
    /// <returns>The value at the relative position</returns>
    public DataType WorldGet(int x, int y)
    {
        if (!WorldOnMap(x, y))
            return defaultValue;

        x /= BlockSize;
        y /= BlockSize;
        return data[x * height + y];
    }

    /// <summary>
    ///     Is the relative position offered within the bounds of the map?
    /// </summary>
    /// <param name="x">Position X wanted</param>
    /// <param name="y">Position Y wanted</param>
    /// <returns>Returns true, if X,Y is on the map (relative world bounds)</returns>
    public bool WorldOnMap(int x, int y)
    {
        return x >= 0 && x < Constants.WorldWidth && y >= 0 && y < Constants.WorldHeight;
    }

    /// <summary>
    ///     Get the underlying data to manipulate directly
    ///     TODO: Remove / Hide this method - it was only used within memory / pointer based direct accesses.
    /// </summary>
    /// <returns>The underlying data array</returns>
    [Obsolete]
    public DataType[] getBase()
    {
        return data;
    }
}

/// <summary>
///     A map of DataType = Byte - BlockSize = 1
/// </summary>
public class ByteMap1 : Map<byte>
{
    public ByteMap1() : base(1)
    {
    }

    public ByteMap1(byte defaultValue) : base(defaultValue, 1)
    {
    }

    public ByteMap1(ByteMap1 map) : base(map)
    {
    }
}

/// <summary>
///     A map of DataType = Byte - BlockSize = 2
/// </summary>
public class ByteMap2 : Map<byte>
{
    public ByteMap2() : base(2)
    {
    }

    public ByteMap2(byte defaultValue) : base(defaultValue, 2)
    {
    }

    public ByteMap2(ByteMap2 map) : base(map)
    {
    }
}

/// <summary>
///     A map of DataType = Byte - BlockSize = 4
/// </summary>
public class ByteMap4 : Map<byte>
{
    public ByteMap4() : base(4)
    {
    }

    public ByteMap4(byte defaultValue) : base(defaultValue, 4)
    {
    }

    public ByteMap4(ByteMap4 map) : base(map)
    {
    }
}

/// <summary>
///     A map of DataType = Short - BlockSize = 8
/// </summary>
public class ShortMap8 : Map<short>
{
    public ShortMap8() : base(8)
    {
    }

    public ShortMap8(short defaultValue) : base(defaultValue, 8)
    {
    }

    public ShortMap8(ShortMap8 map) : base(map)
    {
    }
}