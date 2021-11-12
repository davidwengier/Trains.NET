//This file runs on the server and contains an iframe that runs the whole BlazingTrains App on the client
//This file has one instance per user


using BlazingServerTrains.Services;
using Microsoft.AspNetCore.Components;




namespace BlazingServerTrains.Pages;

public partial class Index
{
    [Inject]
    public SharedMemory SharedMemory { get; set; }

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

    private void Index_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }
}
