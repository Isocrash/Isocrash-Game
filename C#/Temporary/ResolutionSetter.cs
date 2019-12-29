using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Raymarcher
{
    public class ResolutionSetter : Element
    {
        public override void Update()
        {
            
            if(Input.Triggering(Key.P))
            {
                if(Graphics.ResolutionLockMode == Graphics.LockMode.Size)
                {
                    Graphics.ResolutionLockMode = Graphics.LockMode.None;
                }

                else Graphics.ResolutionLockMode = Graphics.LockMode.Size;

                Log.Print(Graphics.ResolutionLockMode);
                //Graphics.LockMode  //= new Vector2I(200, 200);
            }
        }

        [EngineInitializer(123046)]
        public static void Init()
        {
            new ResolutionSetter();
        }
    }
}
