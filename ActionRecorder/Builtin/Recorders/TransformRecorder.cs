using ActionRecorder.Builtin.Instants;
using UnityEngine;

namespace ActionRecorder.Builtin.Recorders
{
    internal class TransformRecorder : ComponentRecorder<Transform, TransformInstant>
    {
        public override FrequencyKind Frequency => FrequencyKind.Update;

        public override TransformInstant RecordInstant(Transform transform) => new TransformInstant(
            transform.localPosition,
            transform.localRotation,
            transform.localScale
        );
    }
}
