using MicropolisSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// From Message.cpp
/// </summary>

namespace MicropolisSharp
{
    public partial class Micropolis
    {
        public long CityPopulationLast { get; private set; }
        public short CategoryLast { get; private set; }
        public bool AutoGoTo { get; private set; }
        
        public void SendMessages() {
            short PowerPop;
            float TM;

            // Running a scenario, and waiting it to 'end' so we can give a score
            if (Scenario > Scenario.None && ScoreType > Scenario.None && scoreWait > 0)
            {
                scoreWait--;
                if (scoreWait == 0)
                {
                    DoScenarioScore(ScoreType);
                }
            }

            CheckGrowth();

            TotalZonePop = ResZonePop + ComZonePop + IndZonePop;
            PowerPop = (short)(NuclearPowerPop + CoalPowerPop);

            switch (CityTime & 63)
            {

                case 1:
                    if (TotalZonePop / 4 >= ResZonePop)
                    {
                        SendMessage(GeneralMessages.MESSAGE_NEED_MORE_RESIDENTIAL);
                    }
                    break;

                case 5:
                    if (TotalZonePop / 8 >= ComZonePop)
                    {
                        SendMessage(GeneralMessages.MESSAGE_NEED_MORE_COMMERCIAL);
                    }
                    break;

                case 10:
                    if (TotalZonePop / 8 >= IndZonePop)
                    {
                        SendMessage(GeneralMessages.MESSAGE_NEED_MORE_INDUSTRIAL);
                    }
                    break;

                case 14:
                    if (TotalZonePop > 10 && TotalZonePop * 2 > RoadTotal)
                    {
                        SendMessage(GeneralMessages.MESSAGE_NEED_MORE_ROADS);
                    }
                    break;

                case 18:
                    if (TotalZonePop > 50 && TotalZonePop > RailTotal)
                    {
                        SendMessage(GeneralMessages.MESSAGE_NEED_MORE_RAILS);
                    }
                    break;

                case 22:
                    if (TotalZonePop > 10 && PowerPop == 0)
                    {
                        SendMessage(GeneralMessages.MESSAGE_NEED_ELECTRICITY);
                    }
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
                    TM = (float)(UnpoweredZoneCount + PoweredZoneCount); /* dec score for unpowered zones */
                    if (TM > 0)
                    {
                        if (PoweredZoneCount / TM < 0.7)
                        {
                            SendMessage(GeneralMessages.MESSAGE_BLACKOUTS_REPORTED);
                        }
                    }
                    break;

                case 35:
                    if (PollutionAverage > /* 80 */ 60)
                    {
                        SendMessage(GeneralMessages.MESSAGE_HIGH_POLLUTION, -1, -1, true);
                    }
                    break;

                case 42:
                    if (CrimeAverage > 100)
                    {
                        SendMessage(GeneralMessages.MESSAGE_HIGH_CRIME, -1, -1, true);
                    }
                    break;

                case 45:
                    if (TotalPop > 60 && FireStationPop == 0)
                    {
                        SendMessage(GeneralMessages.MESSAGE_NEED_FIRE_STATION);
                    }
                    break;

                case 48:
                    if (TotalPop > 60 && PoliceStationPop == 0)
                    {
                        SendMessage(GeneralMessages.MESSAGE_NEED_POLICE_STATION);
                    }
                    break;

                case 51:
                    if (CityTax > 12)
                    {
                        SendMessage(GeneralMessages.MESSAGE_TAX_TOO_HIGH);
                    }
                    break;

                case 54:
                    // If roadEffect < 5/8 of max effect
                    if (RoadEffect < (5 * Constants.MaxRoadEffect / 8) && RoadTotal > 30)
                    {
                        SendMessage(GeneralMessages.MESSAGE_ROAD_NEEDS_FUNDING);
                    }
                    break;

                case 57:
                    // If fireEffect < 0.7 of max effect
                    if (FireEffect < (7 * Constants.MaxFireStationEffect / 10) && TotalPop > 20)
                    {
                        SendMessage(GeneralMessages.MESSAGE_FIRE_STATION_NEEDS_FUNDING);
                    }
                    break;

                case 60:
                    // If policeEffect < 0.7 of max effect
                    if (PoliceEffect < (7 * Constants.MaxPoliceStationEffect / 10)
                                                                    && TotalPop > 20)
                    {
                        SendMessage(GeneralMessages.MESSAGE_POLICE_NEEDS_FUNDING);
                    }
                    break;

                case 63:
                    if (TrafficAverage > 60)
                    {
                        SendMessage(GeneralMessages.MESSAGE_TRAFFIC_JAMS, -1, -1, true);
                    }
                    break;

            }
        }
        public void CheckGrowth() {
            if ((CityTime & 3) == 0)
            {
                short category = 0;
                long thisCityPop = GetPopulation();

                if (CityPopulationLast > 0)
                {

                    CityClassification lastClass = GetCityClass(CityPopulationLast);
                    CityClassification newClass = GetCityClass(thisCityPop);

                    if (lastClass != newClass)
                    {

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

                            default:
                                //NOT_REACHED();
                                break;

                        }
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

        public void DoScenarioScore(Scenario type) {
            GeneralMessages z = GeneralMessages.MESSAGE_SCENARIO_LOST;     /* you lose */

            switch (type)
            {

                case Scenario.Dullsville:
                    if (CityClassification >= CityClassification.Metropolis)
                    {
                        z = GeneralMessages.MESSAGE_SCENARIO_WON;
                    }
                    break;

                case Scenario.SanFrancisco:
                    if (CityClassification >= CityClassification.Metropolis)
                    {
                        z = GeneralMessages.MESSAGE_SCENARIO_WON;
                    }
                    break;

                case Scenario.Hamburg:
                    if (CityClassification >= CityClassification.Metropolis)
                    {
                        z = GeneralMessages.MESSAGE_SCENARIO_WON;
                    }
                    break;

                case Scenario.Bern:
                    if (TrafficAverage < 80)
                    {
                        z = GeneralMessages.MESSAGE_SCENARIO_WON;
                    }
                    break;

                case Scenario.Tokyo:
                    if (CityScore > 500)
                    {
                        z = GeneralMessages.MESSAGE_SCENARIO_WON;
                    }
                    break;

                case Scenario.Detroit:
                    if (CrimeAverage < 60)
                    {
                        z = GeneralMessages.MESSAGE_SCENARIO_WON;
                    }
                    break;

                case Scenario.Boston:
                    if (CityScore > 500)
                    {
                        z = GeneralMessages.MESSAGE_SCENARIO_WON;
                    }
                    break;

                case Scenario.Rio:
                    if (CityScore > 500)
                    {
                        z = GeneralMessages.MESSAGE_SCENARIO_WON;
                    }
                    break;

                default:
                    //NOT_REACHED();
                    break;

            }

            SendMessage(z, Constants.NoWhere, Constants.NoWhere, true, true);

            if (z == GeneralMessages.MESSAGE_SCENARIO_LOST)
            {
                DoLoseGame();
            }
        }

        public void SendMessage(GeneralMessages msgNum, short x = -1, short y = -1, bool picture = false, bool important = false)
        {
            Callback("update", "sdddbb", "message", msgNum.ToString(), x.ToString(), y.ToString(), (picture ? 1 : 0).ToString(), (important ? 1 : 0).ToString());
        }

        public void DoMakeSound(int msgNum, int x, int y) {
            //TODO: Reenable Asserts
            //assert(mesgNum >= 0);

            switch ((GeneralMessages)msgNum)
            {

                case GeneralMessages.MESSAGE_TRAFFIC_JAMS:
                    if (GetRandom(5) == 1)
                    {
                        MakeSound("city", "HonkHonkMed", x, y);
                    }
                    else if (GetRandom(5) == 1)
                    {
                        MakeSound("city", "HonkHonkLow", x, y);
                    }
                    else if (GetRandom(5) == 1)
                    {
                        MakeSound("city", "HonkHonkHigh", x, y);
                    }
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
        public void DoAutoGoTo(short x, short y, string msg) { Callback("autoGoto", "dd", x.ToString(), y.ToString()); }
        public void DoLoseGame() { Callback("loseGame", ""); }
        public void DoWinGame() { Callback("winGame", ""); }
    }
}
