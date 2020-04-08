using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Trains.NET.Engine;
using Trains.NET.Rendering;

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

            await builder.Build().RunAsync().ConfigureAwait(false);
        }
    }
}
