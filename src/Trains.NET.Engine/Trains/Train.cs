using System.ComponentModel;

namespace Trains.NET.Engine;

public class Train : IMovable, INotifyPropertyChanged, ISeeded
{
    // only used for tests??
    internal const float SpeedScaleModifier = 0.005f;

    private const int MaxSpawnCarriages = 6;
    private const int MaximumSpeed = 200;
    private const float MinimumLookaheadSpeed = 5.0f;
    private const float DefaultSpeed = 20.0f;
    private float? _lookaheadOverride;
    private bool _collisionAhead;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Train(int seed)
    {
        Seed = seed;
        UniqueID = Guid.NewGuid();
        Name = TrainNames.GetName(seed);
        Carriages = Math.Abs(seed) % MaxSpawnCarriages;
        DesiredSpeed = DefaultSpeed;
        RelativeLeft = 0.5f;
        RelativeTop = 0.5f;
    }

    private Train(Train other)
    {
        Seed = other.Seed;
        UniqueID = other.UniqueID;
        Column = other.Column;
        Name = other.Name;
        Row = other.Row;
        Angle = other.Angle;
        RelativeLeft = other.RelativeLeft;
        RelativeTop = other.RelativeTop;
        CurrentSpeed = other.CurrentSpeed;
        DesiredSpeed = other.DesiredSpeed;
        Carriages = other.Carriages;
        _lookaheadOverride = other._lookaheadOverride;
    }

    public float LookaheadDistance
    {
        get
        {
            return _lookaheadOverride ?? Math.Max(MinimumLookaheadSpeed, CurrentSpeed) * 30;
        }
        set
        {
            _lookaheadOverride = value;
        }
    }

    public virtual Guid UniqueID { get; }

    public int Column { get; set; }
    public int Row { get; set; }
    public float Angle { get; set; }
    public float RelativeLeft { get; set; }
    public float RelativeTop { get; set; }

    public string Name { get; set; }
    public int Seed { get; }
    public virtual float CurrentSpeed { get; set; }
    public virtual float DesiredSpeed { get; set; }
    public virtual bool Stopped { get; set; }

    public bool Follow { get; set; }

    public int Carriages { get; set; }

    public void SetAngle(float angle)
    {
        while (angle < 0) angle += 360;
        while (angle > 360) angle -= 360;
        Angle = angle;
    }

    public Train Clone()
    {
        return new Train(this);
    }

    public void AddCarriage()
    {
        if (Carriages < 10)
        {
            Carriages += 1;
        }
    }

    public void RemoveCarriage()
    {
        if (Carriages > 0)
        {
            Carriages -= 1;
        }
    }

    internal void ForceSpeed(float speed)
    {
        CurrentSpeed = speed;
        DesiredSpeed = speed;
    }

    public void Start() => Stopped = false;

    public void Stop() => Stopped = true;

    internal void Pause() => _collisionAhead = true;

    internal void Resume() => _collisionAhead = false;

    public void Slower()
    {
        if (DesiredSpeed > 5)
        {
            DesiredSpeed -= 5;
        }
    }

    public void Faster()
    {
        if (DesiredSpeed < MaximumSpeed)
        {
            DesiredSpeed += 5;
        }
    }

    public override string ToString() => $"Train {UniqueID} [Column: {Column} | Row: {Row} | Left: {RelativeLeft} | Top: {RelativeTop} | Angle: {Angle} | Speed: {CurrentSpeed}]";

    internal TrainPosition GetPosition() => new(Column, Row, RelativeLeft, RelativeTop, Angle, 0);

    internal void AdjustSpeed()
    {
        if (Stopped || _collisionAhead)
        {
            CurrentSpeed = Math.Max(CurrentSpeed - 1.0f, 0);
        }
        else if (DesiredSpeed > CurrentSpeed)
        {
            CurrentSpeed = Math.Min(CurrentSpeed + 1.0f, DesiredSpeed);
        }
        else if (DesiredSpeed < CurrentSpeed)
        {
            CurrentSpeed = Math.Max(CurrentSpeed - 1.0f, 0);
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentSpeed)));
    }

    public void ApplyStep(TrainPosition newPosition)
    {
        Column = newPosition.Column;
        Row = newPosition.Row;
        Angle = newPosition.Angle;
        RelativeLeft = newPosition.RelativeLeft;
        RelativeTop = newPosition.RelativeTop;
    }
}
