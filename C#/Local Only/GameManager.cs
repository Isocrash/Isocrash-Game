using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raymarcher;

namespace Raymarcher.Local_Only
{
    public static class GameManager
    {
        [EngineInitializer(90)]
        public static void CreateScene()
        {
            Malleable m = CreateCamera().Malleable;
            m.AddModule<FreeCamController>();//m.AddModule<CharacterController>();



            CreateSun();
        }

        public static Camera CreateCamera()
        {
            Malleable m = new Malleable();
            Camera.Main = m.AddModule<Camera>();
            return Camera.Main;
        }

        public static Light CreateSun()
        {
            Malleable m = new Malleable() { Name = "Sun" };
            Light.Main = m.AddModule<Light>();

            return Light.Main;
        }
    }
}
