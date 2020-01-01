using Comet;
using Comet.Skia;

namespace Trains.NET.Comet
{
    public class MainPage : View
    {
        public MainPage()
        {
            this.Title("Trains.NET");

            this.Body = () => new HStack()
            {
                new VStack()
                {
                    new Button("Pointer"),
                    new Button("Track"),
                    new Button("Eraser"),
                },
                new SkiaView()
            };
        }
    }
}
