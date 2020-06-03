using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Comet;
using Trains.NET.Engine;
using Trains.NET.Rendering;

namespace Trains.NET.Comet
{
    public class MainPage : View
    {
        private readonly State<bool> _configurationShown = false;

        private readonly Timer _timer;

        public MainPage(IGame game,
                        IPixelMapper pixelMapper,
                        ITrackParameters trackParameters,
                        OrderedList<ITool> tools,
                        OrderedList<ILayerRenderer> layers,
                        OrderedList<ICommand> commands,
                        ITrainController trainControls)
        {
            this.Title("Train.NET - " + ThisAssembly.AssemblyInformationalVersion);

            var controlDelegate = new TrainsDelegate(game, pixelMapper);

            this.Body = () =>
            {
                return new HStack()
                {
                    new VStack()
                    {
                        new ToggleButton("Configuration", _configurationShown, ()=> _configurationShown.Value = !_configurationShown.Value),
                        new Spacer(),
                        _configurationShown ?
                             CreateConfigurationControls(trackParameters, layers) :
                             CreateToolsControls(tools, controlDelegate),
                        new Spacer(),
                        _configurationShown ? null :
                            CreateCommandControls(commands),
                        new Spacer()
                    }.Frame(100, alignment: Alignment.Top),
                    new VStack()
                    {
                        new TrainControllerPanel(trainControls),
                        new DrawableControl(controlDelegate).FillVertical()
                    }
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

        private static View CreateCommandControls(IEnumerable<ICommand> commands)
        {
            var controlsGroup = new VStack();
            foreach (ICommand cmd in commands)
            {
                controlsGroup.Add(new Button(cmd.Name, () => cmd.Execute()));
            }

            return controlsGroup;
        }

        private static View CreateToolsControls(IEnumerable<ITool> tools, TrainsDelegate controlDelegate)
        {
            var controlsGroup = new RadioGroup(Orientation.Vertical);
            foreach (ITool tool in tools)
            {
                controlsGroup.Add(new RadioButton(() => tool.Name, () => controlDelegate.CurrentTool.Value == tool, () => controlDelegate.CurrentTool.Value = tool));
            }

            return controlsGroup;
        }

        private static View CreateConfigurationControls(ITrackParameters trackParameters, IEnumerable<ILayerRenderer> layers)
        {
            var layersGroup = new VStack();
            foreach (ILayerRenderer layer in layers)
            {
                layersGroup.Add(new ToggleButton(layer.Name, layer.Enabled, () => layer.Enabled = !layer.Enabled));
            }
#pragma warning disable CA2000 // Dispose objects before losing scope
            layersGroup.Add(new VStack()
                            {
                                GetConfigurationControl(trackParameters, nameof(trackParameters.CellSize)),
                                GetConfigurationControl(trackParameters, nameof(trackParameters.NumPlanks)),
                                GetConfigurationControl(trackParameters, nameof(trackParameters.NumCornerPlanks)),
                                GetConfigurationControl(trackParameters, nameof(trackParameters.PlankWidth)),
                                GetConfigurationControl(trackParameters, nameof(trackParameters.PlankPadding)),
                                GetConfigurationControl(trackParameters, nameof(trackParameters.TrackPadding)),
                                GetConfigurationControl(trackParameters, nameof(trackParameters.TrackWidth)),
                                GetConfigurationControl(trackParameters, nameof(trackParameters.CornerStepDegrees)),
                                GetConfigurationControl(trackParameters, nameof(trackParameters.CornerEdgeOffsetDegrees))
                            }.Margin(top: 50)
#pragma warning restore CA2000 // Dispose objects before losing scope
                        );
            return layersGroup;
        }

        private static View? GetConfigurationControl(ITrackParameters trackParameters, string parameter)
        {
            PropertyInfo? prop = trackParameters.GetType().GetProperty(parameter, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop == null)
            {
                return null;
            }
            return new VStack()
                {
                    new Text(parameter + ":"),
                    new HStack()
                    {
                        new Button("-", () => AdjustProperty(trackParameters, prop, -1)),
                        new Text($"{prop.GetValue(trackParameters)}"),
                        new Button("+", () => AdjustProperty(trackParameters, prop, 1))
                    }
                };
        }

        private static void AdjustProperty(ITrackParameters trackParameters, PropertyInfo prop, int adjustment)
        {
            object? value = prop.GetValue(trackParameters);
            if (value is int intValue)
            {
                prop.SetValue(trackParameters, intValue + adjustment);
            }
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
