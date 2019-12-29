using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raymarcher
{
    public class Module : Element
    {
        internal static List<Module> _LoadedModules = new List<Module>();

        internal static List<Module> _LoadNextFrame = new List<Module>();

        public Module() : base()
        {
            _LoadNextFrame.Add(this);
        }

        public override void Destroy()
        {
            _LoadedModules.Remove(this);
            base.Destroy();
        }

        public Malleable Malleable { get; internal set; }

        internal protected virtual void OnCreation() { }
        internal protected virtual void PreUpdate() { }
        internal protected virtual void Update() { }
        internal protected virtual void PostUpdate() { }
        internal protected virtual void FixedUpdate() { }
        internal protected virtual void EndUpdate() { }

        internal protected virtual void OnCameraRender() { }
    }
}
