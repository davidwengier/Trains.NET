using System.Collections.Generic;

namespace Trains.NET.Engine;

public static class DictionaryExtensions
{
    public static void Deconstruct<T>(this KeyValuePair<(int, int), T> keyValuePair, out int col, out int row, out T value)
    {
        col = keyValuePair.Key.Item1;
        row = keyValuePair.Key.Item2;
        value = keyValuePair.Value;
    }
}
