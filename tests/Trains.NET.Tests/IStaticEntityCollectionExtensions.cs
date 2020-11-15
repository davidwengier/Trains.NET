using Trains.NET.Engine;

namespace Trains.NET.Tests
{
    internal static class IStaticEntityCollectionExtensions
    {
        public static void AddTrack(this ILayout col, int column, int row)
        {
            var track = new SingleTrack()
            {
                Column = column,
                Row = row
            };
            col.Add(column, row, track);
        }

        public static void AddTrack(this ILayout col, int column, int row, SingleTrackDirection direction)
        {
            var track = new SingleTrack()
            {
                Column = column,
                Row = row,
                Direction = direction
            };
            col.Add(column, row, track);
        }

        public static T GetTrackAt<T>(this ILayout trackLayout, int column, int row) where T : Track
        {
            trackLayout.TryGet(column, row, out T track);
            return track;
        }
    }
}
