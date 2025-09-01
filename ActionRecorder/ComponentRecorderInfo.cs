using System;

namespace ActionRecorder
{
    public class ComponentRecorderInfo
    {
        public Type ComponentType { get; internal set; }
        public Type RecorderType { get; internal set; }
        public Type InstantType { get; internal set; }
        public string InstantDisplayName { get; internal set; }

        internal ComponentRecorderInfo() { }
    }
}
