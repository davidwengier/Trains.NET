using System;
using System.Collections.Generic;
using Trains.NET.Engine;

namespace Trains.NET.Rendering.Trains
{
    public class TrainPainter : ITrainPainter
    {
        private readonly Dictionary<Train, ITrainPalette> _paletteMap = new();
        private readonly OrderedList<ITrainPalette> _trainPalettes;
        private readonly Random _random = new Random();

        public TrainPainter(OrderedList<ITrainPalette> trainPalettes)
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
            return _trainPalettes[_random.Next(0, _trainPalettes.Count)];
        }
    }
}
