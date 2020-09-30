using System;
using Trains.NET.Rendering;
using Xunit;
using Xunit.Abstractions;

namespace Trains.NET.Tests.Rendering.PixelMapperTests
{
    public class SnapshotTests
    {
        [Fact]
        public void Snapshot_PostSnapshot_SnapshotShoudntChange()
        {
            int width = 200;
            int height = 200;
            int viewportX = 30;
            int viewportY = 30;
            float gameScale = 2.0f;

            IPixelMapper pixelMapper = new PixelMapper();

            pixelMapper.SetViewPortSize(width, height);
            pixelMapper.AdjustGameScale(gameScale);
            pixelMapper.SetViewPort(viewportX, viewportY);

            IPixelMapper actual = pixelMapper.Snapshot();

            pixelMapper.AdjustGameScale(0.5f);
            pixelMapper.SetViewPort(0, 0);

            Assert.Equal(-viewportX, actual.ViewPortX);
            Assert.Equal(-viewportY, actual.ViewPortY);
            Assert.Equal(gameScale, actual.GameScale);

            Assert.Equal(width, actual.ViewPortWidth);
            Assert.Equal(height, actual.ViewPortHeight);
        }

        [Fact]
        public void Snapshot_FullySetup()
        {
            int width = 200;
            int height = 200;
            int viewportX = 30;
            int viewportY = 30;

            IPixelMapper pixelMapper = new PixelMapper();

            pixelMapper.SetViewPortSize(width, height);
            pixelMapper.SetViewPort(viewportX, viewportY);
            pixelMapper.AdjustGameScale(2f);

            IPixelMapper actual = pixelMapper.Snapshot();

            AssertSnapshotsSame(pixelMapper, actual);
        }

        [Fact]
        public void Snapshot_NoChanges()
        {
            IPixelMapper pixelMapper = new PixelMapper();

            IPixelMapper actual = pixelMapper.Snapshot();

            AssertSnapshotsSame(pixelMapper, actual);
        }

        [Fact]
        public void Snapshot_CompareTwoSnapshotsFromSameMapper()
        {
            IPixelMapper pixelMapper = new PixelMapper();

            IPixelMapper expected = pixelMapper.Snapshot();
            IPixelMapper actual = pixelMapper.Snapshot();

            AssertSnapshotsSame(expected, actual);
        }

        private static void AssertSnapshotsSame(IPixelMapper expected, IPixelMapper actual)
        {
            Assert.Equal(expected.CellSize, actual.CellSize);
            Assert.Equal(expected.GameScale, actual.GameScale);
            Assert.Equal(expected.MaxGridWidth, actual.MaxGridWidth);
            Assert.Equal(expected.MaxGridHeight, actual.MaxGridHeight);
            Assert.Equal(expected.ViewPortHeight, actual.ViewPortHeight);
            Assert.Equal(expected.ViewPortWidth, actual.ViewPortWidth);
            Assert.Equal(expected.ViewPortX, actual.ViewPortX);
            Assert.Equal(expected.ViewPortY, actual.ViewPortY);
        }
    }

    public class CoordPixelConverstionTests
    {
        private const int DefaultCellSize = 40;
        private const int DoubleDefaultCellSize = DefaultCellSize * 2;
        private const int HalfDefaultCellSize = DefaultCellSize / 2;
        // This screen size is used as part of the data below, be careful if you change it.
        private const int ScreenSize = 200;
        private readonly ITestOutputHelper _output;
        private readonly IPixelMapper _pixelMapper;

