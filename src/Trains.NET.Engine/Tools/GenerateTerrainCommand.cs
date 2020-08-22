using System.Collections.Generic;

namespace Trains.NET.Engine
{
    [Order(12)]
    public class GenerateTerrainCommand : ICommand
    {
        private readonly ITerrainMap _terrainMap;

        public GenerateTerrainCommand(ITerrainMap terrainMap)
        {
            _terrainMap = terrainMap;
        }

        public string Name => "Generate Terrain";

        public void Execute()
        {
            if (!_terrainMap.IsEmpty()) return;

            var maxHeight = Terrain.MaxHeight;
            var CellsAcross = 3000 / 40;
            var CellsDown = 3000 / 40;

            var noiseMap = NoiseGenerator.GenerateNoiseMap(CellsAcross, CellsDown, 8);
            var terrainlist = new List<Terrain>();

            
            for (int x=0;x< CellsAcross; x++)
            {
                for (int y = 0; y < CellsDown; y++)
                {
                    var key = (x, y);
                    var noise = noiseMap.ContainsKey(key) ? noiseMap[key] : 0;
                    terrainlist.Add(new Terrain
                    {
                        Column = x,
                        Row = y,
                        Height = (int) (noise * maxHeight)
                    });
                }
            }
            _terrainMap.Set(terrainlist);
        }
    }
}
