using System;

namespace Trains.NET.Engine
{
    public class Track
    {
        private readonly IGameBoard _gameBoard;

        public Track(IGameBoard gameBoard)
        {
            _gameBoard = gameBoard;
        }

        public int Column { get; set; }
        public int Row { get; set; }
        public TrackDirection Direction { get; set; }
        public bool Happy { get; set; }
        public bool CanConnectRight => this.Direction switch
        {
            _ when !this.Happy => true,
            TrackDirection.RightDown => true,
            TrackDirection.RightUp => true,
            TrackDirection.Horizontal => true,
            _ => false
        };

        public bool CanConnectDown => this.Direction switch
        {
            _ when !this.Happy => true,
            TrackDirection.RightDown => true,
            TrackDirection.LeftDown => true,
            TrackDirection.Vertical => true,
            _ => false
        };

        public bool CanConnectLeft => this.Direction switch
        {
            _ when !this.Happy => true,
            TrackDirection.LeftDown => true,
            TrackDirection.LeftUp => true,
            TrackDirection.Horizontal => true,
            _ => false
        };
        public bool CanConnectUp => this.Direction switch
        {
            _ when !this.Happy => true,
            TrackDirection.LeftUp => true,
            TrackDirection.RightUp => true,
            TrackDirection.Vertical => true,
            _ => false
        };
        public void SetBestTrackDirection()
        {
            SetBestTrackDirection(true);
        }

        public void SetBestTrackDirection(bool ignoreIfHappy)
        {
            if (this.Happy && ignoreIfHappy) return;

            TrackNeighbors neighbors = GetNeighbors();

            TrackDirection newDirection;
            if (neighbors.Up != null || neighbors.Down != null)
            {
                if (neighbors.Left != null)
                {
                    newDirection = neighbors.Up == null ? TrackDirection.LeftDown : TrackDirection.LeftUp;
                }
                else if (neighbors.Right != null)
                {
                    newDirection = neighbors.Up == null ? TrackDirection.RightDown : TrackDirection.RightUp;
                }
                else
                {
                    newDirection = TrackDirection.Vertical;
                }
            }
            else if (neighbors.Left != null || neighbors.Right != null)
            {
                if (neighbors.Up != null)
                {
                    newDirection = neighbors.Left == null ? TrackDirection.RightUp : TrackDirection.LeftUp;
                }
                else if (neighbors.Down != null)
                {
                    newDirection = neighbors.Left == null ? TrackDirection.RightDown : TrackDirection.LeftDown;
                }
                else
                {
                    newDirection = TrackDirection.Horizontal;
                }
            }
            else
            {
                newDirection = TrackDirection.Horizontal;
            }

            if (newDirection != this.Direction)
            {
                this.Direction = newDirection;
                RefreshNeighbors(true);
            }

            this.Happy = neighbors.Count >= 2;
        }

        public void RefreshNeighbors(bool ignoreIfHappy)
        {
            TrackNeighbors neighbors = GetNeighbors();
            neighbors.Up?.SetBestTrackDirection(ignoreIfHappy);
            neighbors.Down?.SetBestTrackDirection(ignoreIfHappy);
            neighbors.Right?.SetBestTrackDirection(ignoreIfHappy);
            neighbors.Left?.SetBestTrackDirection(ignoreIfHappy);
        }

        public TrackNeighbors GetNeighbors()
        {
            var left = _gameBoard.GetTrackAt(this.Column - 1, this.Row);
            var up = _gameBoard.GetTrackAt(this.Column, this.Row - 1);
            var right = _gameBoard.GetTrackAt(this.Column + 1, this.Row);
            var down = _gameBoard.GetTrackAt(this.Column, this.Row + 1);

            return new TrackNeighbors(
                left?.CanConnectRight == true ? left : null,
                up?.CanConnectDown == true ? up : null,
                right?.CanConnectLeft == true ? right : null,
                down?.CanConnectUp == true ? down : null
                );
        }
    }
}
