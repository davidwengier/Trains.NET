using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Comet;
using Trains.NET.Engine;
using Trains.NET.Instrumentation;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Tracks;

namespace Trains.NET.Comet
{
    public class MainPage : View
    {
        private readonly State<bool> _configurationShown = false;

        private readonly IStaticEntityCollection _trackLayout;
        private readonly IGameStorage _gameStorage;
        private readonly IGame _game;
        private readonly TrainsDelegate _controlDelegate;
        private readonly MiniMapDelegate _miniMapDelegate;
        private Size _lastSize = Size.Empty;
        private bool _presenting = true;

        public MainPage(IGame game,
                        IPixelMapper pixelMapper,
                        OrderedList<ITool> tools,
                        OrderedList<ILayerRenderer> layers,
                        OrderedList<ICommand> commands,
                        ITrainController trainControls,
                        ITrackParameters trackParameters,
                        IStaticEntityCollection trackLayout,
                        IGameStorage gameStorage,
                        Factory<IToolPreviewer> previewerFactory)
        {
            this.Title("Trains - " + ThisAssembly.AssemblyInformationalVersion);

            _game = game;
            _controlDelegate = new TrainsDelegate(game, pixelMapper, previewerFactory);
            _miniMapDelegate = new MiniMapDelegate(trackLayout, trackParameters, pixelMapper);

            this.Body = () =>
            {
                return new HStack()
                {
                    new VStack()
                    {
                        _configurationShown ? null :
                            new Button(trainControls.BuildMode ? "Building" : "Playing", ()=> SwitchGameMode()),
                        new Spacer(),
                        _configurationShown ?
                                CreateConfigurationControls(layers) :
                                CreateToolsControls(tools, _controlDelegate, trainControls.BuildMode.Value),
                        new Spacer(),
                        _configurationShown || !trainControls.BuildMode ? null :
                            CreateCommandControls(commands),
                        new Spacer(),
                        new Button("Configuration", ()=> _configurationShown.Value = !_configurationShown.Value),
                        new DrawableControl(_miniMapDelegate).Frame(height: 100)
                    }.Frame(100, alignment: Alignment.Top),
                    new VStack()
                    {
                        new TrainControllerPanel(trainControls),
                        new DrawableControl(_controlDelegate)
                    }
                }.FillHorizontal();
            };

            _trackLayout = trackLayout;
            _gameStorage = gameStorage;

            _ = PresentLoop();

            void SwitchGameMode()
            {
                trainControls.ToggleBuildMode();

                if (_controlDelegate == null) return;

                _controlDelegate.CurrentTool.Value = tools.FirstOrDefault(t => ShouldShowTool(trainControls.BuildMode, t));
            }
        }

        private readonly PerSecondTimedStat _fps = InstrumentationBag.Add<PerSecondTimedStat>("Real-FPS");
        private readonly ElapsedMillisecondsTimedStat _drawTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Real Draw Time");

        private async Task PresentLoop()
        {
            while (_presenting)
            {
                _drawTime.Start();

                _controlDelegate.Invalidate();
                _miniMapDelegate.Invalidate();

                _drawTime.Stop();

                _fps.Update();

                await Task.Delay(16).ConfigureAwait(true);
            }
        }

        public void Save()
        {
            _gameStorage.Write(_trackLayout);
        }

        public void Redraw(Size newSize)
        {
            if (Math.Abs(newSize.Width - _lastSize.Width) >= 20 ||
                Math.Abs(newSize.Height - _lastSize.Height) >= 20)
            {
                _lastSize = newSize;
                ViewPropertyChanged(ResetPropertyString, null);
            }
        }

        private static View CreateCommandControls(IEnumerable<ICommand> commands)
        {
            var controlsGroup = new VStack();
            foreach (ICommand cmd in commands)
            {
                controlsGroup.Add(new Button(cmd.Name, () => cmd.Execute()));
            }

            return controlsGroup;
        }

        private static View CreateToolsControls(IEnumerable<ITool> tools, TrainsDelegate controlDelegate, bool buildMode)
        {
            var controlsGroup = new RadioGroup(Orientation.Vertical);
            foreach (ITool tool in tools)
            {
                if (ShouldShowTool(buildMode, tool))
                {
                    if (controlDelegate.CurrentTool.Value == null)
                    {
                        controlDelegate.CurrentTool.Value = tool;
                    }

                    controlsGroup.Add(new RadioButton(() => tool.Name, () => controlDelegate.CurrentTool.Value == tool, () => controlDelegate.CurrentTool.Value = tool));
                }
            }

            return controlsGroup;
        }

        private static bool ShouldShowTool(bool buildMode, ITool tool)
            => (buildMode, tool.Mode) switch
            {
                (true, ToolMode.Build) => true,
                (false, ToolMode.Play) => true,
                (_, ToolMode.All) => true,
                _ => false
            };

        private static View CreateConfigurationControls(IEnumerable<ILayerRenderer> layers)
        {
            var layersGroup = new VStack();
            foreach (ILayerRenderer layer in layers)
            {
                layersGroup.Add(new ToggleButton(layer.Name, layer.Enabled, () => layer.Enabled = !layer.Enabled));
            }
            return layersGroup;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _presenting = false;
                _game.Dispose();
                _miniMapDelegate.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
