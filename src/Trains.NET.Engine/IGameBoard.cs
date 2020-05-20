using System.Collections.Generic;

namespace Trains.NET.Engine
{
    public interface IGameBoard
    {
        int Columns { get; set; }
        int Rows { get; set; }
        bool Enabled { get; set; }

        void AddTrack(int column, int row);
        void RemoveTrack(int column, int row);
        Track? GetTrackAt(int column, int row);
        System.Collections.Generic.IEnumerable<(int, int, Track)> GetTracks();
        void ClearAll();
        IMovable? AddTrain(int column, int row);
        IEnumerable<IMovable> GetMovables();
        void RemoveMovable(IMovable thing);
        IEnumerable<T> GetMovables<T>() where T : IMovable;
        IMovable? GetMovableAt(int column, int row);
        Track? GetTrackForTrain(Train train);
        void LoadTracks(IEnumerable<Track> tracks);
        List<TrainPosition> GetNextSteps(Train train, float distanceToMove);
    }
}
