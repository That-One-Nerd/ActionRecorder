using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ActionRecorder
{
    public class RecordingContainer
    {
        public string DisplayName { get; }

        // TODO: Replace this with a more sophisticated method.
        //       Allocate using powers of 2, implement IList, etc.
        public readonly List<RecordingInstantBase> instants;

        internal RecordingContainer(string name)
        {
            DisplayName = name;
            instants = new List<RecordingInstantBase>();
        }

        public void Add(RecordingInstantBase instant)
        {
            instants.Add(instant);
        }

        public void EncodeBinary(BinaryWriter writer)
        {
            foreach (RecordingInstantBase instant in instants)
            {
                writer.Write(instant.Time);
                instant.EncodeBinary(writer);
            }
        }
        public void EncodeXml(XmlWriter writer)
        {
            foreach (RecordingInstantBase instant in instants)
            {
                writer.WriteStartElement("Instant");
                writer.WriteAttributeString("Time", instant.Time.ToString());
                instant.EncodeXml(writer);
                writer.WriteEndElement();
            }
        }
    }
}
