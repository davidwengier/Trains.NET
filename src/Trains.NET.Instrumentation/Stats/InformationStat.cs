namespace Trains.NET.Instrumentation.Stats;

public class InformationStat : IStat
{
    public string Information { get; set; } = "";
    public string GetDescription() => Information;
    public bool ShouldShow() => !string.IsNullOrWhiteSpace(Information);
}
