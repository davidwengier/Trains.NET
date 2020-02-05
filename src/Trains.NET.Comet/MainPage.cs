using System.Collections.Generic;
using System.Threading;
using Comet;
using Trains.NET.Engine;
using Trains.NET.Rendering;

namespace Trains.NET.Comet
{
    public class MainPage : View
    {
        private readonly Timer _timer;

        public MainPage(IGame game, IPixelMapper pixelMapper, IEnumerable<ITool> tools)
        {
            HotReloadHelper.Register(this, game);

            this.Title("Trains.NET");

            var controlDelegate = new TrainsDelegate(game, pixelMapper);

            this.Body = () =>
            {
                var controlsPanel = new VStack();

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

            _timer = new Timer((state) =>
            {
                ThreadHelper.Run(async () =>
                {
                    await ThreadHelper.SwitchToMainThreadAsync();

                    controlDelegate.Invalidate();
                });
            }, null, 0, 16);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
