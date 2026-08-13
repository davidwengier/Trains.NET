using System.Text;

namespace Trains.NET.Engine;

public class UTF8TerrainSerializer : ITerrainSerializer
{
    public IEnumerable<Terrain> Deserialize(string[] lines)
    {
        var terrainList = new List<Terrain>();

        for (var r = 0; r < lines.Length; r++)
        {
            var line = lines[r];
            var heights = line.Split(',');
            for (var c = 0; c < heights.Length; c++)
            {

                if (!int.TryParse(heights[c], out var height))
                {
                    throw new System.Exception("Invalid height read from file");
                }

                terrainList.Add(new Terrain
                {
                    Row = r,
                    Column = c,
                    Height = height,
                });
            }
        }

        return terrainList;
    }

    public string Serialize(IEnumerable<Terrain> terrainList)
    {
        if (!terrainList.Any()) return string.Empty;

        var dict = terrainList.ToDictionary(t => (t.Column, t.Row), t => t.Height);

        var sb = new StringBuilder();

        var happinessSb = new StringBuilder();

        var maxColumn = terrainList.Max(t => t.Column);
        var maxRow = terrainList.Max(t => t.Row);

        for (var r = 0; r <= maxRow; r++)
        {
            var heights = new List<int>();
            for (var c = 0; c <= maxColumn; c++)
            {
                if (!dict.TryGetValue((c, r), out var height))
                {
                    height = 0;
                }
                heights.Add(height);
            }

            sb.AppendLine(string.Join(',', heights.Select(h => h.ToString())));
        }

        return sb.ToString();
    }
}
