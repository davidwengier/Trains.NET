using Trains.NET.Engine;

namespace Trains.NET.Tests
{
    internal static class IStaticEntityCollectionExtensions
    {
        public static void AddTrack(this IStaticEntityCollection col, int column, int row)
        {
            var track = new Track()
            {
                Column = column,
                Row = row
            };
            col.Add(column, row, track);
        }
        public static Track GetTrackAt(this IStaticEntityCollection trackLayout, int column, int row)
        {
            if (trackLayout.TryGet(column, row, out Track track))
            {
                return track;
            }
            return null;
        }
    }
}
