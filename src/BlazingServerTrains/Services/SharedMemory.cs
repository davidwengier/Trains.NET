using System.Collections.ObjectModel;

namespace BlazingServerTrains.Services;

public class SharedMemory
{
    public ObservableCollection<(string key, string value)> Memory { get; set; } = new();

}
