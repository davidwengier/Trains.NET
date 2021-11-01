using System;

namespace Trains.NET.Engine;

public class Tree : IStaticEntity
{
    private static readonly Random s_random = new();

    public int Column { get; set; }
    public int Row { get; set; }

    public int Seed { get; set; }

    public string Identifier => this.Seed.ToString();

    public void Created()
    {
        this.Seed = s_random.Next(1, 100);
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
