using System;
using System.IO;
using Micropolis.Core;

namespace Micropolis.Windows;
#if WINDOWS || LINUX
/// <summary>
///     The main class.
/// </summary>
public static class Program
{
    /// <summary>
    ///     The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main(string[] args)
    {
        Console.WriteLine("Cities: ");

        var cities = Directory.EnumerateFiles("cities");
        var index = 0;
        foreach (var filename in cities)
        {
            Console.WriteLine("{0}) {1}", index, filename.Replace(".cty", "").Replace("cities\\", ""));
            index++;
        }

        Console.Write("What city would you like to render 0 - {0}: ", index - 1);

        var chosen = Console.ReadLine();

        //Set a default
        var cityName = "kyoto";

        var indexToPlay = 0;
        if (int.TryParse(chosen.Trim(), out indexToPlay))
        {
            var i = 0;
            foreach (var name in cities)
            {
                if (indexToPlay == i)
                {
                    cityName = name.Replace(".cty", "").Replace("cities\\", "");
                    break;
                }

                i++;
            }
        }

        using (var game = new Micropolis(cityName))
            //using(var game = new Micropolis.Basic.MicropolisMapDrawer())
        {
            game.Run();
        }

        Logger.DumpLogs();
    }
}
#endif