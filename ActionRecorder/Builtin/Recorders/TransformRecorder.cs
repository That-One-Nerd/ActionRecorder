using ActionRecorder.Builtin.Instants;
using UnityEngine;

namespace ActionRecorder.Builtin.Recorders
{
    internal class TransformRecorder : ComponentRecorder<Transform, TransformInstant>
    {
        public override FrequencyKind Frequency => FrequencyKind.Update;

        public TransformRecorder()
        {
            Debug.Log("Hello!");
        }

        public override TransformInstant RecordInstant(Transform transform)
        {
            Debug.Log("Caught in 4K");

            return new TransformInstant(
                transform.localPosition,
                transform.localRotation,
                transform.localScale
            );
        }
    }
}
