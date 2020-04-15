using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Trains.NET.Engine;
using Trains.NET.Rendering;
using Trains.NET.Rendering.HtmlCanvas;

namespace Trains.NET.Web
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddBaseAddressHttpClient();

            builder.Services.AddFromAssembly(typeof(IGameBoard).Assembly);
            builder.Services.AddFromAssembly(typeof(IGame).Assembly);
            builder.Services.AddFromAssembly(typeof(Program).Assembly);
            builder.Services.AddFromAssembly(typeof(CanvasWrapper).Assembly);

            builder.Services.AddSingleton(typeof(GameState));

            await builder.Build().RunAsync().ConfigureAwait(false);
        }
    }
}