        public CoordPixelConverstionTests(ITestOutputHelper output)
        {
            _output = output;

            _pixelMapper = new PixelMapper();
            _pixelMapper.SetViewPortSize(ScreenSize, ScreenSize);
            _pixelMapper.LogData(output);

            if (DefaultCellSize != _pixelMapper.CellSize)
            {
                throw new Exception("Cell size is different than this test expects, these tests assume the DefaultCellSize is " + DefaultCellSize);
            }
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(DefaultCellSize, 0, 1, 0)]
        [InlineData(0, DefaultCellSize, 0, 1)]
        [InlineData(DefaultCellSize, DefaultCellSize, 1, 1)]
        [InlineData(5 * DefaultCellSize, 5 * DefaultCellSize, 5, 5)]
        public void ViewPortPixelsToCoords_DefaultScale_DefaultPosition(int x, int y, int expectedCol, int expectedRow)
        {
            (int actualCol, int actualRow) = _pixelMapper.ViewPortPixelsToCoords(x, y);

            Assert.Equal(expectedCol, actualCol);
            Assert.Equal(expectedRow, actualRow);
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(DoubleDefaultCellSize, 0, 1, 0)]
        [InlineData(0, DoubleDefaultCellSize, 0, 1)]
        [InlineData(DoubleDefaultCellSize, DoubleDefaultCellSize, 1, 1)]
        [InlineData(5 * DoubleDefaultCellSize, 5 * DoubleDefaultCellSize, 5, 5)]
        public void ViewPortPixelsToCoords_ZoomedIn_ZeroPosition(int x, int y, int expectedCol, int expectedRow)
        {
            _pixelMapper.AdjustGameScale(2.0f);
            _pixelMapper.SetViewPort(0, 0);
            _pixelMapper.LogData(_output);

            (int actualCol, int actualRow) = _pixelMapper.ViewPortPixelsToCoords(x, y);

            Assert.Equal(expectedCol, actualCol);
            Assert.Equal(expectedRow, actualRow);
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(HalfDefaultCellSize, 0, 1, 0)]
        [InlineData(0, HalfDefaultCellSize, 0, 1)]
        [InlineData(HalfDefaultCellSize, HalfDefaultCellSize, 1, 1)]
        [InlineData(5 * HalfDefaultCellSize, 5 * HalfDefaultCellSize, 5, 5)]
        public void ViewPortPixelsToCoords_ZoomedOut_ZeroPosition(int x, int y, int expectedCol, int expectedRow)
        {
            _pixelMapper.AdjustGameScale(0.5f);
            _pixelMapper.SetViewPort(0, 0);
            _pixelMapper.LogData(_output);

            (int actualCol, int actualRow) = _pixelMapper.ViewPortPixelsToCoords(x, y);

            Assert.Equal(expectedCol, actualCol);
            Assert.Equal(expectedRow, actualRow);
        }

