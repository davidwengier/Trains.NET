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
            col.Set(column, row, track);
            track.Direction = direction;
        }

        public static void AddTIntersectionTrack(this ILayout col, int column, int row, TIntersectionDirection direction, TIntersectionStyle style)
        {
            var track = new TIntersection()
            {
                Column = column,
                Row = row,
                Direction = direction,
                Style = style,
            };
            col.Set(column, row, track);
            track.Direction = direction;
            track.Style = style;
        }

        public static void AddCrossTrack(this ILayout col, int column, int row)
        {
            var track = new CrossTrack()
            {
                Column = column,
                Row = row
            };
            col.Set(column, row, track);
        }

        public static T GetTrackAt<T>(this ILayout trackLayout, int column, int row) where T : Track
        {
            trackLayout.TryGet(column, row, out T track);
            return track;
        }
    }
}
