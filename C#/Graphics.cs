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
                Ratio = value.x / (double)value.y;
            }
        }
        private static Vector2I _RenderResolution = new Vector2I(480, 270);
        public static double Ratio { get; internal set; } = 1.0D;
        public static int FrameRateLimit { get; set; } = 60;

        private static Vector2I PreFullScreenResolution;
        private static Vector2I PreFullScreenPosition;
        private static FormWindowState PreFullScreenState;


        public static LockMode ResolutionLockMode { get; set; } = LockMode.Size;
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
            if(ResolutionLockMode == LockMode.None)
            {
                return _RenderResolution;
            }

            else
            {
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
            
            ResizeWindow(116, 139);
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

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("shell32.dll")]
        public static extern UInt32 SHAppBarMessage(UInt32 dwMessage, ref APPBARDATA pData);

        public enum AppBarMessages
        {
            New = 0x00,
            Remove = 0x01,
            QueryPos = 0x02,
            SetPos = 0x03,
            GetState = 0x04,
            GetTaskBarPos = 0x05,
            Activate = 0x06,
            GetAutoHideBar = 0x07,
            SetAutoHideBar = 0x08,
            WindowPosChanged = 0x09,
            SetState = 0x0a
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public UInt32 cbSize;
            public IntPtr hWnd;
            public UInt32 uCallbackMessage;
            public UInt32 uEdge;
            public System.Drawing.Rectangle rc;
            public Int32 lParam;
        }

        public enum AppBarStates
        {
            AutoHide = 0x01,
            AlwaysOnTop = 0x02
        }

        /// <summary>
        /// Set the Taskbar State option
        /// </summary>
        /// <param name="option">AppBarState to activate</param>
        public static void SetTaskbarState(AppBarStates option)
        {
            APPBARDATA msgData = new APPBARDATA();
            msgData.cbSize = (UInt32)Marshal.SizeOf(msgData);
            msgData.hWnd = FindWindow("System_TrayWnd", null);
            msgData.lParam = (Int32)(option);
            SHAppBarMessage((UInt32)AppBarMessages.SetState, ref msgData);
        }

        /// <summary>
        /// Gets the current Taskbar state
        /// </summary>
        /// <returns>current Taskbar state</returns>
        public static AppBarStates GetTaskbarState()
        {
            APPBARDATA msgData = new APPBARDATA();
            msgData.cbSize = (UInt32)Marshal.SizeOf(msgData);
            msgData.hWnd = FindWindow("System_TrayWnd", null);
            return (AppBarStates)SHAppBarMessage((UInt32)AppBarMessages.GetState, ref msgData);
        }
    }
}
