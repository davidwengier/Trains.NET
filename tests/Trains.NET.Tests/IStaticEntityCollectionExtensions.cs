using Trains.NET.Engine;

namespace Trains.NET.Tests
{
    internal static class IStaticEntityCollectionExtensions
    {
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
