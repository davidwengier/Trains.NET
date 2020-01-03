using System;
using Comet;
using Trains.NET.Rendering;

namespace Trains.NET.Comet
{
    public class MainPage : View
    {
        public MainPage(IGame game)
        {
            this.Title("Trains.NET");

            this.Body = () =>
            {
                var controlsPanel = new VStack();

                foreach (Tool tool in Enum.GetValues(typeof(Tool)))
                {
                    controlsPanel.Add(new Button(tool.ToString(), () => game.CurrentTool = tool));
                }

                return new HStack()
                {
                    controlsPanel.Frame(100),
                    new DrawableControl(new TrainsDelegate(game)).FillVertical()
                }.FillHorizontal();
            };
        }
    }
}
