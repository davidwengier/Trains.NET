namespace Trains.NET.Instrumentation;

public class CountStat(string nameOfThingYouAreCounting) : IStat
{
    private readonly string _thing = nameOfThingYouAreCounting;

    public int Value { get; private set; }
    public void Add() => this.Value++;
    public void Set(int value) => this.Value = value;
    public string GetDescription() => this.Value + ' ' + _thing;
    public bool ShouldShow() => true;
}
