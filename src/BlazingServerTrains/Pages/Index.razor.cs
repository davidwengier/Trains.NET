//This file runs on the server and contains an iframe that runs the whole BlazingTrains App on the client
//This file has one instance per user


using BlazingServerTrains.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazingServerTrains.Pages;

public partial class Index
{
    [Inject]
    public SharedMemory SharedMemory { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    public string TerrainMap
    {
        get => this.SharedMemory!.TerrainMap;
        set => this.SharedMemory!.TerrainMap = value;
    }

    public string ILayout
    {
        get => this.SharedMemory!.ILayout;
        set => this.SharedMemory!.ILayout = value;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.SharedMemory!.PropertyChanged += Index_PropertyChanged;
    }

    private async void Index_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        await InvokeAsync(StateHasChanged);
        var prop = typeof(SharedMemory).GetProperty(e.PropertyName);
        var val = prop?.GetValue(this.SharedMemory);
       // _ = this.JSRuntime.InvokeVoidAsync("MessageOuterJS", $"TO Outer PropertyChanged - {e.PropertyName}: {val}");
       // _ = this.JSRuntime.InvokeVoidAsync("MessageInnerJS", $"TO Inner PropertyChanged - {e.PropertyName}: {val}");
        _ = this.JSRuntime.InvokeVoidAsync("UpdateProperty", e.PropertyName, val);
    }
}
