using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Trains.NET.Engine;
using Xunit;
using Xunit.Abstractions;

namespace Trains.NET.Tests.EngineUtilityTests;

public class GameThreadTimerTests
{
    private readonly ITestOutputHelper _output;

    public GameThreadTimerTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task GameThreadTimer_DisposeWorks()
    {
        const int TestInterval = 20;
        bool run = false;
        using (ITimer gameTimer = new GameThreadTimer())
        {
            gameTimer.Interval = TestInterval;
            gameTimer.Elapsed += (sender, e) => run = true;
            gameTimer.Start();
            await Task.Delay(TestInterval * 2).ConfigureAwait(false);
        }
        Assert.True(run);
    }

    [Theory]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(20)]
    public async Task GameThreadTimer_IntervalIsMet_MinimalWorkload(int testInterval)
    {
        const double IntervalDiffMillisecondThreshold = 2.0;

        (bool enoughSamples, double avgInterval) = await CollectAverageInterval(testInterval, sw => { }).ConfigureAwait(false);

        Assert.True(enoughSamples, "Didn't collect enough samples to average");
        Assert.True(Math.Abs(testInterval - avgInterval) < IntervalDiffMillisecondThreshold, $"Measured interval {testInterval} was lower than threshold {IntervalDiffMillisecondThreshold}");
    }

    [Theory]
    [InlineData(8, 2)]
    [InlineData(8, 4, Skip = "Flaky in CI")]
    [InlineData(8, 8)]
    [InlineData(16, 2)]
    [InlineData(16, 4)]
    [InlineData(16, 8)]
    [InlineData(16, 16)]
    [InlineData(20, 4)]
    [InlineData(20, 8)]
    public async Task GameThreadTimer_IntervalIsMet_FakeSleepWorkload(int testInterval, int sleepWorkloadMS)
    {
        const double IntervalDiffMillisecondThreshold = 2.0;

        (bool enoughSamples, double avgInterval) = await CollectAverageInterval(testInterval, sw =>
        {
            long target = sw.ElapsedMilliseconds + sleepWorkloadMS;
            while (sw.ElapsedMilliseconds < target) ;
        }).ConfigureAwait(false);

        Assert.True(enoughSamples, "Didn't collect enough samples to average");
        Assert.True(Math.Abs(testInterval - avgInterval) < IntervalDiffMillisecondThreshold, $"Measured interval {testInterval} was lower than threshold {IntervalDiffMillisecondThreshold}");
    }

    private async Task<(bool EnoughSamples, double AvgInterval)> CollectAverageInterval(int interval, Action<Stopwatch> work)
    {
        const int TargetSampleSize = 175;
        const int AvgPercentile = 95;

        var times = new List<long>();
        var testStopwatch = Stopwatch.StartNew();
        using (ITimer gameTimer = new GameThreadTimer())
        {
            gameTimer.Interval = interval;
            gameTimer.Elapsed += (sender, e) =>
            {
                work(testStopwatch);
                times.Add(testStopwatch.ElapsedMilliseconds);
            };
            gameTimer.Start();
            await Task.Delay(interval * TargetSampleSize).ConfigureAwait(false);
        }

        if (times.Count < 2)
        {
            return (false, 0);
        }

        var intervals = new List<long>();

        for (int i = 0; i < times.Count - 1; i++)
        {
            intervals.Add(times[i + 1] - times[i]);
        }

        int intervalCount = (int)(intervals.Count * AvgPercentile * 0.01);
        var intervalCutList = intervals.OrderBy(x => x).Take(intervalCount).ToList();
        double intervalAvg = intervalCutList.Average();

        _output.WriteLine($"Raw interval (count, min, avg, max): ({intervals.Count}, {intervals.Min()}, {intervals.Average()}, {intervals.Max()})");
        _output.WriteLine($"{AvgPercentile} percentile (count, min, avg, max): ({intervalCutList.Count}, {intervalCutList.Min()}, {intervalAvg}, {intervalCutList.Max()})");

        return (true, intervalAvg);
    }
}
