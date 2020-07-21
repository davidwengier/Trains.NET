namespace Trains.NET.Instrumentation
{
    public interface IStat
    {
        string GetDescription();

        bool ShouldShow();
    }
}
