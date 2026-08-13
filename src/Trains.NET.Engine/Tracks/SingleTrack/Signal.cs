namespace Trains.NET.Engine;

public class Signal : SingleTrack, IUpdatableEntity
{
    private const int TemporaryStopTime = 5 * 60;

    public SignalState SignalState { get; set; }

    public int TemporaryStopCounter { get; set; }

    public override string Identifier
        => $"{base.Identifier}.{SignalState}";

    public override bool HasMultipleStates => true;

    public Signal() : base()
    {
        SignalState = SignalState.Go;
    }

    public override void NextState()
    {
        SignalState = SignalState switch
        {
            SignalState.Go => SignalState.TemporaryStop,
            SignalState.TemporaryStop => SignalState.Stop,
            SignalState.Stop => SignalState.Go,
            _ => throw new InvalidOperationException()
        };

        OnChanged();
    }

    public void Update()
    {
        if (SignalState == SignalState.TemporaryStop &&
            ++TemporaryStopCounter >= TemporaryStopTime)
        {
            SignalState = SignalState.Go;
            TemporaryStopCounter = 0;

            OnChanged();
        }
    }

    public override bool IsBlocked()
        => SignalState != SignalState.Go;
}
