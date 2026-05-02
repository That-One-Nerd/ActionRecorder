using UnityEngine;

namespace ActionRecorder.Builtin.Instants
{
    internal class TransformInstant : Instant<Transform>
    {
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
        public Vector3 Scale { get; }

        public TransformInstant(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            Position = pos;
            Rotation = rot;
            Scale = scale;
        }
    }
}
