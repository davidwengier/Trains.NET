using System;

namespace Trains.NET.Engine
{
    public interface IGameManager
    {
        event EventHandler? Changed;

        ITool CurrentTool { get; set; }
        bool BuildMode { get; set; }
    }
}
