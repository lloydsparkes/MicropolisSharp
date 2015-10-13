using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Micropolis.Windows
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Cities: ");

            IEnumerable<string> cities = Directory.EnumerateFiles("cities");
            int index = 0;
            foreach (string filename in cities)
            {
                Console.WriteLine("{0}) {1}", index, filename.Replace(".cty", "").Replace("cities\\", ""));
                index++;
            }

            Console.Write("What city would you like to render 0 - {0}: ", index - 1);

            string chosen = Console.ReadLine();
           
            //Set a default
            string cityName = "kyoto";

            int indexToPlay = 0;
            if(int.TryParse(chosen.Trim(), out indexToPlay))
            {
                int i = 0;
                foreach(string name in cities)
                {
                    if(indexToPlay == i)
                    {
                        cityName = name.Replace(".cty", "").Replace("cities\\", "");
                        break;
                    }
                    i++;
                }
            }

            using (var game = new Micropolis.Basic.Micropolis(cityName))
            //using(var game = new Micropolis.Basic.MicropolisMapDrawer())
                game.Run();
        }
    }
#endif
}
