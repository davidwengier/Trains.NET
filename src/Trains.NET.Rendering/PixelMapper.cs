using Trains.NET.Engine;

namespace Trains.NET.Rendering;

public class PixelMapper : IPixelMapper, IInitializeAsync, IGameState
{
    private int _columns;
    private int _rows;
    private bool _firstViewPortAdjustment = true;
    private int? _initialViewPortX;
    private int? _initialViewPortY;

    public int Columns => _columns;
    public int Rows => _rows;

    public int MaxGridWidth => _columns * CellSize;
    public int MaxGridHeight => _rows * CellSize;

    public float GameScale { get; private set; } = 1.0f;
    public int ViewPortX { get; private set; }
    public int ViewPortY { get; private set; }
    public int ViewPortWidth { get; private set; }
    public int ViewPortHeight { get; private set; }

    public int CellSize => (int)(40 * GameScale);

    public event EventHandler? ViewPortChanged;

    public Task InitializeAsync(int columns, int rows)
    {
        _columns = columns;
        _rows = rows;

        return Task.CompletedTask;
    }

    public void SetViewPortSize(int width, int height)
    {
        ViewPortWidth = width;
        ViewPortHeight = height;

        if (_firstViewPortAdjustment && _initialViewPortX.HasValue && _initialViewPortY.HasValue)
        {
            SetViewPort(_initialViewPortX.Value, _initialViewPortY.Value);
        }
        else if (_firstViewPortAdjustment)
        {
            SetViewPort((MaxGridWidth - width) / 2, (MaxGridHeight - height) / 2);
        }
        else
        {
            AdjustViewPort(0, 0);
        }

        _firstViewPortAdjustment = false;
    }

    public void SetViewPort(int x, int y)
    {
        var oldX = ViewPortX;
        var oldY = ViewPortY;
        ViewPortX = Math.Max(Math.Min(-x, 0), -1 * (MaxGridWidth - ViewPortWidth));
        ViewPortY = Math.Max(Math.Min(-y, 0), -1 * (MaxGridHeight - ViewPortHeight));

        if (ViewPortX != oldX || ViewPortY != oldY)
        {
            ViewPortChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void AdjustViewPort(int x, int y)
    {
        SetViewPort(-1 * (ViewPortX + x), -1 * (ViewPortY + y));
    }

    public (int, int) ViewPortPixelsToCoords(int x, int y)
    {
        return ((x - ViewPortX) / CellSize, (y - ViewPortY) / CellSize);
    }

    public (int, int, bool) CoordsToViewPortPixels(int column, int row)
    {
        var x = (column * CellSize) + ViewPortX;
        var y = (row * CellSize) + ViewPortY;
        var onScreen = x > -CellSize &&
                        y > -CellSize &&
                        x <= ViewPortWidth &&
                        y <= ViewPortHeight;
        return (x, y, onScreen);
    }

    public (int, int) WorldPixelsToCoords(int x, int y)
    {
        return (x / CellSize, y / CellSize);
    }

    public (int, int) CoordsToWorldPixels(int column, int row)
    {
        return (column * CellSize, row * CellSize);
    }

    public IPixelMapper Snapshot()
    {
        return new PixelMapper()
        {
            _columns = _columns,
            _rows = _rows,
            ViewPortX = ViewPortX,
            ViewPortY = ViewPortY,
            ViewPortHeight = ViewPortHeight,
            ViewPortWidth = ViewPortWidth,
            GameScale = GameScale
        };
    }

    private (float, float) GetScaledViewPortSize()
        => GetScaledViewPortSize(GameScale);

    private (float, float) GetScaledViewPortSize(float scale)
        => (ViewPortWidth / scale,
            ViewPortHeight / scale);

    public bool AdjustGameScale(float delta)
    {
        var newGameScale = GameScale * delta;

        // Check to see if it is TOO FAR!
        if (newGameScale < 0.1 ||
            MaxGridWidth / GameScale * newGameScale < ViewPortWidth ||
            MaxGridHeight / GameScale * newGameScale < ViewPortHeight)
        {
            return false;
        }
        else if (newGameScale > 5)
        {
            newGameScale = 5.0f;
        }

        if (GameScale == newGameScale)
        {
            return false;
        }

        // Viewport X & Y will be negative, as they are canvas transations, so swap em!
        var currentX = -ViewPortX / GameScale;
        var currentY = -ViewPortY / GameScale;

        (var svpWidth, var svpHeight) = GetScaledViewPortSize();

        var currentCenterX = currentX + svpWidth / 2.0f;
        var currentCenterY = currentY + svpHeight / 2.0f;

        (var newSvpWidth, var newSvpHeight) = GetScaledViewPortSize(newGameScale);

        var newX = currentCenterX - newSvpWidth / 2.0f;
        var newY = currentCenterY - newSvpHeight / 2.0f;

        ViewPortX = -(int)Math.Round(newX * newGameScale);
        ViewPortY = -(int)Math.Round(newY * newGameScale);

        GameScale = newGameScale;

        AdjustViewPort(0, 0);

        ViewPortChanged?.Invoke(this, EventArgs.Empty);

        return true;
    }

    public bool Load(IGameStorage storage)
    {
        // We always return true from this method
        // because if the saved viewport isn't valid it's not worth
        // resetting the users game over.

        var data = storage.Read(nameof(PixelMapper));

        if (data is null)
            return true;

        var bits = data.Split(",");
        if (bits.Length != 3)
            return true;

        if (!int.TryParse(bits[0], out var x) ||
            !int.TryParse(bits[1], out var y) ||
            !float.TryParse(bits[2], out var gameScale))
        {
            return true;
        }

        _initialViewPortX = -x;
        _initialViewPortY = -y;
        GameScale = gameScale;

        return true;
    }

    public void Save(IGameStorage storage)
    {
        storage.Write(nameof(PixelMapper), $"{ViewPortX},{ViewPortY},{GameScale}");
    }

    public void Reset()
    {
    }
}
