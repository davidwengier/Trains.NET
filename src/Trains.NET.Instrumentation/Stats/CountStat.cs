namespace Trains.NET.Instrumentation;

public class CountStat(string nameOfThingYouAreCounting) : IStat
{
    private readonly string _thing = nameOfThingYouAreCounting;

    public int Value { get; private set; }
    public void Add() => Value++;
    public void Set(int value) => Value = value;
    public string GetDescription() => Value + ' ' + _thing;
    public bool ShouldShow() => true;
}
