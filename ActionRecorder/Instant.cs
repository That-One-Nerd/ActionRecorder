using UnityEngine;

namespace ActionRecorder
{
    public abstract class Instant<TComponent>
        where TComponent : Component
    {
        public double Time { get; internal set; }
    }
}
