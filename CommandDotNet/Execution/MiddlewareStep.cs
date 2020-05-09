namespace CommandDotNet.Execution
{
    public class MiddlewareStep
    {
        public static MiddlewareStep operator +(MiddlewareStep step, short increment) => 
            new MiddlewareStep(step.Stage, (short)(step.OrderWithinStage + increment));
        public static MiddlewareStep operator -(MiddlewareStep step, short decrement) =>
            new MiddlewareStep(step.Stage, (short)(step.OrderWithinStage - decrement));

        public MiddlewareStages Stage { get; }

        public short OrderWithinStage { get; }

        public MiddlewareStep(MiddlewareStages stage, short? orderWithinStage = null)
        {
            Stage = stage;
            OrderWithinStage = orderWithinStage.GetValueOrDefault();
        }

        public override string ToString()
        {
            return $"{nameof(MiddlewareStep)}:{Stage} {OrderWithinStage}";
        }

        protected bool Equals(MiddlewareStep other)
        {
            return Stage == other.Stage
                   && OrderWithinStage == other.OrderWithinStage;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((MiddlewareStep)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Stage * 397) ^ OrderWithinStage.GetHashCode();
            }
        }

        public static bool operator ==(MiddlewareStep left, MiddlewareStep right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MiddlewareStep left, MiddlewareStep right)
        {
            return !Equals(left, right);
        }
    }
}