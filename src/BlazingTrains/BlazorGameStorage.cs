using Trains.NET.Engine;

namespace BlazingTrains;

public class BlazorGameStorage : IGameStorage
{
    public IServiceProvider? AspNetCoreServices { get; set; }

    //private ILocalStorageService? LocalStorageService => this.AspNetCoreServices?.GetService<ILocalStorageService>();

    public string? Read(string key)
    {
        return null;
    }

    public void Write(string key, string value)
    {
    }
}
