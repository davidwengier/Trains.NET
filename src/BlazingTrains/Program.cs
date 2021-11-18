using BlazingTrains;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Trains.NET.Engine;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Game>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var host = builder.Build();

// Dodgy!!
var storage = DI.ServiceLocator.GetService<IGameStorage>() as BlazorGameStorage;
if (storage is not null)
{
    storage.AspNetCoreServices = host.Services;
}

await host.RunAsync();
