using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Trains.NET.Engine
{
    [DebuggerDisplay("{Direction,nq}")]
    public class Track
    {
        private bool _isSettingDirection;
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
            TrackDirection.Cross => true,
            TrackDirection.LeftRightDown => true,
            TrackDirection.LeftRightUp => true,
            TrackDirection.RightUpDown => true,
            _ => false
        };

        public bool CanConnectDown => this.Direction switch
        {
            _ when !this.Happy => true,
            TrackDirection.RightDown => true,
            TrackDirection.LeftDown => true,
            TrackDirection.Vertical => true,
            TrackDirection.Cross => true,
            TrackDirection.LeftRightDown => true,
            TrackDirection.LeftUpDown => true,
            TrackDirection.RightUpDown => true,
            _ => false
        };

        public bool CanConnectLeft => this.Direction switch
        {
            _ when !this.Happy => true,
            TrackDirection.LeftDown => true,
            TrackDirection.LeftUp => true,
            TrackDirection.Horizontal => true,
            TrackDirection.Cross => true,
            TrackDirection.LeftRightDown => true,
            TrackDirection.LeftRightUp => true,
            TrackDirection.LeftUpDown => true,
            _ => false
        };

        public bool CanConnectUp => this.Direction switch
        {
            _ when !this.Happy => true,
            TrackDirection.LeftUp => true,
            TrackDirection.RightUp => true,
            TrackDirection.Vertical => true,
            TrackDirection.Cross => true,
            TrackDirection.LeftUpDown => true,
            TrackDirection.LeftRightUp => true,
            TrackDirection.RightUpDown => true,
            _ => false
        };

        public void SetBestTrackDirectionOrCross()
        {
            TrackNeighbors allNeighbors = GetAllNeighbors();
            if (allNeighbors.Up != null && allNeighbors.Up.GetNeighbors().Count == 4)
            {
                allNeighbors.Up.SetBestTrackDirection(true);
            }
            if (allNeighbors.Left != null && allNeighbors.Left.GetNeighbors().Count == 4)
            {
                allNeighbors.Left.SetBestTrackDirection(true);
            }
            if (allNeighbors.Down != null && allNeighbors.Down.GetNeighbors().Count == 4)
            {
                allNeighbors.Down.SetBestTrackDirection(true);
            }
            if (allNeighbors.Right != null && allNeighbors.Right.GetNeighbors().Count == 4)
            {
                allNeighbors.Right.SetBestTrackDirection(true);
            }

            SetBestTrackDirection(true);
        }

        public void SetBestTrackDirection(bool refreshNeighbors)
        {
            if (_isSettingDirection) return;
            _isSettingDirection = true;

            TrackNeighbors neighbors = GetNeighbors();

            int countBefore = neighbors.Count;

            TrackDirection newDirection;
            // Simple cross, someone filling in the middle
            if (neighbors.Count == 4)
            {
                newDirection = TrackDirection.Cross;
            }
            // 3-way connections
            else if (neighbors.Count == 3 && neighbors.Up != null && neighbors.Down != null)
            {
                newDirection = neighbors.Left == null ? TrackDirection.RightUpDown : TrackDirection.LeftUpDown;
            }
            else if (neighbors.Count == 3 && neighbors.Left != null && neighbors.Right != null)
            {
                newDirection = neighbors.Up == null ? TrackDirection.LeftRightDown : TrackDirection.LeftRightUp;
            }
            // standard 2-way connections
            else if (neighbors.Up != null || neighbors.Down != null)
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

            if (newDirection != this.Direction || GetNeighbors().Count > countBefore)
            {
                this.Direction = newDirection;
                if (refreshNeighbors)
                {
                    RefreshNeighbors(false);
                }
            }

            this.Happy = neighbors.Count > 1;

            _isSettingDirection = false;
        }

        public void RefreshNeighbors(bool refreshAllNeighbors)
        {
            TrackNeighbors neighbors = GetNeighbors();
            neighbors.Up?.SetBestTrackDirection(refreshAllNeighbors);
            neighbors.Down?.SetBestTrackDirection(refreshAllNeighbors);
            neighbors.Right?.SetBestTrackDirection(refreshAllNeighbors);
            neighbors.Left?.SetBestTrackDirection(refreshAllNeighbors);
        }

        public TrackNeighbors GetNeighbors()
        {
            Track? left = _gameBoard.GetTrackAt(this.Column - 1, this.Row);
            Track? up = _gameBoard.GetTrackAt(this.Column, this.Row - 1);
            Track? right = _gameBoard.GetTrackAt(this.Column + 1, this.Row);
            Track? down = _gameBoard.GetTrackAt(this.Column, this.Row + 1);

            return new TrackNeighbors(
                left?.CanConnectRight == true ? left : null,
                up?.CanConnectDown == true ? up : null,
                right?.CanConnectLeft == true ? right : null,
                down?.CanConnectUp == true ? down : null
                );
        }

        public TrackNeighbors GetAllNeighbors()
        {
            Track? left = _gameBoard.GetTrackAt(this.Column - 1, this.Row);
            Track? up = _gameBoard.GetTrackAt(this.Column, this.Row - 1);
            Track? right = _gameBoard.GetTrackAt(this.Column + 1, this.Row);
            Track? down = _gameBoard.GetTrackAt(this.Column, this.Row + 1);

            return new TrackNeighbors(left, up, right, down);
        }
    }
}
