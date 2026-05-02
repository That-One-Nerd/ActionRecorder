using UnityEngine;

namespace ActionRecorder
{
    public abstract class ComponentRecorder<TComponent, TInstant>
        where TComponent : Component
        where TInstant : Instant<TComponent>
    {
        public abstract FrequencyKind Frequency { get; }

        public virtual void OnRecordStart() { }
        public virtual void OnRecordStop() { }
        public abstract TInstant RecordInstant(TComponent component);
    }
}
