using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine.Tracks
{
    internal class TrackLayout : ITrackLayout
    {
        public event EventHandler? TracksChanged;

        private readonly Dictionary<(int, int), Track> _tracks = new();

        public void AddTrack(int column, int row)
        {
            if (_tracks.TryGetValue((column, row), out Track track))
            {
                track.SetBestTrackDirection(true);
            }
            else
            {
                track = new Track(this)
                {
                    Column = column,
                    Row = row
                };
                _tracks.Add((column, row), track);

                track.SetBestTrackDirection(false);
            }

            TracksChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveTrack(int column, int row)
        {
            if (_tracks.TryGetValue((column, row), out Track track))
            {
                _tracks.Remove((column, row));
                track.RefreshNeighbors(true);
            }

            TracksChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetTracks(IEnumerable<Track> tracks)
        {
            _tracks.Clear();

            foreach (Track track in tracks)
            {
                _tracks[(track.Column, track.Row)] = new Track(this)
                {
                    Column = track.Column,
                    Row = track.Row,
                    Direction = track.Direction,
                    AlternateState = track.AlternateState
                };
            }

            foreach (Track track in _tracks.Values)
            {
                track.ReevaluateHappiness();
            }

            TracksChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ToggleTrack(int column, int row)
        {
            if (TryGet(column, row, out var track) && track.HasAlternateState())
            {
                track.AlternateState = !track.AlternateState;

                TracksChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool TryGet(int column, int row, [NotNullWhen(true)] out Track? track)
        {
            return _tracks.TryGetValue((column, row), out track);
        }

        public void Clear()
        {
            _tracks.Clear();

            TracksChanged?.Invoke(this, EventArgs.Empty);
        }

        public IEnumerator<Track> GetEnumerator()
        {
            foreach ((_, _, Track track) in _tracks)
            {
                yield return track;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