        [Fact]
        public void ViewPortCoordsPixelsCoords_ChangingZoom()
        {
            int col = 0;
            int row = 0;

            (int x, int y, _) = _pixelMapper.CoordsToViewPortPixels(col, row);

            _pixelMapper.AdjustGameScale(2.0f);
            _pixelMapper.SetViewPort(0, 0);
            _pixelMapper.LogData(_output);

            (int actualCol, int actualRow) = _pixelMapper.ViewPortPixelsToCoords(x, y);

            Assert.Equal(col, actualCol);
            Assert.Equal(row, actualRow);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        [InlineData(0, -1)]
        [InlineData(-1, 0)]
        [InlineData(-1, -1)]
        [InlineData(0, 6)]
        [InlineData(6, 0)]
        [InlineData(6, 6)]
        public void ViewPortCoordsPixelsCoords_DefaultScale_DefaultPosition(int col, int row)
        {
            (int x, int y, _) = _pixelMapper.CoordsToViewPortPixels(col, row);

            (int actualCol, int actualRow) = _pixelMapper.ViewPortPixelsToCoords(x, y);

            Assert.Equal(col, actualCol);
            Assert.Equal(row, actualRow);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        [InlineData(0, -1)]
        [InlineData(-1, 0)]
        [InlineData(-1, -1)]
        [InlineData(0, 6)]
        [InlineData(6, 0)]
        [InlineData(6, 6)]
        public void ViewPortCoordsPixelsCoords_ZoomedIn_DefaultPosition(int col, int row)
        {
            _pixelMapper.AdjustGameScale(2.0f);
            _pixelMapper.SetViewPort(0, 0);
            _pixelMapper.LogData(_output);

            (int x, int y, _) = _pixelMapper.CoordsToViewPortPixels(col, row);

            (int actualCol, int actualRow) = _pixelMapper.ViewPortPixelsToCoords(x, y);

            Assert.Equal(col, actualCol);
            Assert.Equal(row, actualRow);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        [InlineData(0, -1)]
        [InlineData(-1, 0)]
        [InlineData(-1, -1)]
        [InlineData(0, 6)]
        [InlineData(6, 0)]
        [InlineData(6, 6)]
        public void ViewPortCoordsPixelsCoords_ZoomedOut_DefaultPosition(int col, int row)
        {
            _pixelMapper.AdjustGameScale(0.5f);
            _pixelMapper.SetViewPort(0, 0);
            _pixelMapper.LogData(_output);

            (int x, int y, _) = _pixelMapper.CoordsToViewPortPixels(col, row);

            (int actualCol, int actualRow) = _pixelMapper.ViewPortPixelsToCoords(x, y);

            Assert.Equal(col, actualCol);
            Assert.Equal(row, actualRow);
        }


        [Theory]
        [InlineData(0, -1)]
        [InlineData(-1, 0)]
        [InlineData(-1, -1)]
        [InlineData(0, 6)]
        [InlineData(6, 0)]
        [InlineData(6, 6)]
        public void CoordsToViewPortPixels_OffScreen_DefaultScale_DefaultPosition(int col, int row)
        {
            (_, _, bool onScreen) = _pixelMapper.CoordsToViewPortPixels(col, row);

            Assert.False(onScreen);
        }

        [Theory]
        [InlineData(0, 0, DefaultCellSize, DefaultCellSize)]
        [InlineData(0, 0, DefaultCellSize * 2, DefaultCellSize * 2)]
        [InlineData(50, 50, 0, 0)]
        [InlineData(50, 50, DefaultCellSize * 2, DefaultCellSize * 2)]
        public void CoordsToViewPortPixels_OffScreen_DefaultScale_ShiftedViewport(int col, int row, int viewportX, int viewportY)
        {
            _pixelMapper.SetViewPort(viewportX, viewportY);
            _pixelMapper.LogData(_output);

            (_, _, bool onScreen) = _pixelMapper.CoordsToViewPortPixels(col, row);

            Assert.False(onScreen);
        }

        [Theory]
        [InlineData(0, -1)]
        [InlineData(-1, 0)]
        [InlineData(-1, -1)]
        [InlineData(0, 4)]
        [InlineData(4, 0)]
        [InlineData(4, 4)]
        public void CoordsToViewPortPixels_OffScreen_ZoomedIn_DefaultPosition(int col, int row)
        {
            _pixelMapper.AdjustGameScale(2.0f);
            _pixelMapper.LogData(_output);

            (_, _, bool onScreen) = _pixelMapper.CoordsToViewPortPixels(col, row);

            Assert.False(onScreen);
        }

        [Theory]
        [InlineData(-1, -1, 0, 0)]
        [InlineData(0, 0, DoubleDefaultCellSize, DoubleDefaultCellSize)]
        [InlineData(50, 50, 0, 0)]
        [InlineData(50, 50, DoubleDefaultCellSize * 2, DoubleDefaultCellSize * 2)]
        public void CoordsToViewPortPixels_OffScreen_ZoomedIn_ShiftedViewport(int col, int row, int viewportX, int viewportY)
        {
            _pixelMapper.AdjustGameScale(2.0f);
            _pixelMapper.SetViewPort(viewportX, viewportY);
            _pixelMapper.LogData(_output);

            (_, _, bool onScreen) = _pixelMapper.CoordsToViewPortPixels(col, row);

            Assert.False(onScreen);
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 1, 0, 0)]
        [InlineData(0, 4, 0, 0)]
        [InlineData(4, 0, 0, 0)]
        [InlineData(4, 4, 0, 0)]

        [InlineData(1, 1, DefaultCellSize, DefaultCellSize)]
        [InlineData(1, 5, DefaultCellSize, DefaultCellSize)]
        [InlineData(5, 1, DefaultCellSize, DefaultCellSize)]
        [InlineData(5, 5, DefaultCellSize, DefaultCellSize)]
        [InlineData(2, 2, DefaultCellSize * 2, DefaultCellSize * 2)]
        public void CoordsToViewPortPixels_OnScreen_DefaultScale(int col, int row, int viewportX, int viewportY)
        {
            _pixelMapper.SetViewPort(viewportX, viewportY);
            _pixelMapper.LogData(_output);

            (_, _, bool onScreen) = _pixelMapper.CoordsToViewPortPixels(col, row);

            Assert.True(onScreen);
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 0, DefaultCellSize, 0)]
        [InlineData(0, 1, 0, DefaultCellSize)]
        [InlineData(1, 1, DefaultCellSize, DefaultCellSize)]
        [InlineData(5, 5, 5 * DefaultCellSize, 5 * DefaultCellSize)]
        public void CoordsToViewPortPixels_DefaultScale_DefaultPosition(int col, int row, int expectedX, int expectedY)
        {
            (int actualX, int actualY, _) = _pixelMapper.CoordsToViewPortPixels(col, row);

            Assert.Equal(expectedX, actualX);
            Assert.Equal(expectedY, actualY);
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 0, DoubleDefaultCellSize, 0)]
        [InlineData(0, 1, 0, DoubleDefaultCellSize)]
        [InlineData(1, 1, DoubleDefaultCellSize, DoubleDefaultCellSize)]
        [InlineData(5, 5, 5 * DoubleDefaultCellSize, 5 * DoubleDefaultCellSize)]
        public void CoordsToViewPortPixels_ZoomedIn_ZeroPosition(int col, int row, int expectedX, int expectedY)
        {
            _pixelMapper.AdjustGameScale(2.0f);
            _pixelMapper.SetViewPort(0, 0);
            _pixelMapper.LogData(_output);

            (int actualX, int actualY, _) = _pixelMapper.CoordsToViewPortPixels(col, row);

            Assert.Equal(expectedX, actualX);
            Assert.Equal(expectedY, actualY);
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 0, HalfDefaultCellSize, 0)]
        [InlineData(0, 1, 0, HalfDefaultCellSize)]
        [InlineData(1, 1, HalfDefaultCellSize, HalfDefaultCellSize)]
        [InlineData(5, 5, 5 * HalfDefaultCellSize, 5 * HalfDefaultCellSize)]
        public void CoordsToViewPortPixels_ZoomedOut_ZeroPosition(int col, int row, int expectedX, int expectedY)
        {
            _pixelMapper.AdjustGameScale(0.5f);
            _pixelMapper.SetViewPort(0, 0);
            _pixelMapper.LogData(_output);

            (int actualX, int actualY, _) = _pixelMapper.CoordsToViewPortPixels(col, row);

            Assert.Equal(expectedX, actualX);
            Assert.Equal(expectedY, actualY);
        }

