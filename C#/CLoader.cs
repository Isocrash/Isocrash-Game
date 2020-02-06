using System;
using System.Collections.Generic;
using System.IO;
using OpenCL.Net;

namespace Raymarcher.Rendering
{
    internal static class CLoader
    {
        /// <summary>
        /// Load a C file.
        /// </summary>
        /// <param name="relativePath">Path relative to exe</param>
        private static string LoadC(string path)
        {
            if(System.IO.File.Exists(path))
            {
                List<string> script = new List<string>();

                using(StreamReader sr = new StreamReader(path))
                {
                    while(!sr.EndOfStream)
                    {
                        script.Add(sr.ReadLine());
                    }
                }

                return String.Join(System.Environment.NewLine, script);
            }

            else
            {
                return null;
            }
        }

        /// <summary>
        /// Load a C Script to a readable program.
        /// </summary>
        /// <param name="path">The path of the .c file</param>
        /// <param name="device">The device to compile for</param>
        /// <param name="context">The context of the device</param>
        /// <returns></returns>
        public static Program LoadProgram(string[] filePaths, Device device, Context context)
        {
            string[] files = new string[filePaths.Length];
            for (int i = 0; i < filePaths.Length; i++)
            {
                files[i] = LoadC(filePaths[i]);
            }
                

            Program program = Cl.CreateProgramWithSource(context, (uint)files.Length, files, null, out ErrorCode builderror);
            builderror = Cl.BuildProgram(program, 0, null, "-I headers/", null, IntPtr.Zero);

            //string[] paths = path.Split('\\');
            //string filename = paths[paths.Length - 1];

            if (Cl.GetProgramBuildInfo(program, device, ProgramBuildInfo.Status, out ErrorCode error).CastTo<BuildStatus>() != BuildStatus.Success)
            {
                if (builderror != ErrorCode.Success)
                {
                    //Console.ForegroundColor = ConsoleColor.Red;
                    Log.Print("Error during compilation:\n" + Cl.GetProgramBuildInfo(program, device, ProgramBuildInfo.Log, out error));
                }
            }

            else
            {
                //Console.ForegroundColor = ConsoleColor.Green;
                Log.Print("C files compiled with no error.");
            }

            Console.ResetColor();

            return program;
        }
    }
}
