using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using SFile = System.IO.File;
using VFile = Raymarcher.File;

namespace Raymarcher
{
    public static class File
    {
        public static string Root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Raymarcher\";

        private static readonly string[] _FolderPaths = { @"Logs\", @"Screenshots\" };

        //TODO: Automatically initialize at start

        public static void Initialize()
        {
            if(!Directory.Exists(Root))
            {
                Directory.CreateDirectory(Root);
            }

            for (int i = 0; i < _FolderPaths.Length; i++)
            {
                if (!Directory.Exists(Root + _FolderPaths[i]))
                {
                    Directory.CreateDirectory(Root + _FolderPaths[i]);
                }
            }

            //Log.Print("File system initialized");
        }

        public static string GetPath(FolderType type)
        {
            return _FolderPaths[(int)type];
        }

        public static void Delete(string localPath)
        {
            string path = Path.Combine(Root + localPath);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        public static void Create(string localPath, bool overwrite)
        {
            string path = Path.Combine(Root + localPath);

            if (Directory.Exists(path))
            {
                if (overwrite)
                {
                    Delete(localPath);
                }

                else return;
            };

            /*if (Directory.Exists(Path.Combine(path, @"\..")))
            {*/
                SFile.Create(path);
            //}
        }

        public static void Archive(string localPath, string localArchivePath)
        {
            string path = Path.Combine(Root + localPath);
            if (Directory.Exists(path))
            {
                string zipPath = Path.Combine(Root + localArchivePath);

                using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    //ZipFile.CreateFromDirectory(path, zipPath);
                }

                Directory.Delete(path, true);
            }
        }
    }

    public enum FolderType
    {
        Logs,
        Screenshots
    }
}
