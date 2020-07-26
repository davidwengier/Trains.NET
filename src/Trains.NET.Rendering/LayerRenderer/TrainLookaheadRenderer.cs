﻿using Trains.NET.Engine;
using Trains.NET.Rendering.Trains;

namespace Trains.NET.Rendering.LayerRenderer
{
    [Order(425)]
    internal class TrainLookaheadRenderer : ILayerRenderer
    {
        private readonly IGameBoard _gameBoard;
        private readonly ITrackParameters _parameters;
        private readonly ITrainPainter _painter;

        public TrainLookaheadRenderer(IGameBoard gameBoard, ITrackParameters parameters, ITrainPainter painter)
        {
            _gameBoard = gameBoard;
            _parameters = parameters;
            _painter = painter;
        }

        public bool Enabled { get; set; }

        public string Name => "Hitbox";

        public void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper)
        {
            foreach (Train train in _gameBoard.GetMovables())
            {
                var _paint = new PaintBrush
                {
                    Color = _painter.GetPalette(train).FrontSectionEndColor,
                    Style = PaintStyle.Fill
                };

                (int x, int y) = pixelMapper.CoordsToViewPortPixels(train.Column, train.Row);

                canvas.DrawRect(x, y, _parameters.CellSize, _parameters.CellSize, _paint);

                float speedModifier = 0.005f; // * ((_gameTimer?.TimeSinceLastTick / 16f) ?? 1);
                foreach (TrainPosition? position in _gameBoard.GetNextSteps(train, train.LookaheadDistance * speedModifier))
                {
                    (x, y) = pixelMapper.CoordsToViewPortPixels(position.Column, position.Row);

                    canvas.DrawRect(x, y, _parameters.CellSize, _parameters.CellSize, _paint);
                }
            }
        }
    }
}
