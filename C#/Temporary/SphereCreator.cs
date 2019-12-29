using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raymarcher
{
    class SphereCreator : Element
    {
        public override void Update()
        {
            if (Input.Triggering(System.Windows.Input.Key.Enter))
            {
                CreateSphere();
            }
        }

        public static void CreateSphere()
        {
            Vector3D pos = new Vector3D(EngineInitializer.r.NextDouble() * 4 - 2, EngineInitializer.r.NextDouble() * 4 - 2, EngineInitializer.r.NextDouble() * 4 - 2);
            new Sphere()
            {
                Position = pos,
                Scale = Vector3D.Positive * EngineInitializer.r.NextDouble()
            };
        }

        [EngineInitializer(100)]
        public static void Init()
        {
            new SphereCreator();
        }
    }
}
