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

            for (int r = 0; r < lines.Length - 1; r++)
            {
                for (int c = 0; c < lines[r].Length; c++)
                {
                    char current = lines[r][c];
                    if (current == ' ') continue;

                    var intValue = (int)current;
                    var height = intValue >> 3;
                    TerrainType terrainType = (TerrainType) (intValue - (height << 3));

                    terrainList.Add(new Terrain
                    {
                        Row = r,
                        Column = c,
                        Height = height,
                        TerrainType = terrainType
                    });

                }
            }

            return terrainList;
        }

        public string Serialize(IEnumerable<Terrain> terrainList)
        {
            if (!terrainList.Any()) return string.Empty;

            var sb = new StringBuilder();

            var happinessSb = new StringBuilder();

            int maxColumn = terrainList.Max(t => t.Column);
            int maxRow = terrainList.Max(t => t.Row);

            for (int r = 0; r <= maxRow; r++)
            {
                for (int c = 0; c <= maxColumn; c++)
                {
                    Terrain terrain = terrainList.FirstOrDefault(t => t.Column == c && t.Row == r);
                    if (terrain == null)
                    {
                        sb.Append(' ');
                        continue;
                    }

                    int height = terrain.Height;
                    int terrainType = (int) terrain.TerrainType;
                    char terrainCode = (char)((height << 3) + terrainType);
                    sb.Append(terrainCode);
                }
                sb.AppendLine();
            }
            sb.AppendLine(happinessSb.ToString());

            return sb.ToString();
        }
    }
}
