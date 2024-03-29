﻿/* Micropolis.FileIo.cs
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
using System.IO;
using MicropolisSharp.Types;

namespace MicropolisSharp;

/// <summary>
///     Partial Class Containing the content of fileio.cpp
/// </summary>
public partial class Micropolis
{
    /// <summary>
    ///     Swap upper and lower byte of all shorts in the array.
    /// </summary>
    /// <param name="bytes">Array with shorts.</param>
    /// <returns></returns>
    private short SwapShort(byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }

        return BitConverter.ToInt16(bytes, 0);
        ;
    }

    /// <summary>
    ///     Load an array of short values from file to memory.
    ///     Convert to the correct processor architecture, if necessary.
    /// </summary>
    /// <param name="buf">Buffer to put the loaded short values in.</param>
    /// <param name="stream">The stream to read from</param>
    /// <returns>Load was succesful</returns>
    private bool LoadShort(ref short buf, BinaryReader stream)
    {
        var bytes = stream.ReadBytes(sizeof(short));
        if (bytes.Length != sizeof(short)) return false;
        buf = SwapShort(bytes);
        return true;
    }

    private bool LoadShorts(ref short[] output, int len, BinaryReader stream)
    {
        var result = true;
        for (var i = 0; i < len; ++i) result = result && LoadShort(ref output[i], stream);
        return result;
    }

    /// <summary>
    ///     Save an array of short values from memory to file.
    ///     Convert to the correct endianness first, if necessary.
    /// </summary>
    /// <param name="buf">containing the short values to save.</param>
    /// <param name="stream">The stream to save to</param>
    /// <returns>Save was succesful</returns>
    private bool SaveShort(short buf, BinaryWriter stream)
    {
        var toWrite = SwapShort(BitConverter.GetBytes(buf));
        stream.Write(buf);
        return true;
    }

    private bool SaveShorts(short[] buf, BinaryWriter stream)
    {
        foreach (var s in buf) SaveShort(s, stream);
        return true;
    }

    /// <summary>
    ///     Swap upper and lower words of all longs in the array.
    /// </summary>
    /// <param name="shorts">Array with longs.</param>
    /// <param name="indexOfFirst"></param>
    /// <returns></returns>
    private int HalfSwapLong(short[] shorts, int indexOfFirst)
    {
        var bytes = new byte[4];
        bytes[0] = BitConverter.GetBytes(shorts[indexOfFirst])[0];
        bytes[1] = BitConverter.GetBytes(shorts[indexOfFirst])[1];
        indexOfFirst++;
        bytes[2] = BitConverter.GetBytes(shorts[indexOfFirst])[0];
        bytes[3] = BitConverter.GetBytes(shorts[indexOfFirst])[1];

        if (BitConverter.IsLittleEndian)
        {
            bytes[2] = BitConverter.GetBytes(shorts[indexOfFirst])[0];
            bytes[3] = BitConverter.GetBytes(shorts[indexOfFirst])[1];
            indexOfFirst++;
            bytes[0] = BitConverter.GetBytes(shorts[indexOfFirst])[0];
            bytes[1] = BitConverter.GetBytes(shorts[indexOfFirst])[1];
        }

        return BitConverter.ToInt32(bytes, 0);
    }

    private void SaveIntToShort(short[] output, int outputIndex, int valueToSave)
    {
        var data = BitConverter.GetBytes(valueToSave);

        byte[] short1 = { data[0], data[1] };
        byte[] short2 = { data[2], data[3] };

        output[outputIndex] = BitConverter.ToInt16(short1, 0);
        output[outputIndex + 1] = BitConverter.ToInt16(short2, 0);
    }

    /// <summary>
    ///     Load a city file from a given filename and (optionally) directory.
    /// </summary>
    /// <param name="filename">Name of the file to load.</param>
    /// <param name="dir">If not \c NULL, name of the directory containing the file.</param>
    /// <returns>Load was succesful</returns>
    public bool LoadFileDir(string filename, string dir)
    {
        BinaryReader f;
        long size;

        // If needed, construct a path to the file.
        if (dir != null) filename = dir + Path.DirectorySeparatorChar + filename;

        if (!File.Exists(filename)) return false;

        var fileInfo = new FileInfo(filename);

        const int expectedFileSize = 27120;

        var result = fileInfo.Length == expectedFileSize;

        if (!result) return false;

        using (var reader = new BinaryReader(File.OpenRead(filename)))
        {
            result = result &&
                     LoadShorts(ref ResHist, Constants.HistoryLength, reader) &&
                     LoadShorts(ref ComHist, Constants.HistoryLength, reader) &&
                     LoadShorts(ref IndHist, Constants.HistoryLength, reader) &&
                     LoadShorts(ref CrimeHist, Constants.HistoryLength, reader) &&
                     LoadShorts(ref PollutionHist, Constants.HistoryLength, reader) &&
                     LoadShorts(ref MoneyHist, Constants.HistoryLength, reader) &&
                     LoadShorts(ref MiscHist, Constants.MiscHistoryLength, reader);

            for (var x = 0; x < Map.GetLength(0); x++)
            for (var y = 0; y < Map.GetLength(1); y++)
            {
                var temp = (short)Map[x, y];
                result = result && LoadShort(ref temp, reader);
                Map[x, y] = (ushort)temp;
            }
        }

        return result;
    }

    /// <summary>
    ///     Load a file, and initialize the game variables.
    /// </summary>
    /// <param name="filename">Name of the file to load.</param>
    /// <returns>Load was succesfull.</returns>
    public bool LoadFile(string filename)
    {
        int n;

        if (!LoadFileDir(filename, null)) return false;

        /* total funds is a long.....    miscHist is array of shorts */
        /* total funds is being put in the 50th & 51th word of miscHist */
        /* find the address, cast the ptr to a longPtr, take contents */

        n = HalfSwapLong(MiscHist, 50);
        SetFunds(n);

        n = HalfSwapLong(MiscHist, 8);
        CityTime = n;

        SetAutoBulldoze(MiscHist[52] != 0); // flag for autoBulldoze
        SetAutoBudget(MiscHist[53] != 0); // flag for autoBudget
        SetAutoGoTo(MiscHist[54] != 0); // flag for auto-goto
        SetEnableSound(MiscHist[55] != 0); // flag for the sound on/off
        SetCityTax(MiscHist[56]);
        SetSpeed(MiscHist[57]);
        ChangeCensus();
        MustUpdateOptions = true;

        /* yayaya */
        PolicePercentage = HalfSwapLong(MiscHist, 58) / (float)65536;
        FirePercentage = HalfSwapLong(MiscHist, 60) / (float)65536.0;
        RoadPercentage = HalfSwapLong(MiscHist, 62) / (float)65536.0;

        CityTime = Math.Max(0, CityTime);

        // If the tax is nonsensical, set it to a reasonable value.
        if (CityTax > 20 || CityTax < 0) SetCityTax(7);

        // If the speed is nonsensical, set it to a reasonable value.
        if (SimSpeed < 0 || SimSpeed > 3) SetSpeed(3);

        SetSpeed(SimSpeed);
        SetPasses(1);
        InitFundingLevel();

        // Set the scenario id to 0.
        InitWillStuff();
        Scenario = Scenario.None;
        InitSimLoad = 1;
        DoInitialEval = false;
        DoSimInit();
        InvalidateMaps();

        return true;
    }

    /// <summary>
    ///     Save a game to disk.
    /// </summary>
    /// <param name="filename">Name of the file to use for storing the game.</param>
    /// <returns>The game was saved successfully.</returns>
    public bool SaveFile(string filename)
    {
        long n;

        if (!File.Exists(filename)) return false;

        /* total funds is a long.....    miscHist is array of ints */
        /* total funds is bien put in the 50th & 51th word of miscHist */
        /* find the address, cast the ptr to a longPtr, take contents */
        SaveIntToShort(MiscHist, 50, (int)TotalFunds);
        SaveIntToShort(MiscHist, 50, (int)CityTime);

        MiscHist[52] = AutoBulldoze.ToShort(); // flag for autoBulldoze
        MiscHist[53] = AutoBudget.ToShort(); // flag for autoBudget
        MiscHist[54] = AutoGoTo.ToShort(); // flag for auto-goto
        MiscHist[55] = EnableSound.ToShort(); // flag for the sound on/off
        MiscHist[57] = SimSpeed;
        MiscHist[56] = (short)CityTax; /* post release */

        /* yayaya */
        SaveIntToShort(MiscHist, 58, (int)PolicePercentage);
        SaveIntToShort(MiscHist, 60, (int)FirePercentage);
        SaveIntToShort(MiscHist, 62, (int)RoadPercentage);

        var result = true;
        using (var f = new BinaryWriter(File.OpenWrite(filename)))
        {
            result = result &&
                     SaveShorts(ResHist, f) &&
                     SaveShorts(ComHist, f) &&
                     SaveShorts(IndHist, f) &&
                     SaveShorts(CrimeHist, f) &&
                     SaveShorts(PollutionHist, f) &&
                     SaveShorts(MoneyHist, f) &&
                     SaveShorts(MiscHist, f);

            for (var x = 0; x < Constants.WorldWidth; ++x)
            for (var y = 0; y < Constants.WorldHeight; ++y)
                result = result && SaveShort((short)Map[x, y], f);
        }

        return result;
    }

    /// <summary>
    ///     Load a scenario.
    /// </summary>
    /// <param name="s">Scenario to load.</param>
    public void LoadScenario(Scenario s)
    {
        string name = null;
        string fname = null;

        CityFileName = "";

        SetGameLevel(Levels.Easy);

        if (s < Scenario.Dullsville || s > Scenario.Rio) s = Scenario.Dullsville;

        switch (s)
        {
            case Scenario.Dullsville:
                name = "Dullsville";
                fname = "snro.111";
                Scenario = Scenario.Dullsville;
                CityTime = (1900 - 1900) * 48 + 2;
                SetFunds(5000);
                break;
            case Scenario.SanFrancisco:
                name = "San Francisco";
                fname = "snro.222";
                Scenario = Scenario.SanFrancisco;
                CityTime = (1906 - 1900) * 48 + 2;
                SetFunds(20000);
                break;
            case Scenario.Hamburg:
                name = "Hamburg";
                fname = "snro.333";
                Scenario = Scenario.Hamburg;
                CityTime = (1944 - 1900) * 48 + 2;
                SetFunds(20000);
                break;
            case Scenario.Bern:
                name = "Bern";
                fname = "snro.444";
                Scenario = Scenario.Bern;
                CityTime = (1965 - 1900) * 48 + 2;
                SetFunds(20000);
                break;
            case Scenario.Tokyo:
                name = "Tokyo";
                fname = "snro.555";
                Scenario = Scenario.Tokyo;
                CityTime = (1957 - 1900) * 48 + 2;
                SetFunds(20000);
                break;
            case Scenario.Detroit:
                name = "Detroit";
                fname = "snro.666";
                Scenario = Scenario.Detroit;
                CityTime = (1972 - 1900) * 48 + 2;
                SetFunds(20000);
                break;
            case Scenario.Boston:
                name = "Boston";
                fname = "snro.777";
                Scenario = Scenario.Boston;
                CityTime = (2010 - 1900) * 48 + 2;
                SetFunds(20000);
                break;
            case Scenario.Rio:
                name = "Rio de Janeiro";
                fname = "snro.888";
                Scenario = Scenario.Rio;
                CityTime = (2047 - 1900) * 48 + 2;
                SetFunds(20000);
                break;
        }

        SetCleanCityName(name);
        SetSpeed(3);
        SetCityTax(7);

        LoadFileDir(
            fname,
            ResourceDir);

        InitWillStuff();
        InitFundingLevel();
        UpdateFunds();
        InvalidateMaps();
        InitSimLoad = 1;
        DoInitialEval = false;
        DoSimInit();
        DidLoadScenario();
    }

    /// <summary>
    ///     Report to the front-end that the scenario was loaded.
    /// </summary>
    public void DidLoadScenario()
    {
        Callback("didLoadScenario", "");
    }

    /// <summary>
    ///     Try to load a new game from disk.
    ///     TODO: In what state is the game left when loading fails?
    ///     TODO: String normalization code is duplicated in Micropolis::saveCityAs(). Extract to a sub-function.
    /// </summary>
    /// <param name="filename">Name of the file to load.</param>
    /// <returns>Game was loaded successfully.</returns>
    public bool LoadCity(string filename)
    {
        if (LoadFile(filename))
        {
            CityFileName = filename;

            var lastSlash = CityFileName.LastIndexOf('/');
            var pos = lastSlash == -1 ? 0 : lastSlash + 1;

            var lastDot = CityFileName.LastIndexOf('.');
            var last =
                lastDot == -1 ? CityFileName.Length : lastDot;

            var newCityName = CityFileName.Substring(pos, last - pos);
            SetCityName(newCityName);

            DidLoadCity();

            return true;
        }

        DidntLoadCity(string.IsNullOrWhiteSpace(filename) ? "(null)" : filename);

        return false;
    }

    /// <summary>
    ///     Report to the frontend that the game was successfully loaded.
    /// </summary>
    public void DidLoadCity()
    {
        Callback("didLoadCity", "");
    }

    /// <summary>
    ///     Report to the frontend that the game failed to load.
    /// </summary>
    /// <param name="message">File that attempted to load</param>
    public void DidntLoadCity(string message)
    {
        Callback("didntLoadCity", "s", message);
    }

    /// <summary>
    ///     Try to save the game.
    /// </summary>
    public void SaveCity()
    {
        if (CityFileName.Length > 0)
        {
            DoSaveCityAs();
        }
        else
        {
            if (SaveFile(CityFileName))
                DidSaveCity();
            else
                DidntSaveCity(CityFileName);
        }
    }

    /// <summary>
    ///     Report to the frontend that the city is being saved.
    /// </summary>
    public void DoSaveCityAs()
    {
        Callback("saveCityAs", "");
    }

    /// <summary>
    ///     Report to the frontend that the city was saved successfully.
    /// </summary>
    public void DidSaveCity()
    {
        Callback("didSaveCity", "");
    }

    /// <summary>
    ///     Report to the frontend that the city could not be saved.
    /// </summary>
    /// <param name="message">Name of the file used</param>
    public void DidntSaveCity(string message)
    {
        Callback("didntSaveCity", "s", message);
    }

    /// <summary>
    ///     Save the city under a new name (?)
    ///     TODO: String normalization code is duplicated in Micropolis::loadCity(). Extract to a sub-function.
    /// </summary>
    /// <param name="filename">Name of the file to use for storing the game.</param>
    public void SaveCityAs(string filename)
    {
        CityFileName = filename;

        if (SaveFile(CityFileName))
        {
            var lastDot = CityFileName.LastIndexOf('.');
            var lastSlash = CityFileName.LastIndexOf('/');

            var pos = lastSlash == -1 ? 0 : lastSlash + 1;
            var last = lastDot == -1 ? CityFileName.Length : lastDot;
            var len = last - pos;

            var newCityName = CityFileName.Substring(pos, len);

            SetCityName(newCityName);

            DidSaveCity();
        }
        else
        {
            DidntSaveCity(CityFileName);
        }
    }
}