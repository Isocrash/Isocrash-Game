using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Raymarcher
{ 
    public class Element
    {
        
        public Element()
        {
            Sandbox.NextToSpawn.Add(this);
        }

        public T AddModule<T>() where T : Module
        {
            T mod = (T)Activator.CreateInstance(typeof(T));
            Modules.Add(mod);
            return mod;
        }

        public T GetModule<T>() where T : Module
        {
            foreach (Module mod in Modules) if (mod is T module) return module;
            return null;
        }
        public T[] GetModules<T>() where T : Module
        {
            List<T> modules = new List<T>();

            foreach (Module mod in Modules) if (mod is T module) modules.Add(module);

            if (modules.Count == 0) return null;

            return modules.ToArray();
        }
        public T AddOrGetModule<T>() where T : Module
        {
            T module = GetModule<T>();

            if (module) return module;

            return AddModule<T>();
        }

        protected internal List<Module> Modules = new List<Module>();

        public string Name { get; set; } = "Element";

        public Element Parent { get; set; } = null;
        public Vector3D Position { get; set; } = Vector3D.Null;
        public Quaternion Rotation { get; set; }
        public Vector3D Scale { get; set; } = Vector3D.Positive;

        public int Layer { get; set; } = 0;

        internal protected virtual void PreUpdate() { }
        public virtual void Update() { }
        public virtual void PostUpdate() { }
        public virtual void FixedUpdate() { }
        internal protected virtual void EndUpdate() { }

        public virtual void Destroy()
        {
            Sandbox.LoadedElements.Remove(this);
        }

        public static Element Closest(Vector3D position)
        {
            double minDist = Camera.Main.ClipPlanes.y;
            Element minEl = null;

            foreach (Element element in Sandbox.LoadedElements)
            {
                Primitive m = element as Primitive;

                if (m == null) continue;

                double distance = m.DistanceFromSurface(position);

                if (distance < minDist)
                {
                    minEl = element;
                    minDist = distance;
                }
            }

            return minEl;
        }


        /// <summary>
        /// Find the closest collider distance from a position
        /// </summary>
        public static double ClosestDistance(Vector3D position)
        {
            double minDist = Camera.Main.ClipPlanes.y;

            foreach(Element element in Sandbox.LoadedElements)
            {
                Primitive m = element as Primitive;

                if (m == null) continue;

                double distance = m.DistanceFromSurface(position);

                if (distance < minDist)
                {
                    minDist = distance;
                }
            }

            return minDist;
        }

        /// <summary>
        /// Find the closest collider distance from a position
        /// </summary>
        public static double ClosestDistanceSquared(Vector3D position)
        {
            double minDist = Camera.Main.ClipPlanes.y;
            minDist *= minDist;

            foreach (Element element in Sandbox.LoadedElements)
            {
                Primitive m = element as Primitive;

                if (m == null) continue;

                double distance = m.DistanceFromSurfaceSquared(position);

                if (distance < minDist)
                {
                    minDist = distance;
                }
            }

            return Math.Sqrt(minDist);
        }

        public static implicit operator bool(Element exists)
        {
            return exists != null;
        }
    }
}
