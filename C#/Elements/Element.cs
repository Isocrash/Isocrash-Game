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
        internal static List<Element> _LoadedElements = new List<Element>();
        internal static List<Element> _EnabledElements = new List<Element>();
        public Element(string name)
        {
            this.Name = name;
            _LoadedElements.Add(this);
        }
        public Element()
        {
            _LoadedElements.Add(this);
        }

        public Guid GUID { get; private set; } = Guid.NewGuid();
        public string Name { get; set; } = "Element";

        public bool Enabled
        {
            get
            {
                return _Enabled;
            }

            set
            {
                // If the element was not enabled and Enabled is set to true,
                // add it to the loaded list.
                //
                // Otherwise, remove it from it.
                if(value && !_Enabled)
                {
                    _EnabledElements.Add(this);
                }

                else
                {
                    _EnabledElements.Remove(this);
                }

                _Enabled = value;
            }
        }
        private bool _Enabled = true;


        public virtual void Destroy()
        {
            _LoadedElements.Remove(this);
            if (Enabled) _EnabledElements.Remove(this);
        }

        public static implicit operator bool(Element exists)
        {
            return exists != null;
        }
    }
}
