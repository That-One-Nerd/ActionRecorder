using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ActionRecorder
{
    public static class ActionManager
    {
        #region Public Declarations

        // Start multiple recorders at once.
        public static void RecordStartAll() => RecordStartAll(obj => true);
        public static void RecordStartAll(string tag) => RecordStartAll(obj => obj.CompareTag(tag));
        public static void RecordStartAll(Func<ObjectRecorder, bool> selection)
        {
            foreach (ObjectRecorder obj in objects.Values.Where(selection)) obj.RecordStart();
        }

        #endregion

        #region Object Management

        private static readonly Dictionary<string, ObjectRecorder> objects = new Dictionary<string, ObjectRecorder>();
        internal static bool IdExists(string id) => objects.ContainsKey(id);
        internal static void DeclareObject(ObjectRecorder recorder)
        {
            if (objects.ContainsKey(recorder.Id)) throw new Exception($"An {nameof(ObjectRecorder)} with ID \"{recorder.Id}\" already exists.");
            else if (objects.ContainsValue(recorder)) throw new Exception($"This {nameof(ObjectRecorder)} instance is already declared.");

            objects.Add(recorder.Id, recorder);
        }
        internal static void UndeclareObject(ObjectRecorder recorder) => objects.Remove(recorder.Id);

        #endregion
    }
}
