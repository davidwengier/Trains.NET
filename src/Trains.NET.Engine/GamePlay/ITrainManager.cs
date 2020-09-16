
using System;

namespace Trains.NET.Engine
{
    public interface ITrainManager
    {
        event EventHandler? Changed;
        Train? CurrentTrain { get; set; }
    }
}
