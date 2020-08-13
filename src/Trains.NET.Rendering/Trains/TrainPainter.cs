using System;
using System.Collections.Generic;
using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.Trains
{
    public class TrainPainter : ITrainPainter
    {
        private readonly Dictionary<Train, ITrainPalette> _paletteMap = new();
        private readonly IEnumerable<ITrainPalette> _trainPalettes;
        private readonly Random _random = new Random();

        public TrainPainter(IEnumerable<ITrainPalette> trainPalettes)
        {
            _trainPalettes = trainPalettes;
        }


        public ITrainPalette GetPalette(Train train)
        {
            if (!_paletteMap.ContainsKey(train))
            {
                _paletteMap.Add(train, GetRandomPalette());
            }
           return _paletteMap[train];
        }

        private ITrainPalette GetRandomPalette()
        {
            return _trainPalettes.ElementAt(_random.Next(0, _trainPalettes.Count()));
        }
    }
}
