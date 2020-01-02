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
                var controlsPanel = new VStack()
                {
                    new Button("Pointer"),
                    new Button("Track"),
                    new Button("Eraser"),
                    new Spacer(),
                    new Button("Debug")
                };

                return new HStack()
                {
                    controlsPanel,
                    new DrawableControl(new TrainsDelegate(game))
                };
            };
        }
    }
}
