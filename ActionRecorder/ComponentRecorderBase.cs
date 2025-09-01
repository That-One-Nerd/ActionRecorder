using System;
using UnityEngine;

namespace ActionRecorder
{
    public abstract class ComponentRecorderBase
    {
        public ObjectRecorder Controller { get; internal set; }
        public Type ComponentType => ActionManager.GetComponentTypeOfRecorder(this);

        public Component Component { get; internal set; }

        public abstract FrequencyKind RecordFrequency { get; }

        public virtual void Awake() { }
        public virtual void Start() { }

        public virtual void OnBeginRecording() { }
        public abstract RecordingInstantBase RecordInstant();
        public virtual void OnEndRecording() { }

        // TODO: Return a type for containing recordings.
    }
}
