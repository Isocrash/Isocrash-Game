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

        [EngineInitializer(100001)]
        public static void InitializeThread()
        {
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
                    System.Threading.Thread.Sleep((int)(timeToWait * 1000));
                }
            }
        }

        static int noFrame = 0;
        private static void UpdateLoop()
        {
            try
            {
                while (true)
                {
                    Log.Print("============================");
                    Log.Print($"= FRAME UPDATE TRACER #{noFrame}   =");
                    Log.Print("============================");
                    if (First && GameWindow.Instance != null)
                    {
                        Entry.ExecuteOnMainThread(() => Graphics.ResizeWindow(512, 512));
                        First = false;
                    }

                    _Watch.Reset();
                    _Watch.Start();
                    UpdateElements();
                    long elementTime = _Watch.ElapsedMilliseconds;
                    _Watch.Reset();
                    _Watch.Start();
                    UpdateRenders();
                    long rendersTime = _Watch.ElapsedMilliseconds;
                    _Watch.Stop();

                    Log.Print("Total update time: " + (elementTime + rendersTime) + "ms" + "(render: " + rendersTime + "ms)");
                    double secs = _Watch.ElapsedMilliseconds / 1000D;


                    if (Graphics.FrameRateLimit != 0)
                    {
                        double limit = 1.0D / Graphics.FrameRateLimit;

                        double timeToWait = limit - secs;

                        if (timeToWait > 0)
                        {
                            System.Threading.Thread.Sleep((int)(timeToWait * 1000));
                        }
                    }

                    if (OnUpdate != null)
                        OnUpdate.Invoke();

                    Log.Print("============================");
                    Log.Print($"=   END OF FRAME UPDATE    =");
                    Log.Print("============================");

                    noFrame++;
                }
            }
            catch(Exception e)
            {
                Log.InstantPrint(e.Message + Environment.NewLine + e.StackTrace);
            }
        }
        
        private static readonly Stopwatch _FixedWatch = new Stopwatch();

        internal delegate void UpdaterDelegate();

        internal static UpdaterDelegate OnUpdate;
        internal static UpdaterDelegate OnFixedUpdate;

        internal static void FixedUpdateElements()
        {
            try
            {
                foreach (Element element in Sandbox.LoadedElements)
                {
                    element.FixedUpdate();
                }

                if (OnFixedUpdate != null) OnFixedUpdate.Invoke();

            }
            catch (Exception e) { Log.Print("ERROR " + e.Message + e.StackTrace); }
        }
        internal static void UpdateElements()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            //Log.Print("==== PRE UPDATE ====");
            foreach (Element element in Sandbox.LoadedElements)
            {
                //sw.Start();
                element.PreUpdate();
                /*Log.Print(element.Name + " preupdate time: " + sw.ElapsedMilliseconds + "ms");
                sw.Stop();
                sw.Reset();*/
            }
            //Log.Print("==== UPDATE ====");
            foreach (Element element in Sandbox.LoadedElements)
            {
                //sw.Start();
                element.Update();
                //sw.Stop();
                //Log.Print(element.Name + " update time: " + sw.ElapsedMilliseconds + "ms");
                //sw.Reset();
            }
            //Log.Print("==== POST UPDATE ====");
            foreach (Element element in Sandbox.LoadedElements)
            {
                //sw.Start();
                element.PostUpdate();
                //sw.Stop();
                //Log.Print(element.Name + " post update time: " + sw.ElapsedMilliseconds + "ms");
                //sw.Reset();
            }
            //Log.Print("==== END UPDATE ====");
            foreach (Element element in Sandbox.LoadedElements)
            {
                //sw.Start();
                element.EndUpdate();
                //sw.Stop();
                //Log.Print(element.Name + " end update time: " + sw.ElapsedMilliseconds + "ms");
                //sw.Reset();
            }
            Sandbox.LoadedElements.AddRange(Sandbox.NextToSpawn);
            Sandbox.NextToSpawn = new List<Element>();
            
            
        }
        internal static void UpdateRenders()
        {
            try
            {
                Camera.Main.RenderImage();

                Entry.ExecuteOnMainThread(() => {
                    GameWindow.Instance.Render.Image = Camera.Main.Render;
                });
            }

            catch(Exception e) { Log.Print("ERROR " + e.Message); }
        }
    }

    public static class TaskHelper
    {
        /// <summary>
        /// Runs a TPL Task fire-and-forget style, the right way - in the
        /// background, separate from the current thread, with no risk
        /// of it trying to rejoin the current thread.
        /// </summary>
        public static void RunBg(Func<Task> fn)
        {
            Task.Run(fn).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs a task fire-and-forget style and notifies the TPL that this
        /// will not need a Thread to resume on for a long time, or that there
        /// are multiple gaps in thread use that may be long.
        /// Use for example when talking to a slow webservice.
        /// </summary>
        public static void RunBgLong(Func<Task> fn)
        {
            Task.Factory.StartNew(fn, TaskCreationOptions.LongRunning)
                .ConfigureAwait(false);
        }
    }
}
