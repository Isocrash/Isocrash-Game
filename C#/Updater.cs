using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Raymarcher
{
    internal static class Updater
    {
        private static readonly Stopwatch _Watch = new Stopwatch();

        [EngineInitializer(int.MaxValue - 100)]
        public static void InitializeThread()
        {
            OnEndUpdate += RefreshInfos;
            OnPostRender += PostRenderFPS;

            Thread tf = new Thread(FixedUpdateLoop);
            tf.Priority = ThreadPriority.Highest;
            tf.IsBackground = true;
            tf.Start();

            Thread tnf = new Thread(UpdateLoop);
            tnf.Priority = ThreadPriority.Highest;
            tnf.IsBackground = true;
            tnf.Start();

        }

        private static double timeSinceLastFPS = 1.0D;
        private static int usedFPS = 0;
        private static int usedFPSRender = 0;
        private static double usedMSRender = 0;
        private static int frameCount = 0;
        private static double refreshTime = 0.5D;
        private static void RefreshInfos()
        {
            Entry.ExecuteOnMainThread(() =>
            {
                timeSinceLastFPS += Time.DeltaTime;
                double updateTime = _Watch.Elapsed.TotalMilliseconds;

                int fps = (int)Math.Round(1D / updateTime * 1000D, 0);
                double msRender = Math.Round(Rendering.Renderer.RenderTime, 2);

                if(timeSinceLastFPS >= refreshTime)
                {
                    timeSinceLastFPS = 0.0D;
                    usedFPS = (int)(frameCount / refreshTime);
                    usedFPSRender = (int)Math.Round(1D / Rendering.Renderer.RenderTime * 1000D, 0);
                    usedMSRender = msRender;
                    frameCount = 0;
                }

                //double msRenderTime = Math.Round(Rendering.Renderer.RenderTime, 2);
                GameWindow.Instance.lbFPS.Text = usedFPS + " FPS";
            });
        }
        private static void PostRenderFPS()
        {
            frameCount++;
        }



        static bool First = true;

        private static void FixedUpdateLoop()
        {
            while (true)
            {
                _FixedWatch.Reset();
                _FixedWatch.Start();
                FixedUpdateElements();
                _FixedWatch.Stop();

                double secs = _FixedWatch.ElapsedMilliseconds / 1000D;
                double timeToWait = Time.FixedDeltaTime - secs;

                if (timeToWait > 0) Thread.Sleep((int)(timeToWait * 1000));
            }
        }

        private static void UpdateLoop()
        {
            try
            {
                while (true)
                {
                    if (First && GameWindow.Instance != null)
                    {
                        Entry.ExecuteOnMainThread(() => Graphics.ResizeWindow(1024, 1024));
                        First = false;
                    }

                    _Watch.Reset();
                    _Watch.Start();
                    UpdateElements();
                    _Watch.Stop();


                    double secs = _Watch.ElapsedMilliseconds / 1000D;


                    if (Graphics.FrameRateLimit != 0)
                    {
                        double limit = 1.0D / Graphics.FrameRateLimit;

                        double timeToWait = Math.Abs(limit - secs);

                        if (timeToWait > 0)
                        {
                            Thread.Sleep((int)(timeToWait * 1000));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.InstantPrint(e.Message + Environment.NewLine + e.StackTrace);
            }
        }

        private static readonly Stopwatch _FixedWatch = new Stopwatch();

        internal delegate void UpdaterDelegate();

        internal static UpdaterDelegate OnPreUpdate;
        internal static UpdaterDelegate OnUpdate;
        internal static UpdaterDelegate OnPostUpdate;
        internal static UpdaterDelegate OnEndUpdate;
        internal static UpdaterDelegate OnPostRender;

        internal static UpdaterDelegate OnFixedUpdate;

        internal static void FixedUpdateElements()
        {
            try
            {
                foreach (Module mod in Module._LoadedModules.ToArray())
                {
                    mod.FixedUpdate();
                }

                if (OnFixedUpdate != null) OnFixedUpdate.Invoke();

            }
            catch (Exception e) { Log.Print("ERROR " + e.Message + Environment.NewLine + e.StackTrace); }
        }
        internal static void UpdateElements()
        {
            try
            {
                if (OnPreUpdate != null)
                    OnPreUpdate.Invoke();
                foreach (Module mod in Module._LoadedModules.ToArray())
                {
                    if (!mod.Enabled) continue;
                    mod.PreUpdate();
                }
                OnUpdate?.Invoke();
                foreach (Module mod in Module._LoadedModules.ToArray())
                {
                    if (!mod.Enabled) continue;
                    mod.Update();
                }
                OnPostUpdate?.Invoke();
                foreach (Module mod in Module._LoadedModules.ToArray())
                {
                    if (!mod.Enabled) continue;
                    mod.PostUpdate();
                }
                OnEndUpdate?.Invoke();
                foreach (Module mod in Module._LoadedModules.ToArray())
                {
                    if (!mod.Enabled) continue;
                    mod.EndUpdate();
                }

                foreach (Module mod in Module._LoadedModules.ToArray())
                {
                    if (!mod.Enabled) continue;
                    mod.OnCameraRender();
                }
                OnPostRender?.Invoke();
                /*foreach (Module mod in Module._LoadedModules.ToArray())
                {
                    if (!mod.Enabled) continue;
                    mod.OnCameraRender();
                }*/

                Element._LoadedElements.AddRange(Element._SpawnNextFrame);
                Element._SpawnNextFrame = new List<Element>();

                Module._LoadedModules.AddRange(Module._LoadNextFrame);
                Module._LoadNextFrame = new List<Module>();


                Entry.ExecuteOnMainThread(() =>
                {
                    GameWindow.Instance.Render.Image = Camera.Main.RenderImage;
                    //Log.Print(Camera.Main.Malleable.Name);
                });
            }

            catch (Exception e) { Log.Print("ERROR " + e.Message + Environment.NewLine + e.StackTrace); }
        }
    }
}