        [Theory]
        [InlineData(0, 0, 0, 0, 0, 0)]
        [InlineData(0, 1, 0, DefaultCellSize, 0, 0)]
        [InlineData(1, 0, DefaultCellSize, 0, 0, 0)]
        [InlineData(1, 1, DefaultCellSize, DefaultCellSize, 0, 0)]

        [InlineData(0, 0, DefaultCellSize, 0, -DefaultCellSize, 0)]
        [InlineData(0, 0, 0, DefaultCellSize, 0, -DefaultCellSize)]
        [InlineData(0, 0, DefaultCellSize, DefaultCellSize, -DefaultCellSize, -DefaultCellSize)]

        [InlineData(2, 2, DefaultCellSize, DefaultCellSize, DefaultCellSize, DefaultCellSize)]
        public void CoordsToViewPortPixels_DefaultScale_ViewPortOffset(int col, int row, int viewPortX, int viewPortY, int expectedX, int expectedY)
        {
            _pixelMapper.SetViewPort(viewPortX, viewPortY);
            _pixelMapper.LogData(_output);

            (int actualX, int actualY, _) = _pixelMapper.CoordsToViewPortPixels(col, row);

            Assert.Equal(expectedX, actualX);
            Assert.Equal(expectedY, actualY);
        }

        [Theory]
        [InlineData(0, 0, 0, 0, 0, 0)]
        [InlineData(0, 1, 0, DoubleDefaultCellSize, 0, 0)]
        [InlineData(1, 0, DoubleDefaultCellSize, 0, 0, 0)]
        [InlineData(1, 1, DoubleDefaultCellSize, DoubleDefaultCellSize, 0, 0)]

        [InlineData(0, 0, DoubleDefaultCellSize, 0, -DoubleDefaultCellSize, 0)]
        [InlineData(0, 0, 0, DoubleDefaultCellSize, 0, -DoubleDefaultCellSize)]
        [InlineData(0, 0, DoubleDefaultCellSize, DoubleDefaultCellSize, -DoubleDefaultCellSize, -DoubleDefaultCellSize)]

        [InlineData(2, 2, DoubleDefaultCellSize, DoubleDefaultCellSize, DoubleDefaultCellSize, DoubleDefaultCellSize)]
        public void CoordsToViewPortPixels_ZoomedIn_ViewPortOffset(int col, int row, int viewPortX, int viewPortY, int expectedX, int expectedY)
        {
            _pixelMapper.AdjustGameScale(2.0f);
            _pixelMapper.SetViewPort(viewPortX, viewPortY);
            _pixelMapper.LogData(_output);

            (int actualX, int actualY, _) = _pixelMapper.CoordsToViewPortPixels(col, row);

            Assert.Equal(expectedX, actualX);
            Assert.Equal(expectedY, actualY);
        }

