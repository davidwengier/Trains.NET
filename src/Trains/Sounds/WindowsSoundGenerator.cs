using System;
using System.Threading.Tasks;
using Trains.NET.Engine.Sounds;

namespace Trains.Sounds
{
    public sealed class WindowsSoundGenerator : ISoundGenerator
    {
        private readonly MidiSynth _synth;

        public bool IsRunning { get; private set; }

        public WindowsSoundGenerator()
        {
            _synth = new MidiSynth();
        }

        public void Start()
        {
            if (!this.IsRunning)
            {
                this.IsRunning = true;
                _synth.Init();
                Task.Run(async () => await RunLoopAsync().ConfigureAwait(false)); ;
            }
        }

        public void Stop()
        {
            if (this.IsRunning)
            {
                this.IsRunning = false;
                _synth.Close();
            }
        }

        private async Task RunLoopAsync()
        {

            var random = new Random();

            _synth.Train();
            while (this.IsRunning)
            {
                switch (random.Next(0, 20))
                {
                    case 0:
                        await _synth.SoundAsync(HornModel.NathanK3LA, random.Next(1000, 3000)).ConfigureAwait(false);
                        break;
                    case 1 or 2:
                        await _synth.WhistleAsync().ConfigureAwait(false);
                        break;
                    case >= 3 and <= 6:
                        await _synth.TweetAsync().ConfigureAwait(false);
                        break;
                }
                await Task.Delay(random.Next(500, 3000)).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            _synth.Dispose();
        }
    }
}
