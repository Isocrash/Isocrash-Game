using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Raymarcher
{
    public static class Log
    {
        [EngineInitializer(1)]
        public static void Initialize()
        {
            string path = File.Root + File.GetPath(FolderType.Logs) + @"\Engine.log";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            //Log.Print("TEST");

            Updater.OnFixedUpdate += Print;
        }

        //private static TextWriter logWritter;
        private static List<string> PrintOnEndOfFrame { get; set; } = new List<string>();

        private static void Print()
        {
            string path = File.Root + File.GetPath(FolderType.Logs) + @"\Engine.log";
            using (StreamWriter logWritter = new StreamWriter(path, true))
            {
                foreach (string s in PrintOnEndOfFrame)
                {
                    logWritter.WriteLine(s);
                }
                PrintOnEndOfFrame.Clear();
            }
        }
        public static void Print(object content)
        {
            DateTime n = DateTime.Now;
            PrintOnEndOfFrame.Add(n + n.Millisecond.ToString() + " " + content.ToString());
        }
        public static void InstantPrint(object content)
        {
            string path = File.Root + File.GetPath(FolderType.Logs) + @"\Engine.log";
            using (StreamWriter logWritter = new StreamWriter(path, true))
            {
                logWritter.WriteLine(content);
            }
        }
    }
}
