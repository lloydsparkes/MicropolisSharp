using MicropolisSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// From Update.cpp
/// </summary>

namespace MicropolisSharp
{
    public partial class Micropolis
    {
        public bool MustUpdateFunds { get; private set; }
        public bool MustUpdateOptions { get; private set; }
        public long CityTimeLast { get; private set; }
        public long CityYearLast { get; private set; }
        public long CityMonthLast { get; private set; }
        public long TotalFundsLast { get; private set; }
        public long ResLast { get; private set; }
        public long ComLast { get; private set; }
        public long IndLast { get; private set; }

        public void DoUpdateHeads()
        {
            ShowValves();
            DoTimeStuff();
            ReallyUpdateFunds();
            UpdateOptions();
        }

        public void UpdateMaps() {
            InvalidateMaps();
            DoUpdateHeads();
        }

        public void UpdateGraphs()
        {
            ChangeCensus();
        }

        public void UpdateEvaluation()
        {
            ChangeEval();
        }

        public void UpdateHeads()
        {
            MustUpdateFunds = true;
            ValveFlag = true;
            CityTimeLast = CityYearLast = CityMonthLast = TotalFundsLast =
              ResLast = ComLast = IndLast = -999999;
            DoUpdateHeads();
        }

        public void UpdateFunds()
        {
            MustUpdateFunds = true;
        }

        public void ReallyUpdateFunds()
        {
            if (!MustUpdateFunds)
            {
                return;
            }

            MustUpdateFunds = false;

            if (TotalFunds != TotalFundsLast)
            {
                TotalFundsLast = TotalFunds;

                Callback("update", "s", "funds");
            }
        }

        public void DoTimeStuff()
        {
            UpdateDate();
        }

        public void UpdateDate()
        {
            int megalinium = 1000000;

            CityTimeLast = CityTime >> 2;

            CityYear = ((int)CityTime / 48) + (int)StartingYear;
            CityMonth = ((int)CityTime % 48) >> 2;

            if (CityYear >= megalinium)
            {
                SetYear(StartingYear);
                CityYear = StartingYear;
                SendMessage(GeneralMessages.MESSAGE_NOT_ENOUGH_POWER, Constants.NoWhere, Constants.NoWhere, true, false);

            }

            if ((CityYearLast != CityYear) ||
                (CityMonthLast != CityMonth))
            {

                CityYearLast = CityYear;
                CityMonthLast = CityMonth;

                Callback("update", "s", "date");
            }
        }

        public void ShowValves()
        {
            if (ValveFlag)
            {
                DrawValve();
                ValveFlag = false;
            }
        }

        public void DrawValve()
        {
            float r, c, i;

            r = ResValve;

            if (r < -1500)
            {
                r = -1500;
            }

            if (r > 1500)
            {
                r = 1500;
            }

            c = ComValve;

            if (c < -1500)
            {
                c = -1500;
            }

            if (c > 1500)
            {
                c = 1500;
            }

            i = IndValve;

            if (i < -1500)
            {
                i = -1500;
            }

            if (i > 1500)
            {
                i = 1500;
            }

            if ((r != ResLast) ||
                (c != ComLast) ||
                (i != IndLast))
            {

                ResLast = (int)r;
                ComLast = (int)c;
                IndLast = (int)i;

                SetDemand(r, c, i);
            }
        }

        public void SetDemand(float r, float c, float i)
        {
            Callback("update", "s", "demand");
        }

        public void UpdateOptions()
        {
            if (MustUpdateOptions)
            {
                MustUpdateOptions = false;
                Callback("update", "s", "options");
            }
        }

        public void UpdateUserInterface()
        {
            /// @todo Send all pending update messages to the user interface.

            // city: after load file, load scenario, or generate city
            // map: when small overall map changes
            // editor: when large close-up map changes
            // graph: when graph changes
            // evaluation: when evaluation changes
            // budget: when budget changes
            // date: when date changes
            // funds: when funds change
            // demand: when demand changes
            // level: when level changes
            // speed: when speed changes
            // delay: when delay changes
            // option: when options change
        }

    }
}
