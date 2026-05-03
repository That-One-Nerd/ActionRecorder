using System;
using System.Collections.Generic;
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

        private readonly List<IComponentRecorder> recorders = new List<IComponentRecorder>();

        private void Init()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Debug.LogError("Object recorder missing a unique identifier.");
                Id = DefaultId();
            }
            else if (ActionManager.IdExists(Id))
            {
                Debug.LogError($"Object recorder ID \"{Id}\" must be unique.");
                Id = DefaultId();
            }

            ActionManager.DeclareObject(this);

            // Go through each component listed. If it can instantiate a
            // recorder for that type, then we're good! Otherwise remove it
            // from the list.
            List<Component> valid = new List<Component>();
            foreach (Component c in ComponentsToRecord)
            {
                if (c.gameObject != gameObject)
                {
                    Debug.LogError($"{c} does not belong to this game object. Please use a separate object recorder.");
                    continue;
                }
                else if (valid.Contains(c))
                {
                    Debug.LogError($"Duplicate instance of {c} in {nameof(ComponentsToRecord)}.");
                    continue;
                }

                IComponentRecorder recorder = ActionManager.CreateRecorder(c);
                if (recorder is null)
                {
                    // Will only occur in two contexts:
                    // 1. There's no supported recorder for this type. Most likely option.
                    // 2. The recorder type is in an *additional* imported Unity assembly,
                    //    and the user forgot (or didn't know) to call ActionManager.AddAssembly.
                    Debug.LogError($"Cannot create a recorder for {c}! Most likely, no recorder for this component is supported.\n" +
                                   $"If you are sure one exists, make sure to call {nameof(ActionManager.AddAssembly)} in an Awake() method.");
                    continue;
                }

                recorders.Add(recorder);
                valid.Add(c);
            }

            ComponentsToRecord = valid.ToArray();

            // We should be good to go!
        }

        private void Start()
        {
            Init();
            if (AutoRecord) RecordStart();
        }

        private void Update()
        {
            if (!Recording) return;

            // Recorders with FrequencyKind.Update get invoked here.
            for (int i = 0; i < recorders.Count; i++)
            {
                IComponentRecorder r = recorders[i];
                if (r.Frequency == FrequencyKind.Update) RecordComponent(i);
            }
        }
        private void FixedUpdate()
        {
            if (!Recording) return;

            // Recorders with FrequencyKind.FixedUpdate get invoked here.
            for (int i = 0; i < recorders.Count; i++)
            {
                IComponentRecorder r = recorders[i];
                if (r.Frequency == FrequencyKind.FixedUpdate) RecordComponent(i);
            }
        }

        public void RecordStart()
        {
            if (Recording) return;
            Recording = true;
            for (int i = 0; i < recorders.Count; i++) recorders[i].OnRecordStart();
        }
        public void RecordStop()
        {
            if (!Recording) return;
            Recording = false;
            for (int i = 0; i < recorders.Count; i++) recorders[i].OnRecordStop();
        }

        public void RecordInstant()
        {
            if (!Recording)
            {
                Debug.LogError($"Please make sure a recording has started before calling {nameof(RecordInstant)}");
                return;
            }

            // The user initiated a RecordInstant(), so record all components
            // now, regardless of their frequencies.
            for (int i = 0; i < recorders.Count; i++) RecordComponent(i);
        }

        private void RecordComponent(int index)
        {
            // TODO: Actually save the instants somewhere.
            recorders[index].RecordInstant(ComponentsToRecord[index]);
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
