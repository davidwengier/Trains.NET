using System.Collections.Generic;

namespace Trains.NET.Engine
{
    public interface ITrackCodec
    {
        string Encode(IEnumerable<IStaticEntity> entities);

        IEnumerable<IStaticEntity> Decode(string input);
    }
}
