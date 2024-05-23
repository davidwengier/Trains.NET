namespace Trains.NET.Engine;

public class Tree(int seed) : IStaticEntity, ISeeded
{
    public int Column { get; set; }
    public int Row { get; set; }
    public int Seed { get; } = seed;

    public string Identifier => this.Seed.ToString();

    public void Created()
    {
    }

    public void Removed()
    {
    }

    public void Updated()
    {
    }

    public void Replaced()
    {
    }

    public void Stored(ILayout? collection)
    {
    }
}
