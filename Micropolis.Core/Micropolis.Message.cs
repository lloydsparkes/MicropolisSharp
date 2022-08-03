/* Micropolis.Message.cs
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

using System.Diagnostics;
using MicropolisSharp.Types;

namespace MicropolisSharp;

/// <summary>
///     Partial Class Containing the content of message.cpp
/// </summary>
public partial class Micropolis
{
    /// <summary>
    ///     The Cities Population, at the last classification check
    /// </summary>
    public long CityPopulationLast { get; private set; }

    /// <summary>
    ///     THe Last City Classification
    /// </summary>
    public short CategoryLast { get; private set; }

    /// <summary>
    ///     Enable Auto GO TO
    ///     When an important event happens, the map display will jump to location of the event
    /// </summary>
    public bool AutoGoTo { get; private set; }

    /// <summary>
    ///     Check progress of the user, and send him messages about it.
    /// </summary>
    public void SendMessages()
    {
        short PowerPop;
        float TM;

        // Running a scenario, and waiting it to 'end' so we can give a score
        if (Scenario > Scenario.None && ScoreType > Scenario.None && scoreWait > 0)
        {
            scoreWait--;
            if (scoreWait == 0) DoScenarioScore(ScoreType);
        }

        CheckGrowth();

        TotalZonePop = ResZonePop + ComZonePop + IndZonePop;
        PowerPop = (short)(NuclearPowerPop + CoalPowerPop);

        switch (CityTime & 63)
        {
            case 1:
                if (TotalZonePop / 4 >= ResZonePop) SendMessage(GeneralMessages.MESSAGE_NEED_MORE_RESIDENTIAL);
                break;

            case 5:
                if (TotalZonePop / 8 >= ComZonePop) SendMessage(GeneralMessages.MESSAGE_NEED_MORE_COMMERCIAL);
                break;

            case 10:
                if (TotalZonePop / 8 >= IndZonePop) SendMessage(GeneralMessages.MESSAGE_NEED_MORE_INDUSTRIAL);
                break;

            case 14:
                if (TotalZonePop > 10 && TotalZonePop * 2 > RoadTotal)
                    SendMessage(GeneralMessages.MESSAGE_NEED_MORE_ROADS);
                break;

            case 18:
                if (TotalZonePop > 50 && TotalZonePop > RailTotal) SendMessage(GeneralMessages.MESSAGE_NEED_MORE_RAILS);
                break;

            case 22:
                if (TotalZonePop > 10 && PowerPop == 0) SendMessage(GeneralMessages.MESSAGE_NEED_ELECTRICITY);
                break;

            case 26:
                if (ResPop > 500 && StadiumPop == 0)
                {
                    SendMessage(GeneralMessages.MESSAGE_NEED_STADIUM);
                    ResCap = true;
                }
                else
                {
                    ResCap = false;
                }

                break;

            case 28:
                if (IndPop > 70 && SeaportPop == 0)
                {
                    SendMessage(GeneralMessages.MESSAGE_NEED_SEAPORT);
                    IndCap = true;
                }
                else
                {
                    IndCap = false;
                }

                break;

            case 30:
                if (ComPop > 100 && AirportPop == 0)
                {
                    SendMessage(GeneralMessages.MESSAGE_NEED_AIRPORT);
                    ComCap = true;
                }
                else
                {
                    ComCap = false;
                }

                break;

            case 32:
                TM = UnpoweredZoneCount + PoweredZoneCount; /* dec score for unpowered zones */
                if (TM > 0)
                    if (PoweredZoneCount / TM < 0.7)
                        SendMessage(GeneralMessages.MESSAGE_BLACKOUTS_REPORTED);
                break;

            case 35:
                if (PollutionAverage > /* 80 */ 60) SendMessage(GeneralMessages.MESSAGE_HIGH_POLLUTION, -1, -1, true);
                break;

            case 42:
                if (CrimeAverage > 100) SendMessage(GeneralMessages.MESSAGE_HIGH_CRIME, -1, -1, true);
                break;

            case 45:
                if (TotalPop > 60 && FireStationPop == 0) SendMessage(GeneralMessages.MESSAGE_NEED_FIRE_STATION);
                break;

            case 48:
                if (TotalPop > 60 && PoliceStationPop == 0) SendMessage(GeneralMessages.MESSAGE_NEED_POLICE_STATION);
                break;

            case 51:
                if (CityTax > 12) SendMessage(GeneralMessages.MESSAGE_TAX_TOO_HIGH);
                break;

            case 54:
                // If roadEffect < 5/8 of max effect
                if (RoadEffect < 5 * Constants.MaxRoadEffect / 8 && RoadTotal > 30)
                    SendMessage(GeneralMessages.MESSAGE_ROAD_NEEDS_FUNDING);
                break;

            case 57:
                // If fireEffect < 0.7 of max effect
                if (FireEffect < 7 * Constants.MaxFireStationEffect / 10 && TotalPop > 20)
                    SendMessage(GeneralMessages.MESSAGE_FIRE_STATION_NEEDS_FUNDING);
                break;

            case 60:
                // If policeEffect < 0.7 of max effect
                if (PoliceEffect < 7 * Constants.MaxPoliceStationEffect / 10
                    && TotalPop > 20)
                    SendMessage(GeneralMessages.MESSAGE_POLICE_NEEDS_FUNDING);
                break;

            case 63:
                if (TrafficAverage > 60) SendMessage(GeneralMessages.MESSAGE_TRAFFIC_JAMS, -1, -1, true);
                break;
        }
    }

    /// <summary>
    ///     Detect a change in city class, and produce a message if the player has
    ///     reached the next class.
    ///     TODO: This code is very closely related to DoPopNum - merge both in some way?
    ///     (This function gets called much more often however then doPopNum().
    ///     Also, at the first call, the difference between thisCityPop and
    ///     cityPop is huge.)
    /// </summary>
    public void CheckGrowth()
    {
        if ((CityTime & 3) == 0)
        {
            short category = 0;
            var thisCityPop = GetPopulation();

            if (CityPopulationLast > 0)
            {
                var lastClass = GetCityClass(CityPopulationLast);
                var newClass = GetCityClass(thisCityPop);

                if (lastClass != newClass)
                    // Switched class, find appropiate message.
                    switch (newClass)
                    {
                        case CityClassification.Village:
                            // Don't mention it.
                            break;

                        case CityClassification.Town:
                            category = (short)GeneralMessages.MESSAGE_REACHED_TOWN;
                            break;

                        case CityClassification.City:
                            category = (short)GeneralMessages.MESSAGE_REACHED_CITY;
                            break;

                        case CityClassification.Capital:
                            category = (short)GeneralMessages.MESSAGE_REACHED_CAPITAL;
                            break;

                        case CityClassification.Metropolis:
                            category = (short)GeneralMessages.MESSAGE_REACHED_METROPOLIS;
                            break;

                        case CityClassification.Megalopolis:
                            category = (short)GeneralMessages.MESSAGE_REACHED_MEGALOPOLIS;
                            break;
                    }
            }

            if (category > 0 && category != CategoryLast)
            {
                SendMessage((GeneralMessages)category, Constants.NoWhere, Constants.NoWhere, true);
                CategoryLast = category;
            }

            CityPopulationLast = thisCityPop;
        }
    }

    /// <summary>
    ///     Compute score for each scenario
    ///     type CANNOT be SC_NONE
    /// </summary>
    /// <param name="type">Scenario used</param>
    public void DoScenarioScore(Scenario type)
    {
        var z = GeneralMessages.MESSAGE_SCENARIO_LOST; /* you lose */

        switch (type)
        {
            case Scenario.Dullsville:
                if (CityClassification >= CityClassification.Metropolis) z = GeneralMessages.MESSAGE_SCENARIO_WON;
                break;

            case Scenario.SanFrancisco:
                if (CityClassification >= CityClassification.Metropolis) z = GeneralMessages.MESSAGE_SCENARIO_WON;
                break;

            case Scenario.Hamburg:
                if (CityClassification >= CityClassification.Metropolis) z = GeneralMessages.MESSAGE_SCENARIO_WON;
                break;

            case Scenario.Bern:
                if (TrafficAverage < 80) z = GeneralMessages.MESSAGE_SCENARIO_WON;
                break;

            case Scenario.Tokyo:
                if (CityScore > 500) z = GeneralMessages.MESSAGE_SCENARIO_WON;
                break;

            case Scenario.Detroit:
                if (CrimeAverage < 60) z = GeneralMessages.MESSAGE_SCENARIO_WON;
                break;

            case Scenario.Boston:
                if (CityScore > 500) z = GeneralMessages.MESSAGE_SCENARIO_WON;
                break;

            case Scenario.Rio:
                if (CityScore > 500) z = GeneralMessages.MESSAGE_SCENARIO_WON;
                break;
        }

        SendMessage(z, Constants.NoWhere, Constants.NoWhere, true, true);

        if (z == GeneralMessages.MESSAGE_SCENARIO_LOST) DoLoseGame();
    }

    /// <summary>
    ///     Send the user a message of an event that happens at a particular position in the city.
    ///     TODO: Change X,Y to position
    /// </summary>
    /// <param name="msgNum">Message number of the message to display.</param>
    /// <param name="x">X coordinate of the position of the event.</param>
    /// <param name="y">Y coordinate of the position of the event.</param>
    /// <param name="picture">Flag that is true if a picture should be shown.</param>
    /// <param name="important">Flag that is true if the message is important.</param>
    public void SendMessage(GeneralMessages msgNum, short x = -1, short y = -1, bool picture = false,
        bool important = false)
    {
        Callback("update", "sdddbb", "message", msgNum.ToString(), x.ToString(), y.ToString(),
            (picture ? 1 : 0).ToString(), (important ? 1 : 0).ToString());
    }

    /// <summary>
    ///     Make a sound for message \a mesgNum if appropriate.
    ///     TODO: Change X,Y for position
    /// </summary>
    /// <param name="msgNum">Message number displayed.</param>
    /// <param name="x">Horizontal coordinate in the city of the sound.</param>
    /// <param name="y">Vertical coordinate in the city of the sound.</param>
    public void DoMakeSound(int msgNum, int x, int y)
    {
        Debug.Assert(msgNum >= 0);

        switch ((GeneralMessages)msgNum)
        {
            case GeneralMessages.MESSAGE_TRAFFIC_JAMS:
                if (GetRandom(5) == 1)
                    MakeSound("city", "HonkHonkMed", x, y);
                else if (GetRandom(5) == 1)
                    MakeSound("city", "HonkHonkLow", x, y);
                else if (GetRandom(5) == 1) MakeSound("city", "HonkHonkHigh", x, y);
                break;

            case GeneralMessages.MESSAGE_HIGH_CRIME:
            case GeneralMessages.MESSAGE_FIRE_REPORTED:
            case GeneralMessages.MESSAGE_TORNADO_SIGHTED:
            case GeneralMessages.MESSAGE_EARTHQUAKE:
            case GeneralMessages.MESSAGE_PLANE_CRASHED:
            case GeneralMessages.MESSAGE_SHIP_CRASHED:
            case GeneralMessages.MESSAGE_TRAIN_CRASHED:
            case GeneralMessages.MESSAGE_HELICOPTER_CRASHED:
                MakeSound("city", "Siren", x, y);
                break;

            case GeneralMessages.MESSAGE_MONSTER_SIGHTED:
                MakeSound("city", "Monster", x, y);
                break;

            case GeneralMessages.MESSAGE_FIREBOMBING:
                MakeSound("city", "ExplosionLow", x, y);
                MakeSound("city", "Siren", x, y);
                break;

            case GeneralMessages.MESSAGE_NUCLEAR_MELTDOWN:
                MakeSound("city", "ExplosionHigh", x, y);
                MakeSound("city", "ExplosionLow", x, y);
                MakeSound("city", "Siren", x, y);
                break;

            case GeneralMessages.MESSAGE_RIOTS_REPORTED:
                MakeSound("city", "Siren", x, y);
                break;
        }
    }

    /// <summary>
    ///     Tell the front-end that it should perform an auto-goto
    ///     TODO: Change X,Y for position
    /// </summary>
    /// <param name="x">Horizontal coordinate in the city of the sound.</param>
    /// <param name="y">Vertical coordinate in the city of the sound.</param>
    /// <param name="msg">Message</param>
    public void DoAutoGoTo(short x, short y, string msg)
    {
        Callback("autoGoto", "dd", x.ToString(), y.ToString());
    }

    /// <summary>
    ///     Tell the front-end that the player has lost the game
    /// </summary>
    public void DoLoseGame()
    {
        Callback("loseGame", "");
    }

    /// <summary>
    ///     Tell the front-end that the player has won the game
    /// </summary>
    public void DoWinGame()
    {
        Callback("winGame", "");
    }
}