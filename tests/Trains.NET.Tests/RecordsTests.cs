using System;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Drawing;
using Xunit;

namespace Trains.NET.Tests
{
    public class RecordsTests
    {
        [Theory]
        [InlineData("#123456", "aa", "#aa123456")]
        [InlineData("#ff123456", "aa", "#aa123456")]
        public void WithAlphaTests(string initial, string alpha, string expected)
        {
            var colour = new Color(initial);
            var alphaColour = colour.WithAlpha(alpha);

            var expectedColour = new Color(expected);

            Assert.Equal(expectedColour, alphaColour);
        }
    }
}
