using Trains.NET.Engine;

namespace Trains.NET.Tests
{
    internal static class IStaticEntityCollectionExtensions
    {
        public static void AddTrack(this ILayout col, int column, int row)
        {
            var track = new Track()
            {
                Column = column,
                Row = row
            };
            col.Add(column, row, track);
        }
        public static Track GetTrackAt(this ILayout trackLayout, int column, int row)
        {
            trackLayout.TryGet(column, row, out Track track);
            return track;
        }
    }
}
