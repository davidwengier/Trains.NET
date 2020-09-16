using System;
using Trains.NET.Engine;

namespace Trains.NET.Rendering
{
    public interface IGameManager
    {
        event EventHandler? Changed;

        ITool CurrentTool { get; set; }
        bool BuildMode { get; set; }
    }
}
