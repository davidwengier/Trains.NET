namespace Trains.NET.Engine
{
    public interface IStaticEntity : IEntity
    {
        void SetOwner(ILayout? collection);
        void Refresh(bool justAdded);
    }
}
