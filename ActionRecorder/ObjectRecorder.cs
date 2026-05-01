using System;
using UnityEngine;

namespace ActionRecorder
{
    [AddComponentMenu("ActionRecorder/ObjectRecorder")]
    public class ObjectRecorder : MonoBehaviour
    {
        public bool AutoRecord;
        public bool GlobalTime = true;
        public string Id;
        public Component[] ComponentsToRecord;

        public bool Recording { get; private set; }

        private void Awake()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Debug.LogError("Object recorder missing a unique identifier.");
                Id = DefaultId();
            }

            // TODO: Create recorders for each component.
        }

        private void Start()
        {
            if (AutoRecord) RecordStart();
        }

        public void RecordStart()
        {
            if (Recording) return;
            Recording = true;
        }
        public void RecordStop()
        {
            if (!Recording) return;
            Recording = false;
        }

        public void RecordInstant()
        {
            if (!Recording)
            {
                Debug.LogError($"Please make sure a recording has started before calling {nameof(RecordInstant)}");
                return;
            }
        }

        private string DefaultId()
        {
            // Only used in situations when the current ID is missing
            // or invalid. Should *always* be a placeholder, but it's good
            // to keep it deterministic when possible.
            return $"CHANGEME-{Math.Abs(gameObject.GetHashCode())}";
        }
    }
}
