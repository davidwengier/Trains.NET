namespace Trains.NET.Engine;

public class ConfigEntity : IEntity
{
    // Placeholders as there is nothing lower than IEntity at the moment.
    public int Column { get => -1; set { } }
    public int Row { get => -1; set { } }
    public string Name { get; }
    public int Value { get; }
    public ConfigEntity(string name, int value)
    {
        this.Name = name;
        this.Value = value;
    }
}
