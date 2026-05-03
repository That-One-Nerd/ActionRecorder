using UnityEngine;

namespace ActionRecorder
{
    // While the generic types are much easier to use from the perspective
    // of a user of this library, we do not have enough information at
    // compile-time to fill the generic types. So we use these internal
    // interfaces. For all intents and purposes, the functionality of these
    // two should always be identical.
    internal interface IComponentRecorder
    {
        FrequencyKind Frequency { get; }

        void OnRecordStart();
        void OnRecordStop();

        IInstant RecordInstant(Component component);
    }

    public abstract class ComponentRecorder<TComponent, TInstant> : IComponentRecorder
        where TComponent : Component
        where TInstant : Instant<TComponent>
    {
        public abstract FrequencyKind Frequency { get; }

        public virtual void OnRecordStart() { }
        public virtual void OnRecordStop() { }

        public abstract TInstant RecordInstant(TComponent component);
        IInstant IComponentRecorder.RecordInstant(Component component) => RecordInstant((TComponent)component);
    }
}
