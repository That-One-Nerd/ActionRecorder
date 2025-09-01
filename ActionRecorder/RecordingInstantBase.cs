using System.IO;
using System.Xml;
using UnityEngine;

namespace ActionRecorder
{
    public abstract class RecordingInstantBase
    {
        public float Time { get; set; }

        public abstract void EncodeBinary(BinaryWriter writer);
        public abstract void EncodeXml(XmlWriter writer);

        protected void WriteProperty(XmlWriter writer, string name, object value)
        {
            writer.WriteStartElement("Property");
            writer.WriteAttributeString("Name", name);
            writer.WriteValue(value?.ToString() ?? "");
            writer.WriteEndElement();
        }
        protected void WriteProperty(XmlWriter writer, string name, Vector2 value)
        {
            writer.WriteStartElement("Property");
            writer.WriteAttributeString("Name", name);

            WriteProperty(writer, "x", value.x);
            WriteProperty(writer, "y", value.y);

            writer.WriteEndElement();
        }
        protected void WriteProperty(XmlWriter writer, string name, Vector3 value)
        {
            writer.WriteStartElement("Property");
            writer.WriteAttributeString("Name", name);

            WriteProperty(writer, "x", value.x);
            WriteProperty(writer, "y", value.y);
            WriteProperty(writer, "z", value.z);

            writer.WriteEndElement();
        }
        protected void WriteProperty(XmlWriter writer, string name, Vector4 value)
        {
            writer.WriteStartElement("Property");
            writer.WriteAttributeString("Name", name);

            WriteProperty(writer, "x", value.x);
            WriteProperty(writer, "y", value.y);
            WriteProperty(writer, "z", value.z);
            WriteProperty(writer, "w", value.w);

            writer.WriteEndElement();
        }
    }
}
