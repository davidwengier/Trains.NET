//This file runs on the server
//It is injected as a singleton, so it is shared between all users

using System.Collections.ObjectModel;
using MvvmHelpers;

namespace BlazingServerTrains.Services;

public class SharedMemory : ObservableObject
{
    volatile string? terrainMap;
    volatile string? iMovableLayout;
    volatile string? pixelMapper;
    volatile string? iLayout;
    volatile string? autoSave;

    public string TerrainMap
    {
        get => terrainMap;
        set => SetProperty(ref terrainMap, value);
    }

    public string IMovableLayout
    {
        get => iMovableLayout;
        set => SetProperty(ref iMovableLayout, value);
    }

    public string PixelMapper
    {
        get => pixelMapper;
        set => SetProperty(ref pixelMapper, value);
    }

    public string ILayout
    {
        get => iLayout;
        set => SetProperty(ref iLayout, value);
    }

    public string AutoSave
    {
        get => autoSave;
        set => SetProperty(ref autoSave, value);

    }

}
