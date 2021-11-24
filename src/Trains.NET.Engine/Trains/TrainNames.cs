namespace Trains.NET.Engine;

internal static class TrainNames
{
    public static string GetName(int seed) => s_names[Math.Abs(seed) % s_names.Length];

    private static readonly string[] s_names = new string[]{
            "The Flying Scott",
            "The Falling Scott",
            "The Boolean Boiler",
            "The Super Steamer",
            "The Tainted Teabag",
            "The Transcontinental Tuple",
            "The Great Curtislusmore",
            "The Evening Express",
            "The Majestic Steve",
            "The Awkward Scotsman",
            "The Comet Caboose",
            "The Tolkien Express",
            "The Dirty Nullable",
            "The Diagnostics Session",
            "The Mountain Mort",
            "Blue Train, South Africa",
            "The Little Train That Could",
            "The Little Sprite That Could",
            "The Howling Phantom",
            "Trainy McTrainFace"
        };
}
