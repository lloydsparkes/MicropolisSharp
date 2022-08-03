using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MicropolisSharp.Types;

namespace Micropolis.Core;

public static class Logger
{
    private static bool _enabled = false;
    private static readonly Dictionary<Position, Dictionary<DateTime, string>> Messages = new();

    public static void Enable()
    {
        _enabled = true;
    }
    
    public static void LogMessage(Position p, string message)
    {
        if (!_enabled) return;
        
        if (!Messages.ContainsKey(p)) Messages.Add(p, new Dictionary<DateTime, string>());
        Messages[p].Add(DateTime.Now, message);
    }

    public static void DumpLogs()
    {
        foreach (var grouping in Messages)
        {
            var filename = $"Position_X{grouping.Key.X}_Y{grouping.Key.Y}_messages_{DateTime.Now:yyyyMMddhhmmss}.txt";
            File.WriteAllLines(filename, grouping.Value.Select(x => $"{x.Key:yyyy-MM-dd hh:mm:ss} {x.Value}"));
        }
    }
}