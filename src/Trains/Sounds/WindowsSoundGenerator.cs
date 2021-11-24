using Trains.NET.Engine;

namespace Trains.Sounds;

public sealed class WindowsSoundGenerator : ITogglable, IDisposable
{
    private bool _enabled;
    private readonly MidiSynth _synth;

    public string Name => "Sounds";

    public bool Enabled
    {
        get { return _enabled; }
        set
        {
            if (_enabled == value)
            {
                return;
            }

            _enabled = value;
            if (_enabled)
            {
                _synth.Init();
                Task.Run(async () => await RunLoopAsync().ConfigureAwait(false)); ;
            }
            else
            {
                _synth.Close();
            }
        }
    }

    public WindowsSoundGenerator()
    {
        _synth = new MidiSynth();
    }

    private async Task RunLoopAsync()
    {

        var random = new Random();

        _synth.Train();
        while (_enabled)
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
