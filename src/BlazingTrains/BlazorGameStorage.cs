using Blazored.LocalStorage;
using Trains.NET.Engine;

namespace BlazingTrains;

public class BlazorGameStorage : IGameStorage
{
    public IServiceProvider? AspNetCoreServices { get; set; }

    private ISyncLocalStorageService? SyncLocalStorageService => this.AspNetCoreServices?.GetService<ISyncLocalStorageService>();

    public string? Read(string key)
    {
        var data = this.SyncLocalStorageService?.GetItemAsString(key);
        return data;
    }

    public void Write(string key, string value)
    {
        this.SyncLocalStorageService?.SetItemAsString(key, value);
    }
}
