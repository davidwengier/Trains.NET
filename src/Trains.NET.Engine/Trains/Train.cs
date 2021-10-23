using System;
using System.ComponentModel;

namespace Trains.NET.Engine
{
    public class Train : IMovable, INotifyPropertyChanged
    {
        private static readonly Random s_random = new();

        // only used for tests??
        internal const float SpeedScaleModifier = 0.005f;

        private const int MaximumSpeed = 200;
        private const float MinimumLookaheadSpeed = 5.0f;
        private const float DefaultSpeed = 20.0f;
        private float? _lookaheadOverride;
        private bool _collisionAhead;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Train()
        {
            this.UniqueID = Guid.NewGuid();
            this.Name = TrainNames.Names[s_random.Next(0, TrainNames.Names.Length)];
            this.Carriages = s_random.Next(0, 6);
            this.DesiredSpeed = DefaultSpeed;
            this.RelativeLeft = 0.5f;
            this.RelativeTop = 0.5f;
        }

        private Train(Train other)
        {
            this.UniqueID = other.UniqueID;
            this.Column = other.Column;
            this.Name = other.Name;
            this.Row = other.Row;
            this.Angle = other.Angle;
            this.RelativeLeft = other.RelativeLeft;
            this.RelativeTop = other.RelativeTop;
            this.CurrentSpeed = other.CurrentSpeed;
            this.DesiredSpeed = other.DesiredSpeed;
            this.Carriages = other.Carriages;
            _lookaheadOverride = other._lookaheadOverride;
        }

        public float LookaheadDistance
        {
            get
            {
                return _lookaheadOverride ?? Math.Max(MinimumLookaheadSpeed, this.CurrentSpeed) * 30;
            }
            set
            {
                _lookaheadOverride = value;
            }
        }

        public virtual Guid UniqueID { get; }

        public int Column { get; set; }
        public int Row { get; set; }
        public float Angle { get; set; }
        public float RelativeLeft { get; set; }
        public float RelativeTop { get; set; }

        public string Name { get; set; }
        public virtual float CurrentSpeed { get; set; }
        public virtual float DesiredSpeed { get; set; }
        public virtual bool Stopped { get; set; }

        public bool Follow { get; set; }

        public int Carriages { get; set; }

        public void SetAngle(float angle)
        {
            while (angle < 0) angle += 360;
            while (angle > 360) angle -= 360;
            this.Angle = angle;
        }

        public Train Clone()
        {
            return new Train(this);
        }

        public void AddCarriage()
        {
            if (this.Carriages < 10)
            {
                this.Carriages += 1;
            }
        }

        public void RemoveCarriage()
        {
            if (this.Carriages > 0)
            {
                this.Carriages -= 1;
            }
        }

        internal void ForceSpeed(float speed)
        {
            this.CurrentSpeed = speed;
            this.DesiredSpeed = speed;
        }

        public void Start() => this.Stopped = false;

        public void Stop() => this.Stopped = true;

        internal void Pause() => _collisionAhead = true;

        internal void Resume() => _collisionAhead = false;

        public void Slower()
        {
            if (this.DesiredSpeed > 5)
            {
                this.DesiredSpeed -= 5;
            }
        }

        public void Faster()
        {
            if (this.DesiredSpeed < MaximumSpeed)
            {
                this.DesiredSpeed += 5;
            }
        }

        public override string ToString() => $"Train {this.UniqueID} [Column: {this.Column} | Row: {this.Row} | Left: {this.RelativeLeft} | Top: {this.RelativeTop} | Angle: {this.Angle} | Speed: {this.CurrentSpeed}]";

        internal TrainPosition GetPosition() => new(this.Column, this.Row, this.RelativeLeft, this.RelativeTop, this.Angle, 0);

        internal void AdjustSpeed()
        {
            if (this.Stopped || _collisionAhead)
            {
                this.CurrentSpeed = Math.Max(this.CurrentSpeed - 1.0f, 0);
            }
            else if (this.DesiredSpeed > this.CurrentSpeed)
            {
                this.CurrentSpeed = Math.Min(this.CurrentSpeed + 1.0f, this.DesiredSpeed);
            }
            else if (this.DesiredSpeed < this.CurrentSpeed)
            {
                this.CurrentSpeed = Math.Max(this.CurrentSpeed - 1.0f, 0);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CurrentSpeed)));
        }

        public void ApplyStep(TrainPosition newPosition)
        {
            this.Column = newPosition.Column;
            this.Row = newPosition.Row;
            this.Angle = newPosition.Angle;
            this.RelativeLeft = newPosition.RelativeLeft;
            this.RelativeTop = newPosition.RelativeTop;
        }
    }
}
