﻿using System.Linq;
using Trains.NET.Engine;
using Trains.NET.Engine.Tracks;

namespace Trains.NET.Rendering
{
    [Order(0)]
    internal class TerrainTypeRenderer : ILayerRenderer
    {
        private readonly ITerrainMap _terrainMap;
        private readonly ITrackParameters _trackParameters;

        public TerrainTypeRenderer(ITerrainMap terrainMap, ITrackParameters trackParameters)
        {
            _terrainMap = terrainMap;
            _trackParameters = trackParameters;
        }

        public bool Enabled { get; set; } = true;
        public string Name => "TerrainType";

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            Color backgroundColour = Colors.LightGreen;

            // Draw grass background for viewport
            canvas.DrawRect(0, 0, pixelMapper.ViewPortWidth, pixelMapper.ViewPortHeight, new PaintBrush { Style = PaintStyle.Fill, Color = backgroundColour});

            // Draw any non-grass cells
            foreach (Terrain terrain in _terrainMap)
            {
                Color colour = terrain.TerrainType switch
                {
                    TerrainType.Sand => Colors.LightYellow,
                    TerrainType.Water => Colors.LightBlue,
                    _ => backgroundColour,
                };

                if (colour == backgroundColour) continue;

                (int x, int y) = pixelMapper.CoordsToViewPortPixels(terrain.Column, terrain.Row);
                canvas.DrawRect(x, y, _trackParameters.CellSize, _trackParameters.CellSize, new PaintBrush { Style = PaintStyle.Fill, Color = colour });
            }
        }

    }
}