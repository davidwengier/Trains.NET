namespace Trains.NET.Engine;

public class Tree : IStaticEntity, ISeeded
{
    public Tree(int seed)
    {
        this.Seed = seed;
    }

    public int Column { get; set; }
    public int Row { get; set; }
    public int Seed { get; }

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
