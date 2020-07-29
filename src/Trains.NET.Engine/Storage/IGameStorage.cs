using System.Collections.Generic;

namespace Trains.NET.Engine
{
    public interface IGameStorage
    {
        IEnumerable<IStaticEntity> Read();

        void Write(IEnumerable<IStaticEntity> entities);
    }
}
