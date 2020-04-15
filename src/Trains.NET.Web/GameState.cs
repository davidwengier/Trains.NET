using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Web
{
    public class GameState
    {
        public ITool CurrentTool { get; set; }

        public GameState(OrderedList<ITool> tools)
        {
            this.CurrentTool = tools.First();
        }
    }
}
