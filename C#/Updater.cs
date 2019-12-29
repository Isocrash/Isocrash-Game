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

        [EngineInitializer(int.MaxValue - 10)]
        public static void InitializeThread()
        {
            Malleable m = new Malleable();
            Camera.Main = m.AddModule<Camera>();
            m.AddModule<Mover>();
            m.AddModule<PlanetSpawner>();


            Task.Factory.StartNew(() => UpdateLoop()).ConfigureAwait(false);
            Task.Factory.StartNew(() => FixedUpdateLoop()).ConfigureAwait(false);
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

                double secs = _FixedWatch.ElapsedMilliseconds / 1000;
                double timeToWait = Time.FixedDeltaTime - secs;

                if (timeToWait > 0)
                {
                    Thread.Sleep((int)(timeToWait * 1000));
                }
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
                        Entry.ExecuteOnMainThread(() => Graphics.ResizeWindow(512, 512));
                        First = false;
                    }

                    //_Watch.Reset();
                    //_Watch.Start();
                    UpdateElements();
                    //_Watch.Stop();

                    
                    //double secs = _Watch.ElapsedMilliseconds / 1000D;


                    /* (Graphics.FrameRateLimit != 0)
                    {
                        double limit = 1.0D / Graphics.FrameRateLimit;

                        double timeToWait = limit - secs;

                        if (timeToWait > 0)
                        {
                            Thread.Sleep((int)(timeToWait * 1000));
                        }
                    }*/
                }
            }
            catch(Exception e)
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
                if (OnUpdate != null)
                    OnUpdate.Invoke();
                foreach (Module mod in Module._LoadedModules.ToArray())
                {
                    if (!mod.Enabled) continue;
                    mod.Update();
                }
                if (OnPostUpdate != null)
                    OnPostUpdate.Invoke();
                foreach (Module mod in Module._LoadedModules.ToArray())
                {
                    if (!mod.Enabled) continue;
                    mod.PostUpdate();
                }
                if (OnEndUpdate != null)
                    OnEndUpdate.Invoke();
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
                if (OnPostRender != null)
                    OnPostRender.Invoke();
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
