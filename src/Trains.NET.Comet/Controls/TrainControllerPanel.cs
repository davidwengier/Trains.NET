using Comet;

namespace Trains.NET.Comet
{
    public class TrainControllerPanel : HStack
    {
        public TrainControllerPanel(ITrainController trainControls)
            : base(VerticalAlignment.Center, null)
        {
            Add(new HStack()
                {
                    new Text(trainControls.Display),
                    trainControls.BuildMode
                    ?   new HStack()
                        {
                            new Button("Delete", trainControls.Delete),
                        }
                    :   new HStack()
                        {
                            new Button("Stop", trainControls.Stop),
                            new Button("Slower", trainControls.Slower),
                            new Text(trainControls.SpeedDisplay),
                            new Button("Faster", trainControls.Faster),
                            new Button("Go", trainControls.Start),
                            new Button("Toggle Follow Mode", trainControls.ToggleFollowMode),
                        },
                });
        }
    }
}
