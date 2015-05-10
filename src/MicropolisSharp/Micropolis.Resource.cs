using MicropolisSharp.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Resource.cpp
/// </summary>

namespace MicropolisSharp
{
    public partial class Micropolis
    {
        protected string homeDir;
        protected string resourceDir;

        public Resource Resources { get; private set; }
        public StringTable StringTables { get; private set; }

        public Resource GetResource(string name, long id)
        {
            Resource r = Resources;
            while(r != null)
            {
                if(r.Id == id && r.Name == name)
                {
                    return r;
                }
                r = r.Next;
            }

            r = new Resource();
            r.Name = name;
            r.Id = id;

            String filename = String.Format("{0}{1}{2}{3}", resourceDir, Path.DirectorySeparatorChar, r.Name, r.Id);

            if (!File.Exists(filename))
            {
                return null;
            }

            using(BinaryReader reader = new BinaryReader(File.OpenRead(filename)))
            {
                FileInfo fileInfo = new FileInfo(filename);

                r.Data = reader.ReadBytes((int)fileInfo.Length);
                r.Size = (int)fileInfo.Length;
            }

            Resource n = Resources;
            while (n != null)
            {
                if(n.Next == null)
                {
                    n.Next = r;
                    return r;
                }
            }
            return r;
        }

        public String GetIndString(long id, short num)
        {
            StringTable tp = StringTables;
            StringTable st = null;

            while (tp != null)
            {
                if(tp.Id == id)
                {
                    st = tp;
                }
                tp = tp.Next;
            }

            if(st == null)
            {
                st = new StringTable();
                st.Id = id;

                Resource r = GetResource("stri", id);
                long size = r.Size;
                byte[] buf = r.Data;

                List<String> strings = new List<string>();

                string tempLine = "";
                for(long i = 0; i < size; i++)
                {                    
                    if((char)buf[i] == '\n')
                    {
                        strings.Add(tempLine);
                    }
                    else
                    {
                        tempLine += (char)buf[i];
                    }
                }

                st.Lines = strings.Count;
                st.Strings = strings.ToArray();

                tp = StringTables;
                while (tp != null)
                {
                    if(tp.Next == null)
                    {
                        tp.Next = st;
                    }
                }
            }

            return st.Strings[num - 1];
        }
    }
}
