namespace Trains.NET.Engine;

public static class PRNGExtentions
{
    public static BasicPRNG GetPRNG(this ISeeded item) => new(item.Seed);
}
