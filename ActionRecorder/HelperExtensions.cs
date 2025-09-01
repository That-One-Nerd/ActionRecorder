using System;

namespace ActionRecorder
{
    internal static class HelperExtensions
    {
        public static bool HasBaseType(this Type type, Type baseType)
        {
            Type activeType = type.BaseType;
            while (activeType != null)
            {
                if (activeType == baseType) return true;
                else activeType = activeType.BaseType;
            }
            return false;
        }
        public static bool HasBaseType<T>(this Type type) => HasBaseType(type, typeof(T));
    }
}
