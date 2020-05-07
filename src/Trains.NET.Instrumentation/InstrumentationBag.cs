using System.Collections.Generic;

namespace Trains.NET.Instrumentation
{
    public static class InstrumentationBag
    {
        private static readonly List<(string Name, IStat Stat)> s_stats = new List<(string Name, IStat Stat)>();
        public static IEnumerable<(string Name, IStat Stat)> Stats => s_stats.AsReadOnly();
        public static T Add<T>(string name, T stat) where T : IStat
        {
            s_stats.Add((name, stat));
            return stat;
        }
        public static T Add<T>(string name) where T : IStat, new() => Add<T>(name, new T());
    }
}
