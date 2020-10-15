using System;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using SkiaSharp.Views.Mac;
using Trains.NET.Instrumentation;
using Trains.NET.Rendering;
using Trains.NET.Rendering.Skia;

namespace MacTrains
{
    public partial class ViewController : NSViewController
    {
        private readonly PerSecondTimedStat _fps = InstrumentationBag.Add<PerSecondTimedStat>("SKCanvasView-FPS");
        private readonly ElapsedMillisecondsTimedStat _drawTime = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("SKCanvasView-DrawTime");

        private readonly IGame _game;
        private readonly IInteractionManager _interactionManager;

        public ViewController(IntPtr handle)
            : base(handle)
        {
            _game = DI.ServiceLocator.GetService<IGame>();
            _interactionManager = DI.ServiceLocator.GetService<IInteractionManager>();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.canvasView.IgnorePixelScaling = true;
            this.canvasView.PaintSurface += OnPainting;
        }

        public override void MouseDown(NSEvent theEvent)
        {
            _interactionManager.PointerClick((int)theEvent.LocationInWindow.X, (int)this.View.Frame.Height - (int)theEvent.LocationInWindow.Y);
        }

        public override void ScrollWheel(NSEvent theEvent)
        {
            if (theEvent.ScrollingDeltaY > 0)
            {
                _interactionManager.PointerZoomIn((int)theEvent.LocationInWindow.X, (int)this.View.Frame.Height - (int)theEvent.LocationInWindow.Y);
            }
            else
            {
                _interactionManager.PointerZoomOut((int)theEvent.LocationInWindow.X, (int)this.View.Frame.Height - (int)theEvent.LocationInWindow.Y);
            }
        }

        public override void MouseMoved(NSEvent theEvent)
        {
            _interactionManager.PointerMove((int)theEvent.LocationInWindow.X, (int)this.View.Frame.Height - (int)theEvent.LocationInWindow.Y);
        }

        public override void MouseDragged(NSEvent theEvent)
        {
            _interactionManager.PointerDrag((int)theEvent.LocationInWindow.X, (int)this.View.Frame.Height - (int)theEvent.LocationInWindow.Y);
        }

        public override void MouseUp(NSEvent theEvent)
        {
            _interactionManager.PointerRelease((int)theEvent.LocationInWindow.X, (int)this.View.Frame.Height - (int)theEvent.LocationInWindow.Y);
        }

        public override void ViewDidLayout()
        {
            _game.SetSize((int)this.View.Frame.Width, (int)this.View.Frame.Height);
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            using var canvas = new SKCanvasWrapper(e.Surface.Canvas);
            _game.Render(canvas);
        }

        public override void ViewWillAppear()
        {
            base.ViewWillAppear();

            this.View.Window.Title = "Trains - " + ThisAssembly.AssemblyInformationalVersion;
            this.View.Window.AcceptsMouseMovedEvents = true;

            _ = PresentLoop();
        }

        private async Task PresentLoop()
        {
            while (true)
            {
                Invalidate();

                await Task.Delay(16).ConfigureAwait(true);
            }
        }

        private void Invalidate()
        {
            using (_drawTime.Measure())
            {
                this.canvasView.NeedsDisplay = true;
            }
            _drawTime.Stop();

            _fps.Update();
        }
    }
}
