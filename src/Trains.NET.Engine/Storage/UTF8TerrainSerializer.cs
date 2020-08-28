using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trains.NET.Engine
{
    public class UTF8TerrainSerializer : ITerrainSerializer
    {
        public IEnumerable<Terrain> Deserialize(string[] lines)
        {
            var terrainList = new List<Terrain>();

            for (int r = 0; r < lines.Length; r++)
            {
                string? line = lines[r];
                string[]? heights = line.Split(',');
                for (int c = 0; c < heights.Length; c++)
                {

                    if (!int.TryParse(heights[c], out int height))
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

            int maxColumn = terrainList.Max(t => t.Column);
            int maxRow = terrainList.Max(t => t.Row);

            for (int r = 0; r <= maxRow; r++)
            {
                var heights = new List<int>();
                for (int c = 0; c <= maxColumn; c++)
                {
                    if (!dict.TryGetValue((c, r), out int height))
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
}
