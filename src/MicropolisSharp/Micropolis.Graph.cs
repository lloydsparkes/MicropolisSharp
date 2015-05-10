using MicropolisSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicropolisSharp
{
    public partial class Micropolis
    {
        public bool HistoryInitialized { get; private set; }
        public int Graph10Max { get; private set; }
        public int Graph120Max { get; private set; }

        public char[] DrawMonth(short[] hist, float scale)
        {
            var result = new char[120];
            int val, x;

            for (x = 0; x < 120; x++)
            {
                val = (int)(hist[x] * scale);
                result[119 - x] = (char)Utilities.Restrict(val, 0, 255);
            }
            return result;
        }

        public void ChangeCensus() { CensusChanged = true; }

        public void GraphDoer()
        {
            if (CensusChanged)
            {
                Callback("update", "s", "history");
                CensusChanged = false;
            }
        }

        public void InitGraphs()
        {
            if (!HistoryInitialized)
            {
                HistoryInitialized = true;
            }
        }

        public void InitGraphMax() {
            int x;

            ResHist10Max = 0;
            ComHist10Max = 0;
            IndHist10Max = 0;

            for (x = 118; x >= 0; x--)
            {

                if (ResHist[x] < 0)
                {
                    ResHist[x] = 0;
                }
                if (ComHist[x] < 0)
                {
                    ComHist[x] = 0;
                }
                if (IndHist[x] < 0)
                {
                    IndHist[x] = 0;
                }

                ResHist10Max = Math.Max(ResHist10Max, ResHist[x]);
                ComHist10Max = Math.Max(ComHist10Max, ComHist[x]);
                IndHist10Max = Math.Max(IndHist10Max, IndHist[x]);

            }

            Graph10Max = (short)Math.Max(ResHist10Max, Math.Max(ComHist10Max, IndHist10Max));

            ResHist120Max = 0;
            ComHist120Max = 0;
            IndHist120Max = 0;

            for (x = 238; x >= 120; x--)
            {

                if (ResHist[x] < 0)
                {
                    ResHist[x] = 0;
                }
                if (ComHist[x] < 0)
                {
                    ComHist[x] = 0;
                }
                if (IndHist[x] < 0)
                {
                    IndHist[x] = 0;
                }

                ResHist10Max = Math.Max(ResHist10Max, ResHist[x]);
                ComHist10Max = Math.Max(ComHist10Max, ComHist[x]);
                IndHist10Max = Math.Max(IndHist10Max, IndHist[x]);

            }

            Graph120Max = (short)Math.Max(ResHist120Max, Math.Max(ComHist120Max, IndHist120Max));
        }

        public void GetHistoryRange(HistoryType historyType, HistoryScale historyScale, ref short minValResult, ref short maxValResult) {
            if (historyType < 0 || historyType >= HistoryType.Count || historyScale < 0 || historyScale >= HistoryScale.Count)
            {
                minValResult = 0;
                maxValResult = 0;
                return;
            }

            short[] history = null;
            switch (historyType)
            {
                case HistoryType.Res:
                    history = ResHist;
                    break;
                case HistoryType.Com:
                    history = ComHist;
                    break;
                case HistoryType.Ind:
                    history = IndHist;
                    break;
                case HistoryType.Money:
                    history = MoneyHist;
                    break;
                case HistoryType.Crime:
                    history = CrimeHist;
                    break;
                case HistoryType.Pollution:
                    history = PollutionHist;
                    break;
                default:
                    //NOT_REACHED();
                    break;
            }

            int offset = 0;
            switch (historyScale)
            {
                case HistoryScale.Short:
                    offset = 0;
                    break;
                case HistoryScale.Long:
                    offset = 120;
                    break;
                default:
                    //NOT_REACHED();
                    break;
            }

            short minVal = 32000;
            short maxVal = -32000;

            for (int i = 0; i < Constants.HistoryCount; i++)
            {
                short val = history[i + offset];

                minVal = Math.Min(val, minVal);
                maxVal = Math.Max(val, maxVal);
            }

            minValResult = minVal;
            maxValResult = maxVal;
        }

        public short GetHistory(HistoryType historyType, HistoryScale historyScale, int historyIndex) {
            if (historyType < 0 || historyType >= HistoryType.Count
                || historyScale < 0 || historyScale >= HistoryScale.Count
                || historyIndex < 0 || historyIndex >= Constants.HistoryCount)
            {
                return 0;
            }

            short[] history = null;
            switch (historyType)
            {
                case HistoryType.Res:
                    history = ResHist;
                    break;
                case HistoryType.Com:
                    history = ComHist;
                    break;
                case HistoryType.Ind:
                    history = IndHist;
                    break;
                case HistoryType.Money:
                    history = MoneyHist;
                    break;
                case HistoryType.Crime:
                    history = CrimeHist;
                    break;
                case HistoryType.Pollution:
                    history = PollutionHist;
                    break;
                default:
                    //NOT_REACHED();
                    break;
            }

            int offset = 0;
            switch (historyScale)
            {
                case HistoryScale.Short:
                    offset = 0;
                    break;
                case HistoryScale.Long:
                    offset = 120;
                    break;
                default:
                    //NOT_REACHED();
                    break;
            }

            short result = history[historyIndex + offset];

            return result;
        }

        public void SetHistory(HistoryType historyType, HistoryScale historyScale, int historyIndex, short historyValue)
        {
            if (historyType < 0 || historyType >= HistoryType.Count
                || historyScale < 0 || historyScale >= HistoryScale.Count
                || historyIndex < 0 || historyIndex >= Constants.HistoryCount)
            {
                return;
            }

            short[] history = null;
            switch (historyType)
            {
                case HistoryType.Res:
                    history = ResHist;
                    break;
                case HistoryType.Com:
                    history = ComHist;
                    break;
                case HistoryType.Ind:
                    history = IndHist;
                    break;
                case HistoryType.Money:
                    history = MoneyHist;
                    break;
                case HistoryType.Crime:
                    history = CrimeHist;
                    break;
                case HistoryType.Pollution:
                    history = PollutionHist;
                    break;
                default:
                    //NOT_REACHED();
                    break;
            }

            int offset = 0;
            switch (historyScale)
            {
                case HistoryScale.Short:
                    offset = 0;
                    break;
                case HistoryScale.Long:
                    offset = 120;
                    break;
                default:
                    //NOT_REACHED();
                    break;
            }

            history[historyIndex + offset] = historyValue;
        }
    }
}
