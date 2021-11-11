using BlazingServerTrains.Services;
using Microsoft.AspNetCore.Components;

namespace BlazingServerTrains.Pages;

public partial class Index
{
    [Inject]
    public SharedMemory SharedMemory { get; set; }

    public string TrainNames
    {
        get => this.SharedMemory!.Memory.FirstOrDefault(x => x.key == "TrainNames").value;
        set
        {
            var exists = this.SharedMemory!.Memory.FirstOrDefault(x => x.key == nameof(this.TrainNames));
            if(exists.key != null)
                this.SharedMemory!.Memory.Remove(exists);
            this.SharedMemory?.Memory.Add((nameof(this.TrainNames), value));
        }
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await  base.OnAfterRenderAsync(firstRender);
        if(firstRender)
        {
            this.SharedMemory!.Memory.CollectionChanged += Memory_CollectionChanged;
        }
    }

    private void Memory_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);

        //if(e.NewItems?.Count > 0)
        //{
        //    foreach(var item in e.NewItems)
        //    {
        //        if(item is Tuple<string, string> memoryItem )
        //        {
        //            if (memoryItem.Item1 == "TrainNames")
        //            {
        //                StateHasChanged();
        //            }
        //        }
        //    }
        //}
    }
}