        [Theory]
        [InlineData(0, 0, 0, 0, 0, 0)]
        [InlineData(0, 1, 0, HalfDefaultCellSize, 0, 0)]
        [InlineData(1, 0, HalfDefaultCellSize, 0, 0, 0)]
        [InlineData(1, 1, HalfDefaultCellSize, HalfDefaultCellSize, 0, 0)]

        [InlineData(0, 0, HalfDefaultCellSize, 0, -HalfDefaultCellSize, 0)]
        [InlineData(0, 0, 0, HalfDefaultCellSize, 0, -HalfDefaultCellSize)]
        [InlineData(0, 0, HalfDefaultCellSize, HalfDefaultCellSize, -HalfDefaultCellSize, -HalfDefaultCellSize)]

        [InlineData(2, 2, HalfDefaultCellSize, HalfDefaultCellSize, HalfDefaultCellSize, HalfDefaultCellSize)]
        public void CoordsToViewPortPixels_ZoomedOut_ViewPortOffset(int col, int row, int viewPortX, int viewPortY, int expectedX, int expectedY)
        {
            _pixelMapper.AdjustGameScale(0.5f);
            _pixelMapper.SetViewPort(viewPortX, viewPortY);
            _pixelMapper.LogData(_output);

            (int actualX, int actualY, _) = _pixelMapper.CoordsToViewPortPixels(col, row);

            Assert.Equal(expectedX, actualX);
            Assert.Equal(expectedY, actualY);
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 1, 0, 0)]
        [InlineData(DefaultCellSize / 2, DefaultCellSize / 2, 0, 0)]
        [InlineData(DefaultCellSize - 1, 0, 0, 0)]
        [InlineData(0, DefaultCellSize - 1, 0, 0)]
        [InlineData(DefaultCellSize - 1, DefaultCellSize - 1, 0, 0)]

        [InlineData(DefaultCellSize, 0, 1, 0)]
        [InlineData(DefaultCellSize + DefaultCellSize / 2, DefaultCellSize / 2, 1, 0)]
        [InlineData(DefaultCellSize * 2 - 1, DefaultCellSize - 1, 1, 0)]

        [InlineData(0, DefaultCellSize, 0, 1)]
        [InlineData(DefaultCellSize / 2, DefaultCellSize + DefaultCellSize / 2, 0, 1)]
        [InlineData(DefaultCellSize - 1, DefaultCellSize * 2 - 1, 0, 1)]

        [InlineData(DefaultCellSize, DefaultCellSize, 1, 1)]
        [InlineData(5 * DefaultCellSize, 5 * DefaultCellSize, 5, 5)]
        public void WorldPixelsToCoords_DefaultScale(int x, int y, int expectedCol, int expectedRow)
        {
            (int actualCol, int actualRow) = _pixelMapper.WorldPixelsToCoords(x, y);

            Assert.Equal(expectedCol, actualCol);
            Assert.Equal(expectedRow, actualRow);
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 1, 0, 0)]
        [InlineData(DoubleDefaultCellSize / 2, DoubleDefaultCellSize / 2, 0, 0)]
        [InlineData(DoubleDefaultCellSize - 1, 0, 0, 0)]
        [InlineData(0, DoubleDefaultCellSize - 1, 0, 0)]
        [InlineData(DoubleDefaultCellSize - 1, DoubleDefaultCellSize - 1, 0, 0)]

        [InlineData(DoubleDefaultCellSize, 0, 1, 0)]
        [InlineData(DoubleDefaultCellSize + DoubleDefaultCellSize / 2, DoubleDefaultCellSize / 2, 1, 0)]
        [InlineData(DoubleDefaultCellSize * 2 - 1, DoubleDefaultCellSize - 1, 1, 0)]

        [InlineData(0, DoubleDefaultCellSize, 0, 1)]
        [InlineData(DoubleDefaultCellSize / 2, DoubleDefaultCellSize + DoubleDefaultCellSize / 2, 0, 1)]
        [InlineData(DoubleDefaultCellSize - 1, DoubleDefaultCellSize * 2 - 1, 0, 1)]

        [InlineData(DoubleDefaultCellSize, DoubleDefaultCellSize, 1, 1)]
        [InlineData(5 * DoubleDefaultCellSize, 5 * DoubleDefaultCellSize, 5, 5)]
        public void WorldPixelsToCoords_DoubleScale(int x, int y, int expectedCol, int expectedRow)
        {
            _pixelMapper.AdjustGameScale(2.0f);
            _pixelMapper.LogData(_output);

            (int actualCol, int actualRow) = _pixelMapper.WorldPixelsToCoords(x, y);

            Assert.Equal(expectedCol, actualCol);
            Assert.Equal(expectedRow, actualRow);
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 1, 0, 0)]
        [InlineData(HalfDefaultCellSize / 2, HalfDefaultCellSize / 2, 0, 0)]
        [InlineData(HalfDefaultCellSize - 1, 0, 0, 0)]
        [InlineData(0, HalfDefaultCellSize - 1, 0, 0)]
        [InlineData(HalfDefaultCellSize - 1, HalfDefaultCellSize - 1, 0, 0)]

        [InlineData(HalfDefaultCellSize, 0, 1, 0)]
        [InlineData(HalfDefaultCellSize + HalfDefaultCellSize / 2, HalfDefaultCellSize / 2, 1, 0)]
        [InlineData(HalfDefaultCellSize * 2 - 1, HalfDefaultCellSize - 1, 1, 0)]

        [InlineData(0, HalfDefaultCellSize, 0, 1)]
        [InlineData(HalfDefaultCellSize / 2, HalfDefaultCellSize + HalfDefaultCellSize / 2, 0, 1)]
        [InlineData(HalfDefaultCellSize - 1, HalfDefaultCellSize * 2 - 1, 0, 1)]

        [InlineData(HalfDefaultCellSize, HalfDefaultCellSize, 1, 1)]
        [InlineData(5 * HalfDefaultCellSize, 5 * HalfDefaultCellSize, 5, 5)]
        public void WorldPixelsToCoords_HalfScale(int x, int y, int expectedCol, int expectedRow)
        {
            _pixelMapper.AdjustGameScale(0.5f);
            _pixelMapper.LogData(_output);

            (int actualCol, int actualRow) = _pixelMapper.WorldPixelsToCoords(x, y);

            Assert.Equal(expectedCol, actualCol);
            Assert.Equal(expectedRow, actualRow);
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 0, DefaultCellSize, 0)]
        [InlineData(0, 1, 0, DefaultCellSize)]
        [InlineData(1, 1, DefaultCellSize, DefaultCellSize)]
        [InlineData(5, 5, 5 * DefaultCellSize, 5 * DefaultCellSize)]
        public void CoordsToWorldPixels_DefaultScale(int col, int row, int expectedX, int expectedY)
        {
            (int actualX, int actualY) = _pixelMapper.CoordsToWorldPixels(col, row);

            Assert.Equal(expectedX, actualX);
            Assert.Equal(expectedY, actualY);
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 0, DoubleDefaultCellSize, 0)]
        [InlineData(0, 1, 0, DoubleDefaultCellSize)]
        [InlineData(1, 1, DoubleDefaultCellSize, DoubleDefaultCellSize)]
        [InlineData(5, 5, 5 * DoubleDefaultCellSize, 5 * DoubleDefaultCellSize)]
        public void CoordsToWorldPixels_ZoomedIn(int col, int row, int expectedX, int expectedY)
        {
            _pixelMapper.AdjustGameScale(2.0f);
            _pixelMapper.LogData(_output);

            (int actualX, int actualY) = _pixelMapper.CoordsToWorldPixels(col, row);

            Assert.Equal(expectedX, actualX);
            Assert.Equal(expectedY, actualY);
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 0, HalfDefaultCellSize, 0)]
        [InlineData(0, 1, 0, HalfDefaultCellSize)]
        [InlineData(1, 1, HalfDefaultCellSize, HalfDefaultCellSize)]
        [InlineData(5, 5, 5 * HalfDefaultCellSize, 5 * HalfDefaultCellSize)]
        public void CoordsToWorldPixels_ZoomedOut(int col, int row, int expectedX, int expectedY)
        {
            _pixelMapper.AdjustGameScale(0.5f);
            _pixelMapper.LogData(_output);

            (int actualX, int actualY) = _pixelMapper.CoordsToWorldPixels(col, row);

            Assert.Equal(expectedX, actualX);
            Assert.Equal(expectedY, actualY);
        }
    }
}
