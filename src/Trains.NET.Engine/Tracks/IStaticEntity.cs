namespace Trains.NET.Engine
{
    public interface IStaticEntity : IEntity
    {
        void SetOwner(IStaticEntityCollection? collection);
        void Refresh(bool justAdded);
    }
}
