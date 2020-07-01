using Trains.NET.Engine;
using Trains.NET.Engine.Tracks;

namespace Trains.NET.Tests
{
    internal static class TrackLayoutExtensions
    {
        public static Track GetTrackAt(this ITrackLayout trackLayout, int column, int row)
        {
            if (trackLayout.TryGet(column, row, out Track track))
            {
                return track;
            }
            return null;
        }
    }
}
