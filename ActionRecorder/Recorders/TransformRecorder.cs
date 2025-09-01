using System.ComponentModel;
using System.IO;
using System.Xml;
using UnityEngine;

namespace ActionRecorder.Recorders
{
    [ComponentRecorder(typeof(Transform), typeof(TransformInstant))]
    public class TransformRecorder : ComponentRecorderBase
    {
        public override FrequencyKind RecordFrequency { get; } = FrequencyKind.Update;

        new public Transform Component => (Transform)base.Component;

        public override RecordingInstantBase RecordInstant() => new TransformInstant()
        {
            Time = Time.time,
            localPosition = Component.localPosition,
            localRotation = Component.localRotation.eulerAngles,
            localScale = Component.localScale
        };

        [DisplayName("Transform")]
        public class TransformInstant : RecordingInstantBase
        {
            public Vector3 localPosition, localRotation, localScale;

            public override void EncodeBinary(BinaryWriter writer)
            {
                writer.Write(localPosition.x);
                writer.Write(localPosition.y);
                writer.Write(localPosition.z);
                writer.Write(localRotation.x);
                writer.Write(localRotation.y);
                writer.Write(localRotation.z);
                writer.Write(localScale.x);
                writer.Write(localScale.y);
                writer.Write(localScale.z);
            }
            public override void EncodeXml(XmlWriter writer)
            {
                WriteProperty(writer, nameof(localPosition), localPosition);
                WriteProperty(writer, nameof(localRotation), localRotation);
                WriteProperty(writer, nameof(localScale), localScale);
            }
        }
    }
}
