namespace Trains.NET.Engine
{
    public interface IStaticEntity : IEntity
    {
        /// <summary>
        /// A unique identifier for this entity that describes its state, but not its position
        /// </summary>
        string Identifier { get; }
        void SetOwner(ILayout? collection);
        void Refresh(bool justAdded);
    }
}
