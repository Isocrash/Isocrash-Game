using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raymarcher.Debug
{
    public class ResolutionSwitcher : Element
    {
        public override void Update()
        {
            if (Input.Triggering(System.Windows.Input.Key.F11))
            {
                Entry.ExecuteOnMainThread(() =>
                {
                    if (Graphics.RenderMode == Graphics.WindowMode.Windowed)
                    {
                        Graphics.SetWindowMode(Graphics.WindowMode.FullScreenWindowed);
                    }
                    else
                    {
                        Graphics.SetWindowMode(Graphics.WindowMode.Windowed);
                    }
                }
                );
            }

            else if (Input.Triggering(System.Windows.Input.Key.F12))
            {
                Entry.ExecuteOnMainThread(() =>
                {
                    if (Graphics.RenderMode == Graphics.WindowMode.Windowed || Graphics.RenderMode == Graphics.WindowMode.FullScreenWindowed)
                    {
                        Graphics.SetWindowMode(Graphics.WindowMode.FullScreen);
                    }
                    else
                    {
                        Graphics.SetWindowMode(Graphics.WindowMode.Windowed);
                    }
                }
                );
            }
        }

        [EngineInitializer(648)]
        public static void Initialize()
        {
            
            new ResolutionSwitcher();
        }
    }
}
