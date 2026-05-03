using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ActionRecorder
{
    public static class ActionManager
    {
        #region Public Declarations

        // Start multiple recorders at once.
        public static void RecordStartAll() => RecordStartAll(obj => true);
        public static void RecordStartAll(string tag) => RecordStartAll(obj => obj.CompareTag(tag));
        public static void RecordStartAll(Func<ObjectRecorder, bool> selection)
        {
            foreach (ObjectRecorder obj in objects.Values.Where(selection)) obj.RecordStart();
        }

        #endregion

        #region Object Management

        private static readonly Dictionary<string, ObjectRecorder> objects = new Dictionary<string, ObjectRecorder>();
        internal static bool IdExists(string id) => objects.ContainsKey(id);
        internal static void DeclareObject(ObjectRecorder recorder)
        {
            if (objects.ContainsKey(recorder.Id)) throw new Exception($"An {nameof(ObjectRecorder)} with ID \"{recorder.Id}\" already exists.");
            else if (objects.ContainsValue(recorder)) throw new Exception($"This {nameof(ObjectRecorder)} instance is already declared.");

            objects.Add(recorder.Id, recorder);
        }
        internal static void UndeclareObject(ObjectRecorder recorder) => objects.Remove(recorder.Id);

        #endregion

        #region Assembly Awareness and Type Discovery

        private static readonly List<Assembly> toSearch = new List<Assembly>();
        static ActionManager()
        {
            // Discover types based on automatic assembly searches.
            AddAssembly(Assembly.GetCallingAssembly(), false);
            AddAssembly(Assembly.GetEntryAssembly(), false);
            AddAssembly(Assembly.GetExecutingAssembly(), false);

            RediscoverTypes(false);
        }

        public static void AddAssembly(Type type) => AddAssembly(Assembly.GetAssembly(type));
        public static void AddAssembly(Assembly assembly) => AddAssembly(assembly, true);
        public static void AddAssembly(Assembly assembly, bool discover)
        {
            if (assembly is null || toSearch.Contains(assembly)) return;
            
            toSearch.Add(assembly);
            if (discover) RediscoverTypes(false);
        }

        private static readonly Dictionary<Type, Type> recorders = new Dictionary<Type, Type>();
        private static readonly Dictionary<Type, Type> instants = new Dictionary<Type, Type>();

        public static void RediscoverTypes() => RediscoverTypes(false);
        public static void RediscoverTypes(bool clear)
        {
            if (clear)
            {
                recorders.Clear();
                instants.Clear();
            }

            foreach (Assembly asm in toSearch)
            {
                var possible = from t in asm.GetTypes()
                               where t.IsClass && !t.IsAbstract && !t.ContainsGenericParameters
                               select t;

                // Discover instants
                foreach ((Type instant, Type baseType) in from t in possible
                                                          let baseType = derivesFrom(t, typeof(Instant<>))
                                                          where baseType != null
                                                          select (t, baseType))
                {
                    Type componentType = baseType.GenericTypeArguments[0];

                    if (instants.TryGetValue(componentType, out Type old))
                    {
                        if (old == instant) continue; // Already discovered.
                        else throw new Exception($"More than one instant definition exists for {componentType}!");
                    }
                    instants.Add(componentType, instant);
                }

                // Discover recorders.
                foreach ((Type recorder, Type baseType) in from t in possible
                                                           let baseType = derivesFrom(t, typeof(ComponentRecorder<,>))
                                                           where baseType != null
                                                           select (t, baseType))
                {
                    Type componentType = baseType.GenericTypeArguments[0];
                    Type instantType = baseType.GenericTypeArguments[1];

                    if (recorders.TryGetValue(componentType, out Type old))
                    {
                        if (old == recorder) continue; // Already discovered.
                        else throw new Exception($"More than one recorder definition exists for {componentType}!");
                    }
                    recorders.Add(componentType, recorder);
                }
            }

            Type derivesFrom(Type reference, Type baseClass)
            {
                // BaseType only goes one layer deep, so this function
                // iteratively checks parents until it either hits null
                // or the baseClass.

                // Also, this function does NOT check for generics. That's
                // intentional for this use case.

                while (reference.BaseType != null)
                {
                    reference = reference.BaseType;
                    Type compare = reference.IsGenericType ? reference.GetGenericTypeDefinition() : reference;
                    if (compare == baseClass) return reference;
                }
                return null;
            }
        }

        #endregion

        #region Recorders/Players/Instant Types

        internal static IComponentRecorder CreateRecorder(Component component)
        {
            // We expect this method to be called at the correct times.
            // That is, we assume that when this method is called, no other recorder
            // has already been instantiated for this particular component instance.
            if (!recorders.TryGetValue(component.GetType(), out Type recorder)) return null; // No known recorder type found.
            else return (IComponentRecorder)Activator.CreateInstance(recorder);
        }

        #endregion
    }
}
