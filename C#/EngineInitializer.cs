using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace Raymarcher
{
    /// <summary>
    /// Enables the engine to execute a method when the engine starts.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EngineInitializer : Attribute, IComparable, IComparable<EngineInitializer>
    {
        public static Random r = new Random();
        private class Dummy { }

        /// <summary>
        /// The execution priority of the method. Higher the value is, later it is called.
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// The ID is used to distinguate two different initializer of the same priority.
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// Enables the engine to execute a "Initialize" method when it starts.
        /// </summary>
        /// <param name="method">The method triggered when starting.</param>
        public EngineInitializer(int priority)
        {
            this.Priority = priority;
            this.ID = (r.Next(int.MinValue, int.MaxValue - 1)).ToString().GetHashCode().ToString();
        }
        /// <summary>
        /// Triggers all the initialization methods.
        /// </summary>
        public static void Fire()
        {
            MethodInfo[] methods = Assembly.GetExecutingAssembly().GetTypes()
                      .SelectMany(t => t.GetMethods())
                      .Where(m => m.GetCustomAttributes(typeof(EngineInitializer), false).Length > 0)
                      .ToArray();

            SortedDictionary<EngineInitializer, MethodInfo> initmethod = new SortedDictionary<EngineInitializer, MethodInfo>();
            for (int i = 0; i < methods.Length; i++)
            {
                initmethod.Add((EngineInitializer)methods[i].GetCustomAttribute(typeof(EngineInitializer), false), methods[i]);
            }

            foreach(KeyValuePair<EngineInitializer, MethodInfo> init in initmethod)
            {
                Log.Print("Invoking " + init.Value.Name + " of " + init.Value.ReflectedType.Name + " (priority " + init.Key.Priority + ")");
                init.Value.Invoke(new Dummy(), null);
            }
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as EngineInitializer);
        }
        public int CompareTo(EngineInitializer obj)
        {
            if (obj == null) return 1;

            if (obj.Priority > this.Priority) return -1;
            if (obj.Priority < this.Priority) return 1;

            return 0;
        }
    }

    
}
