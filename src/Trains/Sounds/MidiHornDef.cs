using System;

namespace Trains.Sounds;

internal sealed record MidiHornDef
{
    // Taken from https://www.youtube.com/watch?v=yLBinl2sQp8

    //Leslie S3L - C D# A
    public static readonly MidiHornDef LeslieS3L = new(MidiNotes.C, MidiNotes.DSharp, MidiNotes.A);

    //Leslie S5T - C D# F# A C#   60, 63, 66, 69, 73
    public static readonly MidiHornDef LeslieS5T = new(MidiNotes.C, MidiNotes.DSharp, MidiNotes.FSharp, MidiNotes.A, MidiNotes.CSharp);

    //Nathan P3 - C# E A
    public static readonly MidiHornDef NathanP3 = new(MidiNotes.CSharp, MidiNotes.E, MidiNotes.A);

    //Nathan K3LA/K3H - D# F# B
    public static readonly MidiHornDef NathanK3LA = new(MidiNotes.DSharp, MidiNotes.FSharp, MidiNotes.B);

    //Nathan M3H - D# F# A#
    public static readonly MidiHornDef NathanM3H = new(MidiNotes.DSharp, MidiNotes.FSharp, MidiNotes.ASharp);

    //Nathan K5H - 
    // Option 1 - D# F# A# C D# 
    public static readonly MidiHornDef NathanK5H1 = new(MidiNotes.DSharp, MidiNotes.FSharp, MidiNotes.ASharp, MidiNotes.C, MidiNotes.DSharp);
    // Option 2 - E G B C# E
    public static readonly MidiHornDef NathanK5H2 = new(MidiNotes.E, MidiNotes.G, MidiNotes.B, MidiNotes.CSharp, MidiNotes.E);

    public MidiNotes[] Notes { get; }

    private MidiHornDef(params MidiNotes[] notes)
    {
        this.Notes = notes;
    }

    internal static MidiHornDef GetHorn(HornModel horn) => horn switch
    {
        HornModel.LeslieS3L => MidiHornDef.LeslieS3L,
        HornModel.LeslieS5T => MidiHornDef.LeslieS5T,
        HornModel.NathanK3LA => MidiHornDef.NathanK3LA,
        HornModel.NathanK5H1 => MidiHornDef.NathanK5H1,
        HornModel.NathanK5H2 => MidiHornDef.NathanK5H2,
        HornModel.NathanM3H => MidiHornDef.NathanM3H,
        HornModel.NathanP3 => MidiHornDef.NathanP3,
        _ => throw new NotSupportedException($"Unsupported horn model: {horn}"),
    };
}
