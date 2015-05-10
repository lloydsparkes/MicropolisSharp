/* Micropolis.Resource.cs
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
using MicropolisSharp.Types;
using System;
using System.Collections.Generic;
using System.IO;

namespace MicropolisSharp
{
    /// <summary>
    /// Partial Class Containing the content of resource.cpp
    /// </summary>
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
