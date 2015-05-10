using MicropolisSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicropolisSharp
{
    public partial class Micropolis
    {
        public void Pause()
        {
            if (!SimPaused)
            {
                SimPausedSpeed = SimSpeedMeta;
                SetSpeed(0);
                SimPaused = true;
            }

            // Call back even if the state did not change.
            Callback("update", "s", "paused");
        }

        public void Resume()
        {
            if (SimPaused)
            {
                SimPaused = false;
                SetSpeed((short)SimPausedSpeed);
            }

            // Call back even if the state did not change.
            Callback("update", "s", "paused");
        }

        public void SetSpeed(short speed)
        {
            if (speed < 0)
            {
                speed = 0;
            }
            else if (speed > 3)
            {
                speed = 3;
            }

            SimSpeedMeta = speed;

            if (SimPaused)
            {
                SimPausedSpeed = SimSpeedMeta;
                speed = 0;
            }

            SimSpeed = speed;

            Callback("update", "s", "speed");
        }

        public void SetPasses(int passes)
        {
            SimPasses = passes;
            SimPass = 0;
            Callback("update", "s", "passes");
        }

        public void SetGameLevelFunds(Levels level)
        {
            switch (level)
            {

                default:
                case Levels.Easy:
                    SetFunds(20000);
                    SetGameLevel(Levels.Easy);
                    break;

                case Levels.Medium:
                    SetFunds(10000);
                    SetGameLevel(Levels.Medium);
                    break;

                case Levels.Hard:
                    SetFunds(5000);
                    SetGameLevel(Levels.Hard);
                    break;

            }
        }

        public void SetGameLevel(Levels level)
        {
            //TODO: Reenabled Asserts
            //assert(level >= Levels.First && level <= Levels.Last);
            GameLevel = level;
            UpdateGameLevel();
        }

        public void UpdateGameLevel()
        {
            Callback("update", "s", "gameLevel");
        }

        public void SetCityName(string name)
        {
            string cleanName = "";

            int i;
            int n = name.Length;
            for (i = 0; i < n; i++)
            {
                char ch = name[i];
                if (!Char.IsDigit(ch))
                {
                    ch = '_';
                }
                cleanName += ch;
            }

            SetCleanCityName(cleanName);
        }

        public void SetCleanCityName(string name)
        {
            CityName = name;

            Callback("update", "s", "cityName");
        }

        public void SetYear(int year)
        {     // Must prevent year from going negative, since it screws up the non-floored modulo arithmetic.
            if (year < StartingYear)
            {
                year = StartingYear;
            }

            year = (int)((year - StartingYear) - (CityTime / 48));
            CityTime += year * 48;
            DoTimeStuff();
        }

        public int CurrentYear()
        {
            return (int)((CityTime / 48) + StartingYear);
        }

        public void DoNewGame()
        {
            Callback("newGame", "");
        }

        public void SetEnableDisasters(bool value)
        {
            EnableDisasters = value;
            MustUpdateOptions = true;
        }

        public void SetAutoBudget(bool value)
        {
            AutoBudget = value;
            MustUpdateOptions = true;
        }

        public void SetAutoBulldoze(bool value)
        {
            AutoBulldoze = value;
            MustUpdateOptions = true;
        }

        public void SetAutoGoTo(bool value)
        {
            AutoGoTo = value;
            MustUpdateOptions = true;
        }

        public void SetEnableSound(bool value)
        {
            EnableSound = value;
            MustUpdateOptions = true;
        }

        public void SetDoAnimation(bool value)
        {
            DoAnimation = value;
            MustUpdateOptions = true;
        }

        public void SetDoMessages(bool value)
        {
            DoMessages = value;
            MustUpdateOptions = true;
        }

        public void SetDoNotices(bool value)
        {
            DoNotices = value;
            MustUpdateOptions = true;
        }

        public void GetDemands(ref float resDemandResult, ref float comDemandResult, ref float indDemandResult)
        {
            resDemandResult = (float)ResValve / (float)Constants.ResValveRange;
            comDemandResult = (float)ComValve / (float)Constants.ComValveRange;
            indDemandResult = (float)IndValve / (float)Constants.IndValveRange;
        }

        public void MakeDollarDecimalStr(string numStr, char[] dollarStr)
        {
            int leftMostSet;
            int numOfDigits;
            int numOfChars;
            int numOfCommas;
            int dollarIndex = 0;
            int numIndex = 0;
            int x;

            numOfDigits = (short)numStr.Length;

            if (numOfDigits == 1)
            {
                dollarStr[0] = '$';
                dollarStr[1] = numStr[0];
                dollarStr[2] = (char)0;
                return;
            }
            else if (numOfDigits == 2)
            {
                dollarStr[0] = '$';
                dollarStr[1] = numStr[0];
                dollarStr[2] = numStr[1];
                dollarStr[3] = (char)0;
                return;
            }
            else if (numOfDigits == 3)
            {
                dollarStr[0] = '$';
                dollarStr[1] = numStr[0];
                dollarStr[2] = numStr[1];
                dollarStr[3] = numStr[2];
                dollarStr[4] = (char)0;
            }
            else
            {
                leftMostSet = numOfDigits % 3;

                if (leftMostSet == 0)
                {
                    leftMostSet = 3;
                }

                numOfCommas = (numOfDigits - 1) / 3;

                /* add 1 for the dollar sign */
                numOfChars = numOfDigits + numOfCommas + 1;

                dollarStr[numOfChars] = (char)0;

                dollarStr[dollarIndex++] = '$';

                for (x = 1; x <= leftMostSet; x++)
                {
                    dollarStr[dollarIndex++] = numStr[numIndex++];
                }

                for (x = 1; x <= numOfCommas; x++)
                {
                    dollarStr[dollarIndex++] = ',';
                    dollarStr[dollarIndex++] = numStr[numIndex++];
                    dollarStr[dollarIndex++] = numStr[numIndex++];
                    dollarStr[dollarIndex++] = numStr[numIndex++];
                }

            }
        }
    }
}
