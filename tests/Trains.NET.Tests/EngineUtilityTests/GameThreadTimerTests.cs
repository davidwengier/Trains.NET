using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Trains.NET.Engine;
using Xunit;

namespace Trains.NET.Tests.EngineUtilityTests
{
    public class GameThreadTimerTests
    {
        [Fact]
        public async Task GameThreadTimer_DisposeWorks()
        {
            const int TestInterval = 20;
            bool run = false;
            using(ITimer gameTimer = new GameThreadTimer())
            {
                gameTimer.Interval = TestInterval;
                gameTimer.Elapsed += (sender, e) => run = true;
                gameTimer.Start();
                await Task.Delay(TestInterval * 2);
            }
            Assert.True(run);
        }

        [Theory]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(24)]
        public async Task GameThreadTimer_IntervalIsMet_MinimalWorkload(int testInterval)
        {
            const int TargetSampleSize = 10;
            const double IntervalDiffMilliseoncdThreshold = 2.0;

            var times = new List<long>();
            var testStopwatch = Stopwatch.StartNew();
            using (ITimer gameTimer = new GameThreadTimer())
            {
                gameTimer.Interval = testInterval;
                gameTimer.Elapsed += (sender, e) => times.Add(testStopwatch.ElapsedMilliseconds);
                gameTimer.Start();
                await Task.Delay(testInterval * TargetSampleSize);
            }

            double sumInterval = 0;
            int countInterval = 0;

            for(int i = 0; i < times.Count - 1; i++)
            {
                sumInterval += times[i + 1] - times[i];
                countInterval++;
            }

            Assert.True(countInterval > 0, $"Didn't collect enough samples to average, countInterval: {countInterval}");

            double intervalDiff = Math.Abs(testInterval - sumInterval / countInterval);

            Assert.True(intervalDiff < IntervalDiffMilliseoncdThreshold, $"Measured interval {countInterval} was lower than threshold {IntervalDiffMilliseoncdThreshold}");
        }

        [Theory]
        [InlineData(8, 2)]
        [InlineData(8, 4)]
        [InlineData(8, 8)]
        [InlineData(16, 2)]
        [InlineData(16, 4)]
        [InlineData(16, 8)]
        [InlineData(16, 16)]
        [InlineData(24, 2)]
        [InlineData(24, 4)]
        [InlineData(24, 8)]
        [InlineData(24, 16)]
        [InlineData(24, 24)]
        public async Task GameThreadTimer_IntervalIsMet_FakeSleepWorkload(int testInterval, int sleepWorkloadMS)
        {
            const int TargetSampleSize = 10;
            const double IntervalDiffMilliseoncdThreshold = 2.0;

            var times = new List<long>();
            var testStopwatch = Stopwatch.StartNew();
            using (ITimer gameTimer = new GameThreadTimer())
            {
                gameTimer.Interval = testInterval;
                gameTimer.Elapsed += (sender, e) =>
                {
                    long target = testStopwatch.ElapsedMilliseconds + sleepWorkloadMS;
                    while (testStopwatch.ElapsedMilliseconds < target) ;
                    times.Add(testStopwatch.ElapsedMilliseconds);
                };
                gameTimer.Start();
                await Task.Delay(testInterval * TargetSampleSize);
            }

            double sumInterval = 0;
            int countInterval = 0;

            for (int i = 0; i < times.Count - 1; i++)
            {
                sumInterval += times[i + 1] - times[i];
                countInterval++;
            }

            Assert.True(countInterval > 0, $"Didn't collect enough samples to average, countInterval: {countInterval}");

            double intervalDiff = Math.Abs(testInterval - sumInterval / countInterval);

            Assert.True(intervalDiff < IntervalDiffMilliseoncdThreshold, $"Measured interval {countInterval} was lower than threshold {IntervalDiffMilliseoncdThreshold}");
        }
    }
}
