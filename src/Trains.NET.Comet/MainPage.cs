using System.Collections.Generic;
using System.Threading.Tasks;
using Comet;
using Trains.NET.Engine;
using Trains.NET.Instrumentation;
using Trains.NET.Rendering;

namespace Trains.NET.Comet
{
    public class MainPage : View
    {
        private readonly ILayout _trackLayout;
        private readonly IGameStorage _gameStorage;
        private readonly ITerrainMap _terrainMap;
        private readonly IGame _game;
        private readonly TrainsDelegate _controlDelegate;
        private bool _presenting = true;

        public MainPage(IGame game,
                        ILayout trackLayout,
                        IGameStorage gameStorage,
                        ITerrainMap terrainMap,
                        TrainsDelegate trainsDelegate)
        {
            this.Title("Trains - " + ThisAssembly.AssemblyInformationalVersion);

            _game = game;
            _controlDelegate = trainsDelegate;
            this.Body = () => new DrawableControl(_controlDelegate).FillHorizontal();

            _trackLayout = trackLayout;
            _gameStorage = gameStorage;

            _ = PresentLoop();

            _terrainMap = terrainMap;
        }

        private readonly PerSecondTimedStat _fps = InstrumentationBag.Add<PerSecondTimedStat>("Real-FPS");
        private readonly ElapsedMillisecondsTimedStat _drawTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Real Draw Time");

        private async Task PresentLoop()
        {
            while (_presenting)
            {
                _drawTime.Start();

                _controlDelegate.Invalidate();

                _drawTime.Stop();

                _fps.Update();

                await Task.Delay(16).ConfigureAwait(true);
            }
        }

        public void Save()
        {
            _gameStorage.WriteStaticEntities(_trackLayout);
            _gameStorage.WriteTerrain(_terrainMap);
        }

        public void Redraw()
        {
            ViewPropertyChanged(ResetPropertyString, null);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _presenting = false;
                _game.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
