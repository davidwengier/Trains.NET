using System.Collections.Generic;
using Comet;
using Trains.NET.Engine;
using Trains.NET.Rendering;

namespace Trains.NET.Comet
{
    public class MainPage : View
    {
        public MainPage(IGame game, IPixelMapper pixelMapper, IEnumerable<ITool> tools)
        {
            HotReloadHelper.Register(this, game);

            this.Title("Trains.NET");

            this.Body = () =>
            {
                var controlsPanel = new VStack();

                var controlDelegate = new TrainsDelegate(game, pixelMapper);

                foreach (ITool tool in tools)
                {
                    controlsPanel.Add(new Button(tool.Name, () => controlDelegate.CurrentTool = tool));
                }

                return new HStack()
                {
                    controlsPanel.Frame(100),
                    new DrawableControl(controlDelegate).FillVertical()
                }.FillHorizontal();
            };
        }
    }
}
