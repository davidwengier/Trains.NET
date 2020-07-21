namespace Trains.NET.Instrumentation
{
    public class CountStat : IStat
    {
        private readonly string _thing;
        public CountStat(string nameOfThingYouAreCounting)
        {
            _thing = nameOfThingYouAreCounting;
        }
        public int Value { get; private set; }
        public void Add() => this.Value++;
        public void Set(int value) => this.Value = value;
        public string GetDescription() => this.Value + ' ' + _thing;
        public bool ShouldShow() => true;
    }
}
