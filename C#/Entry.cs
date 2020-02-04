using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Drawing;

namespace Raymarcher
{
    internal static class Entry
    {

        [STAThread]
        private static void Main()
        {
            File.Initialize();
            Log.Initialize();

            try
            {
                EngineInitializer.Fire();
            }
            catch(Exception e)
            {
                Log.InstantPrint(e);
            }
        }


        [EngineInitializer(int.MaxValue)]
        public static void InitializeWindowsForms()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GameWindow());
        }

        public static void ExecuteOnMainThread(Action action)
        {
            try
            {
                if (GameWindow.Instance != null)
                    GameWindow.Instance.Render.Invoke(action);
            }
            catch { }
        }
    }
}
