using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Input;
using System.Windows.Forms;
using FCursor = System.Windows.Forms.Cursor;

namespace Raymarcher
{
    public enum CursorLockMode
    {
        Free,
        Confined,
        Locked
    }

    public static class Input
    {

        private static Key[] _ALL_KEYS;

        private static List<Key> _PreviousFramePressedKeys = new List<Key>();
        private static List<Key> _ThisFramePressedKeys = new List<Key>();
        private static List<Key> _PressedDownKeys = new List<Key>();
        private static List<Key> _PressedUpKeys = new List<Key>();

        private static List<MouseButtons> _NextUpdateClicked = new List<MouseButtons>();
        private static List<MouseButtons> _ClickedThisFrame = new List<MouseButtons>();

        public static CursorLockMode CursorMode = CursorLockMode.Locked;
        public static bool ShowCursor
        {
            get
            {
                return _ShowCursor;
            }

            set
            {
                if(value)
                {
                    FCursor.Show();
                }

                else
                {
                    FCursor.Hide();
                }

                _ShowCursor = value;
            }
        }
        private static bool _ShowCursor = true;


        public static bool Pressed(Key key)
        {
            return _ThisFramePressedKeys.Contains(key);
        }
        public static bool Triggering(Key key)
        {
            return _PressedDownKeys.Contains(key);
        }
        public static bool Releasing(Key key)
        {
            return _PressedUpKeys.Contains(key);
        }

        public static bool Clicked(MouseButtons button)
        {
            return _ClickedThisFrame.Contains(button);
        }

        public static Vector2I CursorPosition { get; private set; }
        public static Vector2I CursorMovement { get; private set; }
        public static int Scroll { get; private set; }

        [EngineInitializer(4)]
        public static void Initialize()
        {
            InitKeyList();
            Updater.OnPreUpdate += PreUpdate;
            Updater.OnEndUpdate += EndUpdate;
        }

        private static void InitKeyList()
        {
            _ALL_KEYS = Enum.GetValues(typeof(Key)) as Key[];
        }

        private static void PreUpdate()
        {
            
            if(Graphics.ApplicationIsActivated())
            {
                Entry.ExecuteOnMainThread(new Action(CalculateKeyInfos));
                Entry.ExecuteOnMainThread(new Action(CalculateMouseInfos));
            }

            Entry.ExecuteOnMainThread(() =>
            {
                switch (CursorMode)
                {
                    case CursorLockMode.Free:
                        {
                        }
                        break;
                    case CursorLockMode.Confined:
                        {
                            if (!Graphics.ApplicationIsActivated()) return;

                            GameWindow w = GameWindow.Instance;

                            w.Cursor = new FCursor(FCursor.Current.Handle);
                            System.Drawing.Point l = w.Location;
                            l.Y += Graphics._BordersSize.y - (Graphics._BordersSize.y / 4) + 1;
                            l.X += Graphics._BordersSize.X / 2;


                            System.Drawing.Size s = w.Size;
                            s.Height -= Graphics._BordersSize.y;
                            s.Width -= Graphics._BordersSize.x;

                            FCursor.Clip = new System.Drawing.Rectangle(l, s);
                        }
                        break;
                    case CursorLockMode.Locked:
                        {
                            if (!Graphics.ApplicationIsActivated()) return;

                            GameWindow w = GameWindow.Instance;

                            if (w == null) return;
                            w.Cursor = new FCursor(FCursor.Current.Handle);

                            //Center of render window
                            System.Drawing.Point wp = w.Location;
                            Vector2I winPos = new Vector2I(wp.X, wp.Y);
                            System.Drawing.Size ws = w.Size;
                            Vector2I winSize = new Vector2I(ws.Width, ws.Height);
                            Vector2I cp = winPos + (winSize / 2) + new Vector2I(0, Graphics._BordersSize.y / 2);

                            Vector2I previous = new Vector2I(FCursor.Position.X, FCursor.Position.Y);

                            FCursor.Position = new System.Drawing.Point(cp.x, cp.y);

                            CursorMovement = previous - cp;

                            if (_ShowCursor)
                            {
                                FCursor.Show();
                            }

                            else FCursor.Hide();
                        }
                        break;
                }
            });
        }
        private static void EndUpdate()
        {
            _ClickedThisFrame = _NextUpdateClicked;
            _NextUpdateClicked = new List<MouseButtons>();
        }
        private static void CalculateMouseInfos()
        {
            Vector2I previousCursorPosition = CursorPosition;

            CalculateCursorPosition();

            CursorMovement = CursorPosition - previousCursorPosition;

            Scroll = SystemInformation.MouseButtons;
            
        }
        private static void CalculateCursorPosition()
        {
            System.Drawing.Point cp = FCursor.Position;
            Vector2I cursorPos = new Vector2I(cp.X, cp.Y);

            System.Drawing.Point wp = GameWindow.Instance.Location;
            Vector2I winPos = new Vector2I(wp.X, wp.Y);

            cursorPos = cursorPos - winPos;
            cursorPos.x -= Graphics._BordersSize.X / 2;
            cursorPos.y -= 31;

            cursorPos.y = Graphics.WindowSize.y - cursorPos.y;

            CursorPosition = cursorPos;
        }
        private static void CalculateKeyInfos()
        {
            
            _PressedDownKeys = new List<Key>();
            _PressedUpKeys = new List<Key>();

            //Saving the old keys
            _PreviousFramePressedKeys = _ThisFramePressedKeys;

            List<Key> pressedKeys = new List<Key>();

            for (int i = 0; i < _ALL_KEYS.Length; i++)
            {
                if (_ALL_KEYS[i] == Key.None) continue;

                if (Keyboard.IsKeyDown(_ALL_KEYS[i]))
                {
                    //If the key was not pressed in the previous frame, add it to the key down list
                    if (!_PreviousFramePressedKeys.Contains(_ALL_KEYS[i]))
                    {
                        _PressedDownKeys.Add(_ALL_KEYS[i]);
                    }

                    pressedKeys.Add(_ALL_KEYS[i]);
                }

                //If the key is not down, check if it was at last fame
                else
                {
                    if (_PreviousFramePressedKeys.Contains(_ALL_KEYS[i]))
                    {
                        _PressedUpKeys.Add(_ALL_KEYS[i]);
                    }
                }
            }

            _ThisFramePressedKeys = pressedKeys;
        }
        internal static void AddClicked(MouseButtons button)
        {
            _NextUpdateClicked.Add(button);
        }

    }
}
