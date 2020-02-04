using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace Raymarcher
{
    public static class Screenshot
    {

        private static bool ScreenshotAsked = false;
        [EngineInitializer(1484)]
        public static void Init()
        {
            Updater.OnFixedUpdate += CheckForScreens;
            Updater.OnPostRender += SaveScreen;
        }

        public static void SaveScreenshot()
        {
            Log.Print("Screenshot!!");
            string path = Path.Combine(File.Root, File.GetPath(FolderType.Screenshots));

            int n = Directory.GetFiles(path, "*.png").Count();
            Camera.Main.RenderImage.Save(path + $@"\screenshot{n}.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        public static void CheckForScreens()
        {
            ScreenshotAsked = Input.Triggering(System.Windows.Input.Key.F1);
        }

        public static void SaveScreen()
        {
            if (ScreenshotAsked)
            {
                SaveScreenshot();
            }
        }
    }
}
