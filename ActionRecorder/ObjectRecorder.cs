using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

namespace ActionRecorder
{
    public class ObjectRecorder : MonoBehaviour
    {
        public bool AutoRecord;
        public string Id;
        public Component[] ComponentsToRecord;

        public bool Recording { get; private set; }

        internal ComponentRecorderBase[] recorders;
        internal RecordingContainer[] containers;

        private void Awake()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Debug.LogWarning("You should always give your recorders a unique identifier!");
                Id = GetRandomId();
            }
            else if (ActionManager.IdTaken(Id))
            {
                Debug.LogWarning($"A recorder with the ID \"{Id}\" already exists! Please choose a different unique identifier.");
                Id = GetRandomId();
            }

            // Go through the list of components to record, ensure they are
            // components belonging to this object, and initialize recorders
            // for those components.

            List<Component> valid = new List<Component>();
            foreach (Component c in ComponentsToRecord)
            {
                if (c is null) continue;
                if (c.gameObject != gameObject)
                {
                    Debug.LogWarning($"The component {c} does not belong to this game object and thus cannot be recorded!");
                    continue;
                }
                else if (!ActionManager.HasRecorderTypeFor(c))
                {
                    Debug.LogError($"No recognized recorder type for {c}! This component cannot be recorded.");
                    continue;
                }
                valid.Add(c);
            }
            ComponentsToRecord = valid.ToArray();

            recorders = new ComponentRecorderBase[ComponentsToRecord.Length];
            for (int i = 0; i < recorders.Length; i++)
            {
                ComponentRecorderBase rec = ActionManager.GetRecorderFor(Id, ComponentsToRecord[i]);
                rec.Controller = this;
                rec.Component = ComponentsToRecord[i];
                recorders[i] = rec;
            }
            ActionManager.RegisterController(this);

            OnAwake();
            for (int i = 0; i < recorders.Length; i++) recorders[i].Awake();
        }
        private void Start()
        {
            if (AutoRecord) RecordStart();

            OnStart();
            for (int i = 0; i < recorders.Length; i++) recorders[i].Start();
        }

        protected virtual void OnAwake() { }
        protected virtual void OnStart() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnFixedUpdate() { }
        protected virtual void BeginRecording() { }
        protected virtual void EndRecording() { }
        public event Action OnBeginRecording = delegate { };
        public event Action OnEndRecording = delegate { };

        private void Update()
        {
            OnUpdate();
            if (!Recording) return;

            for (int i = 0; i < recorders.Length; i++)
            {
                ComponentRecorderBase rec = recorders[i];
                if (rec.RecordFrequency != FrequencyKind.Update) continue;
                RecordingInstantBase instant = rec.RecordInstant();
                containers[i].Add(instant);
            }
        }
        private void FixedUpdate()
        {
            OnFixedUpdate();
            if (!Recording) return;

            for (int i = 0; i < recorders.Length; i++)
            {
                ComponentRecorderBase rec = recorders[i];
                if (rec.RecordFrequency != FrequencyKind.FixedUpdate) continue;
                RecordingInstantBase instant = rec.RecordInstant();
                containers[i].Add(instant);
            }
        }

        public void RecordStart()
        {
            if (Recording) return;
            Recording = true;
            BeginRecording();
            OnBeginRecording();
            for (int i = 0; i < recorders.Length; i++) recorders[i].OnBeginRecording();
        }
        public void RecordStop()
        {
            if (!Recording) return;
            Recording = false;
            EndRecording();
            OnEndRecording();
            for (int i = 0; i < recorders.Length; i++) recorders[i].OnEndRecording();
        }

        private static string GetRandomId()
        {
            StringBuilder id = new StringBuilder("CHANGEME-");
            System.Random rand = new System.Random();
            for (int i = 0; i < 8; i++) id.Append((char)('A' + rand.Next(26)));
            return id.ToString();
        }
    }
}
