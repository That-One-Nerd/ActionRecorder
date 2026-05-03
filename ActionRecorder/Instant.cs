using UnityEngine;

namespace ActionRecorder
{
    // See comment in ComponentRecorder.
    internal interface IInstant
    {
        double Time { get; }
    }

    public abstract class Instant<TComponent> : IInstant
        where TComponent : Component
    {
        public double Time { get; }
    }
}
