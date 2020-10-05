using System;

namespace Trains.NET.Engine
{
    public class Tree : IStaticEntity
    {
        private static readonly Random s_random = new Random();

        public int Column { get; set; }
        public int Row { get; set; }

        public int Seed { get; set; }

        public string Identifier => this.Seed.ToString();

        public Tree()
        {
            Refresh(true);
        }

        public void Refresh(bool justAdded)
        {
            this.Seed = s_random.Next(1, 100);
        }

        public void SetOwner(ILayout? collection)
        {
        }
    }
}
