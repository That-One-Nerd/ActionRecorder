using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using UnityEngine;
using DisplayNameAttribute = System.ComponentModel.DisplayNameAttribute;

namespace ActionRecorder
{
    public static class ActionManager
    {
        private static readonly List<ComponentRecorderInfo> recorderTypes;
        private static readonly List<ComponentRecorderBase> activeRecorders;

        private static readonly Dictionary<string, (ObjectRecorder recorder, object playback)> registeredIds;
        private static readonly Dictionary<(string id, string recordname), RecordingContainer> recordingData;

        static ActionManager()
        {
            Assembly[] toSearch = new Assembly[]
            {
                Assembly.GetEntryAssembly(),
                Assembly.GetExecutingAssembly()
            };

            recorderTypes = new List<ComponentRecorderInfo>();
            foreach (Assembly asm in toSearch)
            {
                if (asm is null) continue;
                foreach (Type t in asm.GetTypes())
                {
                    if (recorderTypes.Any(x => x.RecorderType == t)) continue;

                    ComponentRecorderAttribute att = t.GetCustomAttribute<ComponentRecorderAttribute>();
                    if (att is null || !t.HasBaseType<ComponentRecorderBase>()) continue;

                    // Try to get the instant name. If there isn't one, use the name
                    // of the recorder, minus the word "recorder"
                    string instantName;
                    DisplayNameAttribute instantNameAtt = att.instantType.GetCustomAttribute<DisplayNameAttribute>();
                    if (instantNameAtt is null)
                    {
                        instantName = t.Name.Replace("Recorder", "");
                    }
                    else instantName = instantNameAtt.DisplayName;

                    recorderTypes.Add(new ComponentRecorderInfo()
                    {
                        ComponentType = att.componentType,
                        RecorderType = t,
                        InstantType = att.instantType,
                        InstantDisplayName = instantName
                    });
                }
            }

            activeRecorders = new List<ComponentRecorderBase>();
            registeredIds = new Dictionary<string, (ObjectRecorder recorder, object playback)>();
            recordingData = new Dictionary<(string, string), RecordingContainer>();
        }

        public static ComponentRecorderInfo GetRecorderInfo(Type recorderType)
        {
            return recorderTypes.SingleOrDefault(x => x.RecorderType == recorderType);
        }

        public static bool IdTaken(string id) => registeredIds.ContainsKey(id);
        public static void RegisterController(ObjectRecorder recorder)
        {
            if (registeredIds.ContainsKey(recorder.Id))
            {
                (ObjectRecorder, object) current = registeredIds[recorder.Id];
                current.Item1 = recorder;
                registeredIds[recorder.Id] = current;
            }
            else registeredIds.Add(recorder.Id, (recorder, null));

            recorder.containers = new RecordingContainer[recorder.ComponentsToRecord.Length];
            for (int i = 0; i < recorder.containers.Length; i++)
            {
                ComponentRecorderInfo info = GetRecorderInfo(recorder.recorders[i].GetType());
                if (!recordingData.TryGetValue((recorder.Id, info.InstantDisplayName), out RecordingContainer data))
                {
                    data = new RecordingContainer(info.InstantDisplayName);
                    recordingData.Add((recorder.Id, info.InstantDisplayName), data);
                }
                recorder.containers[i] = data;
            }
        }

        public static bool HasRecorderTypeFor(Component component)
        {
            return recorderTypes.Any(x => x.ComponentType == component.GetType());
        }
        public static ComponentRecorderBase GetRecorderFor(string id, Component component)
        {
            Type comType = component.GetType();

            // First check if a recorder with this ID already exists.
            ComponentRecorderBase result = activeRecorders.SingleOrDefault(x =>
                x.Controller.Id == id &&
                GetComponentTypeOfRecorder(x) == comType);
            if (result != null) return result;

            // Otherwise make a new instance.
            ComponentRecorderInfo recInfo = recorderTypes.SingleOrDefault(x => x.ComponentType == comType);
            if (recInfo is null) return null;
            result = (ComponentRecorderBase)Activator.CreateInstance(recInfo.RecorderType);
            activeRecorders.Add(result);

            return result;
        }

        public static Stream GetRecordingAsBinary(string id)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms, Encoding.UTF8, true);
            WriteRecordingAsBinary(id, writer);
            writer.Close();
            return ms;
        }
        public static void WriteRecordingAsBinary(string id, BinaryWriter writer)
        {
            writer.Write(id);
            IEnumerable<RecordingContainer> data = from c in recordingData
                                                   where c.Key.id == id
                                                   select c.Value;
            foreach (RecordingContainer container in data)
            {
                writer.Write(container.DisplayName);
                container.EncodeBinary(writer);
            }
        }
        public static string GetRecordingAsXml(string id, bool indent = true)
        {
            StringBuilder result = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            if (indent)
            {
                settings.Indent = true;
                settings.IndentChars = "  ";
            }
            XmlWriter writer = XmlWriter.Create(result, settings);
            WriteRecordingAsXml(id, writer);
            writer.Close();
            return result.ToString();
        }
        public static void WriteRecordingAsXml(string id, XmlWriter writer)
        {
            writer.WriteStartElement("Object");
            writer.WriteAttributeString("Identifier", id);
            IEnumerable<RecordingContainer> data = from c in recordingData
                                                   where c.Key.id == id
                                                   select c.Value;
            foreach (RecordingContainer container in data)
            {
                writer.WriteStartElement("Component");
                writer.WriteAttributeString("Identifier", container.DisplayName);
                container.EncodeXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        internal static Type GetComponentTypeOfRecorder(ComponentRecorderBase instance) =>
            recorderTypes.Single(x => x.RecorderType == instance.GetType()).ComponentType;
    }
}
