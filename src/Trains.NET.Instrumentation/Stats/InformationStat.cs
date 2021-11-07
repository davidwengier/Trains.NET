using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trains.NET.Instrumentation.Stats;

public class InformationStat : IStat
{
    public string Information { get; set; }
    public string? GetDescription() => this.Information;
    public bool ShouldShow() => !string.IsNullOrWhiteSpace(this.Information);
}
