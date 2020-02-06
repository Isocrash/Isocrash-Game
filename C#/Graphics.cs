using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

namespace Raymarcher
{
    public static class Graphics
    {
        internal static Vector2I _BordersSize
        {
            get
            {
                return new Vector2I(16, 39);
            }
        }

        public static Vector2I WindowSize { get; private set; }
        public static Vector2I RenderResolution
        {
            get
            {
                return _RenderResolution;
            }

            set
            {
                _RenderResolution = value;
                Ratio = value.x / (double)value.y;
            }
        }

        public static PictureBoxSizeMode RenderSizeMode
        {
            get
            {
                return GameWindow.Instance.Render.SizeMode;
            }

            set
            {
                GameWindow.Instance.Render.SizeMode = value;
            }
        }



        private static Vector2I _RenderResolution = new Vector2I(1024, 1024);
        public static double Ratio { get; internal set; } = 1.0D;
        public static int FrameRateLimit { get; set; } = 60;



        public static RenderProc Processor { get; set; } = RenderProc.NativeCPU;

        private static Vector2I PreFullScreenResolution;
        private static Vector2I PreFullScreenPosition;
        private static FormWindowState PreFullScreenState;


        public static LockMode ResolutionLockMode { get; set; } = LockMode.None;
        public static WindowMode RenderMode { get; private set; } = WindowMode.Windowed;

        public static void SetWindowMode(WindowMode mode)
        {

            if (GameWindow.Instance == null) return;

            Entry.ExecuteOnMainThread(() =>
            {

                switch (mode)
                {
                    case WindowMode.Windowed:
                        {
                            GameWindow.Instance.Width = PreFullScreenResolution.x - _BordersSize.x;
                            GameWindow.Instance.Height = PreFullScreenResolution.y - _BordersSize.y;

                            GameWindow.Instance.Left = PreFullScreenPosition.x;
                            GameWindow.Instance.Top = PreFullScreenPosition.y;

                            GameWindow.Instance.FormBorderStyle = FormBorderStyle.Sizable;

                            GameWindow.Instance.WindowState = PreFullScreenState;
                        }
                        break;

                    case WindowMode.FullScreenWindowed:
                        {
                            PreFullScreenResolution = new Vector2I(GameWindow.Instance.Width, GameWindow.Instance.Height);
                            PreFullScreenPosition = new Vector2I(GameWindow.Instance.Left, GameWindow.Instance.Top);
                            PreFullScreenState = GameWindow.Instance.WindowState;

                            if (GameWindow.Instance.WindowState == FormWindowState.Maximized)
                            {
                                GameWindow.Instance.WindowState = FormWindowState.Normal;
                            }
                            GameWindow.Instance.FormBorderStyle = FormBorderStyle.None;

                            GameWindow.Instance.Left = 0;
                            GameWindow.Instance.Top = 0;

                            Screen s = Screen.FromControl(GameWindow.Instance);

                            GameWindow.Instance.Width = s.Bounds.Width + _BordersSize.x;
                            GameWindow.Instance.Height = s.Bounds.Height + _BordersSize.y;

                        }
                        break;

                    case WindowMode.FullScreen:
                        {
                            SetWindowMode(WindowMode.FullScreenWindowed);
                        }
                        break;
                }
            });
            RenderMode = mode;
        }

        internal static void ResizeWindow(int x, int y) { ResizeWindow(new Vector2I(x, y)); }
        internal static void ResizeWindow(Vector2I size)
        {
            if (GameWindow.Instance == null) return;

            Ratio = (double)size.x / size.y;

            GameWindow.Instance.Size = new System.Drawing.Size(size.x + _BordersSize.x, size.y + _BordersSize.y);
            //GameWindow.Instance.Render.Size = new System.Drawing.Size(size.x, size.y);
        }

        internal static Vector2I GetRenderResolution()
        {
            if (ResolutionLockMode == LockMode.None)
            {
                return _RenderResolution;
            }

            else
            {
                if (GameWindow.Instance == null) return _RenderResolution;
                System.Drawing.Size s = GameWindow.Instance.Size;

                if (RenderMode == WindowMode.Windowed)
                    return new Vector2I(s.Width - _BordersSize.x, s.Height - _BordersSize.y);
                else return new Vector2I(s.Width, s.Height);
            }
        }

        public static bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        [EngineInitializer(10001)]
        public static void Initialize()
        {
            Updater.OnUpdate += CheckOnUpdate;
        }

        private static void CheckOnUpdate()
        {
            if (Input.Triggering(System.Windows.Input.Key.F11))
            {
                Entry.ExecuteOnMainThread(() =>
                {
                    if (RenderMode == WindowMode.Windowed)
                    {
                        SetWindowMode(WindowMode.FullScreenWindowed);
                    }
                    else
                    {
                        SetWindowMode(WindowMode.Windowed);
                    }
                }
                );
            }

            else if (Input.Triggering(System.Windows.Input.Key.F12))
            {
                Entry.ExecuteOnMainThread(() =>
                {
                    if (RenderMode == WindowMode.Windowed || RenderMode == WindowMode.FullScreenWindowed)
                    {
                        SetWindowMode(WindowMode.FullScreen);
                    }
                    else
                    {
                        SetWindowMode(WindowMode.Windowed);
                    }
                }
                );
            }
        }

        public enum LockMode
        {
            None,
            Size
        }

        public enum WindowMode
        {
            Windowed,
            FullScreenWindowed,
            FullScreen
        }

        public enum RenderProc
        {
            NativeCPU,
            CUDA
        }
    }
}
