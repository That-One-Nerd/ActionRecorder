using System;

namespace ActionRecorder
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentRecorderAttribute : Attribute
    {
        public readonly Type componentType;
        public readonly Type instantType;

        public ComponentRecorderAttribute(Type componentType, Type instantType)
        {
            this.componentType = componentType;
            this.instantType = instantType;
        }
    }
}
