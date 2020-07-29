using System.Collections.Generic;
using System.Linq;
using Trains.NET.Engine;

namespace Trains.NET.Tests
{
    internal class NullStorage : IGameStorage
    {
        public IEnumerable<IStaticEntity> Read()
        {
            return Enumerable.Empty<IStaticEntity>();
        }

        public void Write(IEnumerable<IStaticEntity> entities)
        {
        }
    }
}
