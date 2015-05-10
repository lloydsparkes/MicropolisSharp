using MicropolisSharp.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// From FileIo.Cpp
/// </summary>

namespace MicropolisSharp
{
    public partial class Micropolis
    {
        private short swapShort(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
                return BitConverter.ToInt16(bytes, 0);
            }
            return BitConverter.ToInt16(bytes, 0); ;
        }

        private bool loadShort(ref short buf, BinaryReader stream)
        {
            byte[] bytes = stream.ReadBytes(sizeof(short));
            if(bytes.Length != sizeof(short))
            {
                return false;
            }
            buf = swapShort(bytes);
            return true;
        }

        private bool loadShorts(ref short[] output, int len, BinaryReader stream)
        {
            bool result = true;
            for(int i = 0; i < len; ++i)
            {
                result = result && loadShort(ref output[i], stream);
            }
            return result;
        }

        private bool saveShort(short buf, BinaryWriter stream)
        {
            short toWrite = swapShort(BitConverter.GetBytes(buf));
            stream.Write(buf);
            return true;
        }

        private bool saveShorts(short[] buf, BinaryWriter stream)
        {
            foreach(short s in buf)
            {
                saveShort(s, stream);
            }
            return true;
        }

        private int halfSwapLong(short[] shorts, int indexOfFirst)
        {
            byte[] bytes = new byte[4];
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

        private void saveIntToShort(short[] output, int outputIndex, int valueToSave)
        {
            byte[] data = BitConverter.GetBytes(valueToSave);

            byte[] short1 = { data[0], data[1] };
            byte[] short2 = { data[2], data[3] };

            output[outputIndex] = BitConverter.ToInt16(short1, 0);
            output[outputIndex+1] = BitConverter.ToInt16(short2, 0);
        }

        public bool LoadFileDir(string filename, string dir)
        {
            BinaryReader f;
            long size;

            // If needed, construct a path to the file.
            if (dir != null)
            {
                filename = dir + Path.DirectorySeparatorChar + filename;
            }

            if (!File.Exists(filename))
            {
                return false;
            }

            FileInfo fileInfo = new FileInfo(filename);

            const int expectedFileSize = 27120;

            bool result = fileInfo.Length == expectedFileSize;

            if (!result)
            {
                return false;
            }

            using (BinaryReader reader = new BinaryReader(File.OpenRead(filename)))
            {
                result = result &&
                  loadShorts(ref ResHist, Constants.HistoryLength, reader) &&
                  loadShorts(ref ComHist, Constants.HistoryLength, reader) &&
                  loadShorts(ref IndHist, Constants.HistoryLength, reader) &&
                  loadShorts(ref CrimeHist, Constants.HistoryLength, reader) &&
                  loadShorts(ref PollutionHist, Constants.HistoryLength, reader) &&
                  loadShorts(ref MoneyHist, Constants.HistoryLength, reader) &&
                  loadShorts(ref MiscHist, Constants.MiscHistoryLength, reader);

                for(int x = 0; x < Constants.WorldWidth; ++x)
                {
                    for(int y = 0; y < Constants.WorldHeight; ++y)
                    {
                        short temp = (short)Map[x, y];
                        result = result && loadShort(ref temp, reader);
                        Map[x, y] = (ushort)temp;
                    }
                }
            }

            return result;
        }

        public bool LoadFile(string filename) {
            int n;

            if (!LoadFileDir(filename, null))
            {
                return false;
            }

            /* total funds is a long.....    miscHist is array of shorts */
            /* total funds is being put in the 50th & 51th word of miscHist */
            /* find the address, cast the ptr to a longPtr, take contents */

            n = halfSwapLong(MiscHist, 50);
            SetFunds(n);

            n = halfSwapLong(MiscHist, 8);
            CityTime = n;

            SetAutoBulldoze(MiscHist[52] != 0);   // flag for autoBulldoze
            SetAutoBudget(MiscHist[53] != 0);     // flag for autoBudget
            SetAutoGoTo(MiscHist[54] != 0);       // flag for auto-goto
            SetEnableSound(MiscHist[55] != 0);    // flag for the sound on/off
            SetCityTax(MiscHist[56]);
            SetSpeed(MiscHist[57]);
            ChangeCensus();
            MustUpdateOptions = true;

            /* yayaya */
            PolicePercentage = ((float)halfSwapLong(MiscHist, 58)) / ((float)65536);
            FirePercentage = (float)halfSwapLong(MiscHist, 60) / (float)65536.0;
            RoadPercentage = (float)halfSwapLong(MiscHist, 62) / (float)65536.0;

            CityTime = Math.Max(0, CityTime);

            // If the tax is nonsensical, set it to a reasonable value.
            if (CityTax > 20 || CityTax < 0)
            {
                SetCityTax(7);
            }

            // If the speed is nonsensical, set it to a reasonable value.
            if (SimSpeed < 0 || SimSpeed > 3)
            {
                SetSpeed(3);
            }

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

        public bool SaveFile(string filename) {
            long n;

            if (!File.Exists(filename))
            {
                return false;
            }

            /* total funds is a long.....    miscHist is array of ints */
            /* total funds is bien put in the 50th & 51th word of miscHist */
            /* find the address, cast the ptr to a longPtr, take contents */
            saveIntToShort(MiscHist, 50, (int)TotalFunds);
            saveIntToShort(MiscHist, 50, (int)CityTime);

            MiscHist[52] = AutoBulldoze.ToShort();   // flag for autoBulldoze
            MiscHist[53] = AutoBudget.ToShort();     // flag for autoBudget
            MiscHist[54] = AutoGoTo.ToShort();       // flag for auto-goto
            MiscHist[55] = EnableSound.ToShort();    // flag for the sound on/off
            MiscHist[57] = SimSpeed;
            MiscHist[56] = (short)CityTax;        /* post release */

            /* yayaya */
            saveIntToShort(MiscHist, 58, (int)PolicePercentage);
            saveIntToShort(MiscHist, 60, (int)FirePercentage);
            saveIntToShort(MiscHist, 62, (int)RoadPercentage);

            bool result = true;
            using (BinaryWriter f = new BinaryWriter(File.OpenWrite(filename)))
            {
                result = result &&
                    saveShorts(ResHist, f) &&
                    saveShorts(ComHist, f) &&
                    saveShorts(IndHist, f) &&
                    saveShorts(CrimeHist, f) &&
                    saveShorts(PollutionHist, f) &&
                    saveShorts(MoneyHist, f) &&
                    saveShorts(MiscHist, f);

                for (int x = 0; x < Constants.WorldWidth; ++x)
                {
                    for (int y = 0; y < Constants.WorldHeight; ++y)
                    {
                        result = result && saveShort((short)Map[x, y], f);
                    }
                }

            }

            return result;
        }

        public void LoadScenario(Scenario s) {
            string name = null;
            string fname = null;

            CityFileName = "";

            SetGameLevel(Levels.Easy);

            if (s < Scenario.Dullsville || s > Scenario.Rio)
            {
                s = Scenario.Dullsville;
            }

            switch (s)
            {
                case Scenario.Dullsville:
                    name = "Dullsville";
                    fname = "snro.111";
                    Scenario = Scenario.Dullsville;
                    CityTime = ((1900 - 1900) * 48) + 2;
                    SetFunds(5000);
                    break;
                case Scenario.SanFrancisco:
                    name = "San Francisco";
                    fname = "snro.222";
                    Scenario = Scenario.SanFrancisco;
                    CityTime = ((1906 - 1900) * 48) + 2;
                    SetFunds(20000);
                    break;
                case Scenario.Hamburg:
                    name = "Hamburg";
                    fname = "snro.333";
                    Scenario = Scenario.Hamburg;
                    CityTime = ((1944 - 1900) * 48) + 2;
                    SetFunds(20000);
                    break;
                case Scenario.Bern:
                    name = "Bern";
                    fname = "snro.444";
                    Scenario = Scenario.Bern;
                    CityTime = ((1965 - 1900) * 48) + 2;
                    SetFunds(20000);
                    break;
                case Scenario.Tokyo:
                    name = "Tokyo";
                    fname = "snro.555";
                    Scenario = Scenario.Tokyo;
                    CityTime = ((1957 - 1900) * 48) + 2;
                    SetFunds(20000);
                    break;
                case Scenario.Detroit:
                    name = "Detroit";
                    fname = "snro.666";
                    Scenario = Scenario.Detroit;
                    CityTime = ((1972 - 1900) * 48) + 2;
                    SetFunds(20000);
                    break;
                case Scenario.Boston:
                    name = "Boston";
                    fname = "snro.777";
                    Scenario = Scenario.Boston;
                    CityTime = ((2010 - 1900) * 48) + 2;
                    SetFunds(20000);
                    break;
                case Scenario.Rio:
                    name = "Rio de Janeiro";
                    fname = "snro.888";
                    Scenario = Scenario.Rio;
                    CityTime = ((2047 - 1900) * 48) + 2;
                    SetFunds(20000);
                    break;
                default:
                    //NOT_REACHED();
                    break;
            }

            SetCleanCityName(name);
            SetSpeed(3);
            SetCityTax(7);

            LoadFileDir(
                fname,
                resourceDir);

            InitWillStuff();
            InitFundingLevel();
            UpdateFunds();
            InvalidateMaps();
            InitSimLoad = 1;
            DoInitialEval = false;
            DoSimInit();
            DidLoadScenario();
        }

        public void DidLoadScenario() { Callback("didLoadScenario", ""); }

        public bool LoadCity(string filename)
        {
            if (LoadFile(filename))
            {

                CityFileName = filename;

                int lastSlash = CityFileName.LastIndexOf('/');
                int pos = (lastSlash == -1) ? 0 : lastSlash + 1;

                int lastDot = CityFileName.LastIndexOf('.');
                int last =
                    (lastDot == -1) ? CityFileName.Length : lastDot;

                string newCityName = CityFileName.Substring(pos, last - pos);
                SetCityName(newCityName);

                DidLoadCity();

                return true;
            }
            else
            {
                DidntLoadCity(String.IsNullOrWhiteSpace(filename) ? "(null)" : filename);

                return false;
            }
        }

        public void DidLoadCity() { Callback("didLoadCity", ""); }

        public void DidntLoadCity(string message) { Callback("didntLoadCity", "s", message); }

        public void SaveCity()
        {
            if (CityFileName.Length > 0)
            {

                DoSaveCityAs();

            }
            else
            {
                if (SaveFile(CityFileName))
                {

                    DidSaveCity();

                }
                else
                {

                    DidntSaveCity(CityFileName);

                }
            }
        }

        public void DoSaveCityAs() { Callback("saveCityAs", ""); }
        public void DidSaveCity() { Callback("didSaveCity", ""); }
        public void DidntSaveCity(string message) { Callback("didntSaveCity", "s", message); }

        public void SaveCityAs(string filename)
        {
            CityFileName = filename;

            if (SaveFile(CityFileName))
            {

                int lastDot = CityFileName.LastIndexOf('.');
                int lastSlash = CityFileName.LastIndexOf('/');

                int pos =  (lastSlash == -1) ? 0 : lastSlash + 1;
                int last =  (lastDot == -1) ? CityFileName.Length : lastDot;
                int len = last - pos;

                string newCityName = CityFileName.Substring(pos, len);

                SetCityName(newCityName);

                DidSaveCity();

            }
            else
            {

                DidntSaveCity(CityFileName);

            }
        }
    }
}
