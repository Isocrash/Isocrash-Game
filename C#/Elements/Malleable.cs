using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raymarcher
{
    public class Malleable : Element
    {
        /// <summary>
        /// The malleable this is attached to.
        /// </summary>
        public Malleable Parent { get; set; } = null;

        public Vector3D Position { get; set; } = Vector3D.Null;
        public Quaternion Rotation { get; set; } = EQuaternion.FromEuler(0,0,0); //TODO : Use double quaternion system.
        public Vector3D Scale { get; set; } = Vector3D.Positive;
        public Vector3D Forward
        {
            get
            {
                return Rotation * Vector3D.Forward;
            }
        }

        public Vector3D Right
        {
            get
            {
                return Rotation * Vector3D.Right;
            }
        }

        public Vector3D Up
        {
            get
            {
                return Rotation * Vector3D.Up;
            }
        }

        public Vector3D Translate(Vector3D translation)
        {
            Position += translation;
            return Position;
        }


        public override void Destroy()
        {
            foreach(Module m in _Modules)
            {
                m.Destroy();
            }

            _Modules = null;

            base.Destroy();
        }

        #region Modules
        public T AddModule<T>() where T : Module
        {
            T mod = (T)Activator.CreateInstance(typeof(T));
            _Modules.Add(mod);
            mod.Malleable = this;
            mod.OnCreation();
            return mod;
        }
        public T GetModule<T>() where T : Module
        {
            foreach (Module mod in _Modules) if (mod is T module) return module;
            return null;
        }
        public T[] GetModules<T>() where T : Module
        {
            List<T> modules = new List<T>();

            foreach (Module mod in _Modules) if (mod is T module) modules.Add(module);

            if (modules.Count == 0) return null;

            return modules.ToArray();
        }
        public T AddOrGetModule<T>() where T : Module
        {
            T module = GetModule<T>();

            if (module) return module;

            return AddModule<T>();
        }

        private List<Module> _Modules = new List<Module>();
        #endregion
    }
}
