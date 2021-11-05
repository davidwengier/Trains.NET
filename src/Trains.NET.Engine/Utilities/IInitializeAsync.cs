using System.Threading.Tasks;

namespace Trains.NET.Engine;

public interface IInitializeAsync
{
    Task InitializeAsync(int columns, int rows);
}
